using Photon.Pun;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject player;
    public Transform positionLeft;
    public Transform positionRight;

    public Text nicknameP1;
    public Text nicknameP2;

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

    private void Start()
    {
        SpawnPlayer();
    }

    public void SpawnPlayer()
    {
        switch (PhotonNetwork.LocalPlayer.ActorNumber)
        {
            case 1:
                PhotonNetwork.Instantiate(Path.Combine("Prefabs", player.name), positionLeft.position, Quaternion.identity);
                nicknameP1.text = PhotonNetwork.PlayerList[0].NickName;
                nicknameP2.text = PhotonNetwork.PlayerList[1].NickName;
                break;
            case 2:
                PhotonNetwork.Instantiate(Path.Combine("Prefabs", player.name), positionRight.position, Quaternion.identity);
                nicknameP1.text = PhotonNetwork.PlayerList[0].NickName;
                nicknameP2.text = PhotonNetwork.PlayerList[1].NickName;
                break;
            default:
                break;
        }

        if (PhotonNetwork.OfflineMode && PhotonNetwork.LocalPlayer.ActorNumber == -1)
            PhotonNetwork.Instantiate(Path.Combine("Prefabs", player.name), positionLeft.position, Quaternion.identity);
    }

}
