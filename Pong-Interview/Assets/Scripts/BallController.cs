using UnityEngine;
using ExitGames.Client.Photon;

public class BallController : Photon.PunBehaviour
{
    public GameObject Player;
    public GameManager GameManager;
    public ScoreKeeper ScoreKeeper;
    
    public float BallSpeed;
    public float SpawnHeight;
    private float GoalDistance;
    
    private void Start()
    {
        GoalDistance = GameManager.SpawnDistance;
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
            GameManager.photonView.RPC("UpdateBall", PhotonTargets.Others, photonView.viewID, transform.position, velocity, ping);
        }
    }

    private void FixedUpdate()
    {
        // Ball has exited right
        if(transform.position.x >= GoalDistance)
        {
            GameManager.photonView.RPC("ResetBalls", PhotonTargets.All);
            ScoreKeeper.Score(true, photonView.viewID);
        }
        // Ball has exited left
        if(transform.position.x <= -GoalDistance)
        {
            GameManager.photonView.RPC("ResetBalls", PhotonTargets.All);
            ScoreKeeper.Score(false, photonView.viewID);
        }
    }
}
