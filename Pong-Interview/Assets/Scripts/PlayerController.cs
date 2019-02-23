using UnityEngine;

public class PlayerController : Photon.PunBehaviour
{
    public float MoveSpeed = 5.0f;

    // Update is called once per frame
    void Update()
    {
        if(photonView.isMine == false && PhotonNetwork.connected == true)
        {
            return;
        }
        float z = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(0, 0, z * MoveSpeed) * Time.deltaTime);
    }
}
