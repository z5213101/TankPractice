using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;
    public static GameObject localPlayer;
    string gameVersion = "1";

    private void Awake()
    {
        if(instance != null && instance.name != "GameManager")  //delete words after && if there is bug
        {
            Debug.LogErrorFormat(gameObject, 
                "Multiple instance of {0} is not allow", GetType().Name);
            DestroyImmediate(gameObject);
            return;
        }

        PhotonNetwork.AutomaticallySyncScene = true;
        DontDestroyOnLoad(gameObject);
        instance = this;
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneloaded;
        PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.GameVersion = gameVersion;
    }
    public override void OnConnected()
    {
        Debug.Log("PUN connected");
        PhotonNetwork.AutomaticallySyncScene = true;    //Not sure where to add
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("PUN connected to Master");
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarningFormat("PUN Disconnected was called by PUN with reason {0}", cause);
    }

    public void JoinGameRoom()
    {
        Debug.Log("Try joining game");
        var options = new RoomOptions
        {
            MaxPlayers = 6
        };
        PhotonNetwork.JoinOrCreateRoom("Kingdom", options, null);
    }
    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Created room");
            PhotonNetwork.LoadLevel("GameScene");
        }
        else
            Debug.Log("Joined room");        
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogWarningFormat("Joined room failed: {0}",message);
    }

    private void OnSceneloaded(Scene scene, LoadSceneMode mode)
    {
        if(!PhotonNetwork.InRoom)
        {
            return;
        }
        localPlayer = PhotonNetwork.Instantiate("TankPlayer", new Vector3(0, 0, 0), Quaternion.identity, 0);
        Debug.Log("Player Instance ID: " + localPlayer.GetInstanceID());
    }
}
