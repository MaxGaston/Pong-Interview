using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Photon.PunBehaviour
{
    #region Public Variables

    [Tooltip("The prefab to use for representing the player")]
    public GameObject PlayerPrefab;
    #endregion

    #region Private Variable
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
            else x = 8.0f;
            // PhotonNetwork.Instantiate handles spawning and syncing the player object when the game starts.
            PhotonNetwork.Instantiate(this.PlayerPrefab.name, new Vector3(x, 1f, 0f), Quaternion.identity, 0);
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
        /*
        if (PhotonNetwork.isMasterClient)
        {
            Debug.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient);


            LoadGame();
        }*/
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer other)
    {
        /*
        if (PhotonNetwork.isMasterClient)
        {
            Debug.Log("OnPhotonPlayerDisonnected isMasterClient " + PhotonNetwork.isMasterClient);


            LoadGame();
        }*/
    }
    #endregion
}
