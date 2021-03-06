﻿using ExitGames.Client.Photon;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : Photon.PunBehaviour
{
    #region Variables
    public PhotonLogLevel LogLevel = PhotonLogLevel.Informational;

    public Button[] Buttons;
    private Button HostButton;
    private Button JoinButton;
    private Button ResetButton;
    private Button LeaveButton;
    private Button ServeButton;
     
    public GameObject PlayerPrefab;

    public GameObject WhiteBall;
    public GameObject BlackBall;

    public float SpawnDistance = 12.0f;

    private Vector3 LeftSpawnPoint;
    private Vector3 RightSpawnPoint;

    public ScoreKeeper ScoreKeeper;
    #endregion

    #region Functions
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

    public void ResetGame()
    {
        ResetBalls();
        ScoreKeeper.ResetScores();
    }

    [PunRPC]
    public void ResetAll()
    {
        photonView.RPC("ResetGame", PhotonTargets.All);
    }

    /// <summary>
    /// Sets the ball given by ballID on a random vector of magnitude BallSpeed.
    /// </summary>
    /// <param name="ballID">The viewID of the ball to serve.</param>
    private void ServeBall(int ballID)
    {
        int ping = PhotonNetwork.GetPing();

        PhotonView ball = PhotonView.Find(ballID);

        float angle = Random.Range(0, 360);
        float bx = Mathf.Cos(Mathf.Deg2Rad * angle) * ball.GetComponent<BallController>().BallSpeed;
        float by = Mathf.Sin(Mathf.Deg2Rad * angle) * ball.GetComponent<BallController>().BallSpeed;

        Vector3 vel = new Vector3(bx, by, 0);

        ball.transform.position = new Vector3(0, ball.GetComponent<BallController>().SpawnHeight, -1);
        photonView.RPC("UpdateBall", PhotonTargets.Others, ballID, new Vector3(0, ball.GetComponent<BallController>().SpawnHeight, -1), vel, ping);

        ball.GetComponent<Rigidbody>().velocity = vel;
    }

    public void ServeBalls()
    {
        ServeBall(WhiteBall.GetPhotonView().viewID);
        ServeBall(BlackBall.GetPhotonView().viewID);
    }
    #endregion

    #region RPCs

    /// <summary>
    /// Sync a ball between both players.
    /// </summary>
    /// <param name="ballID">The viewID of the ball to sync.</param>
    /// <param name="pos">The ball's actual position.</param>
    /// <param name="vel">The ball's actual velocity.</param>
    /// <param name="remotePing">Ping from the remote player. Used to compensate for latency.</param>
    [PunRPC]
    public void UpdateBall(int ballID, Vector3 pos, Vector3 vel, int remotePing)
    {
        PhotonView ball = PhotonView.Find(ballID);

        int ping = PhotonNetwork.GetPing();
        float delay = (float)(ping / 2 + remotePing / 2); // Getting the round trip time for this message

        ball.gameObject.transform.position = pos + (vel * delay / 1000);
        ball.GetComponent<Rigidbody>().velocity = vel;
    }

    /// <summary>
    /// Place a ball at its starting position and zero its velocity.
    /// </summary>
    /// <param name="ballID">The viewID of the ball to reset.</param>
    [PunRPC]
    public void ResetBall(int ballID)
    {
        PhotonView ball = PhotonView.Find(ballID);
        ball.gameObject.transform.position = new Vector3(0, ball.gameObject.GetComponent<BallController>().SpawnHeight, -1);
        ball.gameObject.GetComponent<Rigidbody>().velocity *= 0;
    }
    
    [PunRPC]
    public void ResetBalls()
    {
        ResetBall(WhiteBall.GetPhotonView().viewID);
        ResetBall(BlackBall.GetPhotonView().viewID);
    }
    #endregion

    #region Callbacks
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

        //ResetButton.interactable = true;
        LeaveButton.interactable = true;

        /*
         *  Check for a player on the left side. If there is, spawn new player on the right.
         *  If there isn't, spawn on the left side and update LeftSideFree to false.
         */
        bool LeftSideFree = (bool)PhotonNetwork.room.CustomProperties["LeftSideFree"];
        if (LeftSideFree)
        {
            PhotonNetwork.Instantiate(this.PlayerPrefab.name, LeftSpawnPoint, Quaternion.identity, 0);
            Hashtable Props = new Hashtable() { { "LeftSideFree", false } };
            PhotonNetwork.room.SetCustomProperties(Props);
            gameObject.transform.position = LeftSpawnPoint;
        }
        else
        {
            PhotonNetwork.Instantiate(this.PlayerPrefab.name, RightSpawnPoint, Quaternion.identity, 0);
            gameObject.transform.position = RightSpawnPoint;
        }

        // DEBUG
        if(PhotonNetwork.isMasterClient) ServeButton.interactable = true;
    }

    public override void OnLeftRoom()
    {
        Debug.Log("<color=blue>Left Room</color>");
        //ResetButton.interactable = false;
        LeaveButton.interactable = false;
        ResetGame();
    }

    public override void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        Debug.Log("<color=blue>Player Connected</color>");

        if (PhotonNetwork.room.PlayerCount == 2)
        {
            if(PhotonNetwork.isMasterClient) ServeButton.interactable = true;
        }
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        // Tell the room the left side is free if we're on the right.
        PlayerController pc = otherPlayer.TagObject as PlayerController;
        if (pc.transform.position.x < 0)
        {
            Hashtable Props = new Hashtable() { { "LeftSideFree", true } };
            PhotonNetwork.room.SetCustomProperties(Props);
        }
        ResetGame();
    }

    private void Awake()
    {
        PhotonNetwork.autoJoinLobby = false;

        PhotonNetwork.automaticallySyncScene = true;

        PhotonNetwork.logLevel = LogLevel;
    }

    // Connect to the network and initialize some properties.
    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings("1.0");

        LeftSpawnPoint = new Vector3(-SpawnDistance, 1.5f, -1);
        RightSpawnPoint = new Vector3(SpawnDistance, 1.5f, -1);
        
        HostButton = Buttons[0];
        JoinButton = Buttons[1];
        //ResetButton = Buttons[2];
        LeaveButton = Buttons[2];
        ServeButton = Buttons[3];
    }
    #endregion
}
