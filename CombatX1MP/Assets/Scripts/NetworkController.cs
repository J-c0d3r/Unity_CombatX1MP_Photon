using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkController : MonoBehaviourPunCallbacks
{
    public static NetworkController instance;


    private void Awake()
    {
        if (instance == null || instance == this)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
        Destroy(gameObject);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Disconected!");
    }

    public string GetPlayersList()
    {
        var list = "";
        foreach (var player in PhotonNetwork.PlayerList)
        {
            list += player.NickName + "\n";
        }
        return list;
    }

    public bool GetRoomOwner()
    {
        return PhotonNetwork.IsMasterClient;
    }


    [PunRPC]
    public void StartGameMP()
    {
        PhotonNetwork.LoadLevel(1);
    }


}
