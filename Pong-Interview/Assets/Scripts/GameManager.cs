using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Photon.PunBehaviour
{
    public PhotonLogLevel LogLevel = PhotonLogLevel.Informational;

    public Button HostButton;
    public Button JoinButton;
    public Button LeaveButton;

    // Reference to the actual player object.
    public GameObject PlayerPrefab;

    // Distance from the center of the screen that players will spawn.
    public float SpawnDistance = 2.375f;

    public void HostGame()
    {
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = 2 }, null);
    }

    public void JoinGame()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public void LeaveGame()
    {
        // Tell the room the left side is free if we were on that side.
        float xPos = gameObject.transform.position.x;
        if (xPos < 0)
        {
            Hashtable Props = new Hashtable() { { "LeftSideFree", true } };
            PhotonNetwork.room.SetCustomProperties(Props);
        }

        PhotonNetwork.LeaveRoom();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("<color=blue>Connected to Master</color>");

        // Once we're connected, we can either host or join a game.
        HostButton.interactable = true;
        JoinButton.interactable = true;
    }

    public override void OnDisconnectedFromPhoton()
    {
        Debug.Log("<color=blue>Disconnected from Photon</color>");
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("<color=blue>Created Room</color>");

        // Since the room was just created, the left side of the arena is empty.
        Hashtable Props = new Hashtable() { {"LeftSideFree", true } };
        PhotonNetwork.room.SetCustomProperties(Props);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("<color=blue>Joined Room</color>");

        // Once in a room, we can now leave but no longer host/join.
        HostButton.interactable = false;
        JoinButton.interactable = false;
        LeaveButton.interactable = true;

        /*
         *  Check for a player on the left side. If there is, spawn new player on the right.
         *  If there isn't, spawn on the left side and update LeftSideFree to false.
         */
        bool LeftSideFree = (bool)PhotonNetwork.room.CustomProperties["LeftSideFree"];
        Vector3 spawnPoint;
        if(LeftSideFree)
        {
            spawnPoint = new Vector3(-SpawnDistance, 2.375f, -1);
            PhotonNetwork.Instantiate(this.PlayerPrefab.name, spawnPoint, Quaternion.identity, 0);
            Hashtable Props = new Hashtable() { { "LeftSideFree", false } };
            PhotonNetwork.room.SetCustomProperties(Props);
        }
        else
        {
            spawnPoint = new Vector3(SpawnDistance, 2.375f, -1);
            PhotonNetwork.Instantiate(this.PlayerPrefab.name, spawnPoint, Quaternion.identity, 0);
        }

        // Remember which side this player was spawned on.
        gameObject.transform.position = spawnPoint;
    }

    public override void OnLeftRoom()
    {
        Debug.Log("<color=blue>Left Room</color>");
        LeaveButton.interactable = false;
    }

    private void Awake()
    {
        PhotonNetwork.autoJoinLobby = false;

        PhotonNetwork.automaticallySyncScene = true;

        PhotonNetwork.logLevel = LogLevel;
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings("1.0");
    }
}
