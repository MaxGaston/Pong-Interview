using UnityEngine;
using UnityEngine.UI;

public class BallController : Photon.PunBehaviour
{
    public GameObject Player;
    public GameObject GameManager;
    private GameManager GM;

    public Text LeftScore;
    public Text RightScore;

    private float PlayerHeight;
    private float MaxBounceAngle = 360;
    public float BallSpeed;
    public float SpawnHeight;
    private float GoalDistance;

    private void Start()
    {
        PlayerHeight = Player.transform.lossyScale.y;

        GM = GameManager.GetComponent<GameManager>();
        GoalDistance = GM.SpawnDistance;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Wall")
        {
            Vector3 vel = GetComponent<Rigidbody>().velocity;
            vel.y *= -1;
        }
        else if(collision.gameObject.name == "Player(Clone)")
        {
            float yPos = transform.position.y;
            float paddleY = collision.collider.transform.position.y;

            float IntersectYPos = (paddleY + (PlayerHeight / 2)) - yPos;
            float normalIntersectYPos = IntersectYPos / (PlayerHeight / 2);
            float angle = normalIntersectYPos * MaxBounceAngle;

            float bx = Mathf.Cos(Mathf.Deg2Rad * angle) * BallSpeed;
            float by = Mathf.Sin(Mathf.Deg2Rad * angle) * BallSpeed;

            Vector3 vel = new Vector3(bx, by, 0);

            int ping = PhotonNetwork.GetPing();

            GM.photonView.RPC("UpdateBall", PhotonTargets.Others, photonView.viewID, transform.position, vel, ping);
            GetComponent<Rigidbody>().velocity = vel;
        }
    }

    private void FixedUpdate()
    {
        if(transform.position.x >= GoalDistance)
        {
            int score = int.Parse(LeftScore.text) + 1;
            LeftScore.text = score.ToString();
            GM.photonView.RPC("ResetBalls", PhotonTargets.All);
        }
        else if(transform.position.x <= -GoalDistance)
        {
            int score = int.Parse(RightScore.text) + 1;
            RightScore.text = score.ToString();
            GM.photonView.RPC("ResetBalls", PhotonTargets.All);
        }
    }
}
