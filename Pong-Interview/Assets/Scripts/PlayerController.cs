using UnityEngine;

public class PlayerController : Photon.PunBehaviour
{
    public float MoveSpeed = 5.0f;
    public float TurnSpeed = 10.0f;

    private Rigidbody RBody;

    private void Start()
    {
        RBody = GetComponent<Rigidbody>();    
    }

    private void FixedUpdate()
    {
        /*  
         *  The 'isMine' check will prevent us from controlling other players.
         *  With the 'connection' check, we'll be able to test player input offline.
         *  Without it, the statement would always return early because 'isMine' is false if we're not connected.
         */
        if(photonView.isMine == false && PhotonNetwork.connected)
        {
            return;
        }

        float delta = Input.GetAxis("Vertical");
        RBody.velocity = new Vector3(0, delta, 0) * Time.fixedDeltaTime * MoveSpeed;
    }
}
