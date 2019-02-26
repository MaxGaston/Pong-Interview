using UnityEngine;

public class BallController : Photon.PunBehaviour
{
    public GameObject GameManager;

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.name == "Wall")
        {
            Debug.Log("<color=blue>Hit Wall</color>");

            Vector3 vel = GetComponent<Rigidbody>().velocity;
            vel.y *= -1;

            Debug.Log("X = " + vel.x + " Y = " + vel.y + " Z = " + vel.z);

            GameManager.GetComponent<GameManager>().photonView.RPC("UpdateBall", PhotonTargets.All, photonView.viewID, gameObject.transform.position, vel);
        }
    }
}
