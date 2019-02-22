using UnityEngine;

public class Launcher : Photon.PunBehaviour
{
    #region Public Variables

    public PhotonLogLevel LogLevel = PhotonLogLevel.Informational;

    [Tooltip("The maximum number of players per room. When no rooms are available to join, incoming players will create their own.")]
    public byte MaxPlayersPerRoom = 2;

    [Tooltip("The UI Panel to let the user connect and play.")]
    public GameObject ControlPanel;

    [Tooltip("The UI Label to inform the user that the connection is in progress.")]
    public GameObject ProgressLabel;
    #endregion

    #region Private Variables

    /// <summary>
    /// This client's version number. Separates users on different versions.
    /// </summary>
    string GameVersion = "1";
    #endregion

    #region Public Methods

    /// <summary>
    /// Start the connection process.
    /// - If already connected, try joining a random room.
    /// - If not connected, connect this application instance to Photon Cloud Network.
    /// </summary>
    public void Connect()
    {
        ProgressLabel.SetActive(true);
        ControlPanel.SetActive(false);

        // If we're connected, try joining a random room.
        if (PhotonNetwork.connected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else 
        {
            // We're not connected yet, so we need to do that now.
            PhotonNetwork.ConnectUsingSettings(GameVersion);
        }
    }
    #endregion

    #region Private Methods
    #endregion

    #region MonoBehavior CallBacks
    private void Awake()
    {
        PhotonNetwork.logLevel = LogLevel;

        PhotonNetwork.autoJoinLobby = false;

        PhotonNetwork.automaticallySyncScene = true;
    }

    public void Start()
    {
        ProgressLabel.SetActive(false);
        ControlPanel.SetActive(true);
    }
    #endregion

    #region Photon CallBacks

    public override void OnConnectedToMaster()
    {
        Debug.Log("Launcher: OnConnectedToMaster() was called by PUN");

        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnDisconnectedFromPhoton()
    {
        ProgressLabel.SetActive(false);
        ControlPanel.SetActive(true);

        Debug.LogWarning("Launcher: OnDisconnectedFromPhoton() was called by PUN");
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        Debug.Log("Launcher:OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
        
        // No available room, let's make our own.
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom }, null);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");
    }

    #endregion
}
