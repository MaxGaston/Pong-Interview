using UnityEngine;
using UnityEngine.UI;

public class BallController : Photon.PunBehaviour
{
    public GameObject Player;
    public GameObject GameManager;
    private GameManager GM;
    
    private float MaxBounceAngle = 75;
    public float BallSpeed;
    public float SpawnHeight;
    private float GoalDistance;

    public GameObject LeftScore;
    public GameObject RightScore;
    
    private void Start()
    {
        GM = GameManager.GetComponent<GameManager>();
        GoalDistance = GM.SpawnDistance;
    }

    public void OnCollisionEnter(Collision collision)
    {
        Vector3 velocity = GetComponent<Rigidbody>().velocity;
        velocity = velocity.normalized * BallSpeed;

        if (collision.gameObject.name == "Wall")
        {
            velocity.y *= -1;
        }
        else
        {
            int ping = PhotonNetwork.GetPing();
            GM.photonView.RPC("UpdateBall", PhotonTargets.Others, photonView.viewID, transform.position, velocity, ping);
        }
    }

    private void FixedUpdate()
    {
        if(transform.position.x >= GoalDistance) // Ball has exited right side
        {
            GM.photonView.RPC("ResetBalls", PhotonTargets.All);
            GM.photonView.RPC("IncrementScore", PhotonTargets.Others, false, photonView.viewID);

            //LeftScore.GetComponent<ScoreManager>().Increment();
        }
        else if(transform.position.x <= -GoalDistance) // Ball has exited left side
        {
            GM.photonView.RPC("ResetBalls", PhotonTargets.All);
            GM.photonView.RPC("IncrementScore", PhotonTargets.Others, true, photonView.viewID);

            //RightScore.GetComponent<ScoreManager>().Increment();
        }
    }
}
