using UnityEngine;
using UnityEngine.UI;

public class GameManager : Photon.PunBehaviour
{
    public PhotonLogLevel Loglevel = PhotonLogLevel.Informational;

    public Button HostButton;
    public Button JoinButton;
    public Button LeaveButton;

    public GameObject PlayerPrefab;

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

    public override void OnConnectedToMaster()
    {
        Debug.Log("<color=blue>Connected to Master</color>");

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
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("<color=blue>Joined Room</color>");
        HostButton.interactable = false;
        JoinButton.interactable = false;
        LeaveButton.interactable = true;

        //PhotonNetwork.Instantiate(this.PlayerPrefab.name, new Vector3(0f, 5f, -1f), Quaternion.identity, 0);
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

        PhotonNetwork.logLevel = Loglevel;
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings("1.0");
    }
}
