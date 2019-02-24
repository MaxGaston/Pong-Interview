using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Photon.PunBehaviour
{
    #region Public Variables

    [Tooltip("The prefab to use for representing the player.")]
    public GameObject PlayerPrefab;

    [Tooltip("The prefab to use for representing the ball.")]
    public GameObject BallPrefab;
    #endregion

    #region Private Variables

    GameObject BallRef;
    #endregion

    #region Public Methods

    /// <summary>
    /// Wrapper function for Photon's LeaveRoom()
    /// </summary>
    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }
    #endregion

    #region Private Methods

    private void LoadGame()
    {
        if(!PhotonNetwork.isMasterClient)
        {
            Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
        }

        Debug.Log("PhotonNetwork : Loading the game.");
        PhotonNetwork.LoadLevel("Game");
    }
    #endregion

    #region MonoBehavior CallBacks

    public void Start()
    {
        if (PlayerPrefab == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
        }
        else
        {
            float x = 0.0f;
            if (PhotonNetwork.countOfPlayersInRooms == 0) x = -8.0f;
            else
            {
                x = 8.0f;
            }
            // PhotonNetwork.Instantiate handles spawning and syncing the player object when the game starts.
            PhotonNetwork.Instantiate(this.PlayerPrefab.name, new Vector3(x, 1f, 0f), Quaternion.identity, 0);
        }

        if(BallPrefab == null)
        {
            Debug.LogError("<Color=Red><a>Missing</a></Color> BallPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
        }
    }
    #endregion

    #region Photon CallBacks

    /// <summary>
    /// Loads the Launcher scene when the player leaves the room.
    /// </summary>
    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer other)
    {
        if(PhotonNetwork.room.PlayerCount == 2)
        {
            BallRef = PhotonNetwork.Instantiate(this.BallPrefab.name, new Vector3(0f, 1f, 0f), Quaternion.identity, 0);
        }
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer other)
    {
        // If someone leaves, move the remaining player to the left side of the field.
        GameObject player = (GameObject)PhotonNetwork.playerList[0].TagObject;
        player.transform.SetPositionAndRotation(new Vector3(-8.0f, 1.0f, 0), Quaternion.identity);

        if(BallRef != null) PhotonNetwork.Destroy(BallRef);
    }
    #endregion
}
