using UnityEngine;

public class PlayerController : Photon.PunBehaviour
{
    #region Public Variables
    public float MoveSpeed = 5.0f;

    [Tooltip("The local player instance. Use this to know if the local player is represented in the Scene")]
    public static GameObject LocalPlayerInstance;
    #endregion

    #region Private Variables
    #endregion

    #region Public Methods
    #endregion

    #region Private Methods
    #endregion

    #region MonoBehavior CallBacks
    public void Awake()
    {
        if (photonView.isMine)
        {
            PlayerController.LocalPlayerInstance = this.gameObject;
        }
    }

    public void Update()
    {
        if (photonView.isMine == false && PhotonNetwork.connected == true)
        {
            return;
        }
        GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
        float z = Input.GetAxis("Vertical");

        transform.Translate(new Vector3(0, 0, z * MoveSpeed) * Time.deltaTime);
    }
    #endregion

    #region Photon CallBacks

    public override void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        info.sender.TagObject = this.gameObject;
    }
    #endregion
}
