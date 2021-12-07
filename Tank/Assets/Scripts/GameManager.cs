using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;
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
}
