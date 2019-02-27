using UnityEngine;
using ExitGames.Client.Photon;

public class BallController : Photon.PunBehaviour
{
    public GameObject Player;
    public GameObject GameManager;
    private GameManager GM;
    
    public float BallSpeed;
    public float SpawnHeight;
    private float GoalDistance;
    
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
        // Ball has left the field
        if(transform.position.x >= GoalDistance || transform.position.x <= -GoalDistance)
        {
            GM.photonView.RPC("ResetBalls", PhotonTargets.All);
        }
    }
}
