using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Photon.PunBehaviour
{
    public PhotonLogLevel LogLevel = PhotonLogLevel.Informational;

    public Button HostButton;
    public Button JoinButton;
    public Button LeaveButton;
    public Button ServeButton;

    // Reference to the actual player object.
    public GameObject PlayerPrefab;

    // Reference to the local player after instantiation.
    private GameObject PlayerRef;

    public GameObject WhiteBall;
    public GameObject BlackBall;

    private int WhiteBallID;
    private int BlackBallID;

    public float BallSpeed = 5.0f;
    
    // Distance from the center of the screen that players will spawn.
    public float SpawnDistance = 10.0f;
    public float SpawnHeight = 1.5f;

    [PunRPC]
    public void UpdateBall(int ballID, Vector3 pos, Vector3 vel)
    {
        Debug.Log("<color=blue>Update Ball</color>: X = " + vel.x + " Y = " + vel.y + " Z = " + vel.z);

        PhotonView ball = PhotonView.Find(ballID);

        ball.gameObject.transform.position = pos;
        ball.GetComponent<Rigidbody>().velocity = vel;
    }

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
        PhotonNetwork.LeaveRoom();
    }

    public void Serve()
    {
        WhiteBallID = WhiteBall.GetComponent<BallController>().photonView.viewID;
        
        Vector3 vel = Random.insideUnitSphere.normalized * Time.deltaTime * BallSpeed;
        vel.z = 0;

        Debug.Log("<color=blue>Serve</color>: X = " + vel.x + " Y = " + vel.y + " Z = " + vel.z);
        photonView.RPC("UpdateBall", PhotonTargets.All, WhiteBallID, new Vector3(0, SpawnHeight, -1), vel);
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
        Vector3 spawnPoint = new Vector3(SpawnDistance, SpawnHeight, -1);
        if (LeftSideFree)
        {
            spawnPoint.x = -SpawnDistance;
            PlayerRef = PhotonNetwork.Instantiate(this.PlayerPrefab.name, spawnPoint, Quaternion.identity, 0);
            Hashtable Props = new Hashtable() { { "LeftSideFree", false } };
            PhotonNetwork.room.SetCustomProperties(Props);
        }
        else
        {
            PlayerRef = PhotonNetwork.Instantiate(this.PlayerPrefab.name, spawnPoint, Quaternion.identity, 0);
        }

        // Remember which side this player was spawned on.
        gameObject.transform.position = spawnPoint;

        // DEBUG
        ServeButton.interactable = true;
    }

    public override void OnLeftRoom()
    {
        Debug.Log("<color=blue>Left Room</color>");
        LeaveButton.interactable = false;
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        Debug.Log("<color=blue>Player Connected</color>");

        if (PhotonNetwork.room.PlayerCount == 2)
        {
            ServeButton.interactable = true;
        }
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        // Tell the room the left side is free if we're on the right.
        float xPos = gameObject.transform.position.x;
        if (xPos > 0)
        {
            Hashtable Props = new Hashtable() { { "LeftSideFree", true } };
            PhotonNetwork.room.SetCustomProperties(Props);
        }
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
