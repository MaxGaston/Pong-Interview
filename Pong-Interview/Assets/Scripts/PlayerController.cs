using UnityEngine;

public class PlayerController : Photon.PunBehaviour
{
    public float MoveSpeed = 5.0f;
    public float TurnSpeed = 10.0f;
    public float MaxAngle = 45.0f;

    private Rigidbody RB;
    private float Angle = 0;

    private void Start()
    {
        RB = GetComponent<Rigidbody>();
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
        RB.velocity = new Vector3(0, delta, 0) * Time.fixedDeltaTime * MoveSpeed;
        
        // Allow the player to rotate from -MaxAngle to MaxAngle
        Angle -= Input.GetAxis("Horizontal") * TurnSpeed;
        Angle = Mathf.Clamp(Angle, -MaxAngle, MaxAngle);
        transform.localRotation = Quaternion.AngleAxis(Angle, Vector3.back);
    }
}
