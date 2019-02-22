using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Photon.PunBehaviour
{
    #region Public Variables
    #endregion

    #region Private Variables
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
