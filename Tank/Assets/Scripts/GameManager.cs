using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using Tanks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;
    public static GameObject localPlayer;
    string gameVersion = "1";

    private GameObject defaultSpawnPoint;

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

        defaultSpawnPoint = new GameObject("Default SpawnPoint");
        defaultSpawnPoint.transform.position = new Vector3(0, 0, 0);
        defaultSpawnPoint.transform.SetParent(transform, false);
    }
    #region Connection
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

        var spawnPoint = GetRandomSpawnPoint();
        localPlayer = PhotonNetwork.Instantiate(
            "TankPlayer",
            spawnPoint.position,
            spawnPoint.rotation,
            0);
        Debug.Log("Player Instance ID: " + localPlayer.GetInstanceID());
    }
    #endregion
    public static List<GameObject> GetAllObjectsOfTypeInScene<T>()
    {
        var objsInScene = new List<GameObject>();
        foreach(var go in (GameObject[])Resources.FindObjectsOfTypeAll(typeof(GameObject)))
        {
            if (go.hideFlags == HideFlags.NotEditable ||
                go.hideFlags == HideFlags.HideAndDontSave)
                continue;
            if (go.GetComponent<T>() != null)
                objsInScene.Add(go);
        }
        return objsInScene;
    }
    private Transform GetRandomSpawnPoint()
    {
        var SPs = GetAllObjectsOfTypeInScene<SpawnPoint>();
        return SPs.Count == 0
            ? defaultSpawnPoint.transform
            : SPs[Random.Range(0, SPs.Count)].transform;
    }
}
