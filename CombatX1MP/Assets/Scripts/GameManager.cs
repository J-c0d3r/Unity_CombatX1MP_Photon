using Photon.Pun;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager instance;

    public GameObject player;
    public Transform positionLeft;
    public Transform positionRight;

    public Text nicknameP1;
    public Text nicknameP2;

    [Header("UI Points")]
    public Text scoreP1;
    public Text scoreP2;
    private int pointsCountP1;
    private int pointsCountP2;

    [Header("Timer")]
    public Text timeUI;
    [SerializeField] private float maxTimeMatch;
    private float timeCount;
    private bool canStart;

    [Header("UI GameOver")]
    public Text winnerTxt;
    public GameObject panelGameOver;

    [Header("UI PlayAgain")]
    public Text timePlayAgainTxt;
    public float maxTimePlayAgain;
    private float timeCountPlayAgain;
    private bool willP1PlayAgain;
    private bool willP2PlayAgain;
    private bool canStartCountPlayAgain;


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
            //return;
        }
        //Destroy(gameObject);        
    }

    private void Start()
    {
        Time.timeScale = 1.0f;
        panelGameOver.SetActive(false);
        timeCount = maxTimeMatch;
        timeCountPlayAgain = maxTimePlayAgain;
        SpawnPlayer();
        canStart = true;
    }

    private void Update()
    {
        if (canStart && PhotonNetwork.IsMasterClient)
        {
            timeCount -= Time.deltaTime;
            timeUI.text = timeCount.ToString("00");
            photonView.RPC("RPC_UpdateTime", RpcTarget.OthersBuffered, timeCount);

            if (timeCount <= 0)
            {
                photonView.RPC("GameOver", RpcTarget.All);
            }
        }

        //if (canStartCountPlayAgain)
        //{
        //    timeCountPlayAgain -= PhotonNetwork.ServerTimestamp;
        //    timePlayAgainTxt.text = timeCountPlayAgain.ToString("00");
        //    photonView.RPC("RPC_UpdateTimePlayAgain", RpcTarget.OthersBuffered, timeCountPlayAgain);

        //    if (timeCountPlayAgain <= 0)
        //    {
        //        photonView.RPC("LeaveMatch", RpcTarget.All);
        //    }
        //}
    }

    [PunRPC]
    public void RPC_UpdateTime(float time)
    {
        timeUI.text = time.ToString("00");
    }

    [PunRPC]
    public void RPC_UpdateTimePlayAgain(float time)
    {
        timePlayAgainTxt.text = time.ToString("00");
    }

    [PunRPC]
    public void GameOver()
    {
        canStart = false;
        //Time.timeScale = 0f;

        PlayerController[] playerList = FindObjectsOfType<PlayerController>();

        foreach (var player in playerList)
        {
            player.isAlive = false;
        }

        panelGameOver.SetActive(true);

        if (pointsCountP1 > pointsCountP2)
        {
            winnerTxt.text = "Player " + nicknameP1.text + " was winner!";
        }
        else if (pointsCountP2 > pointsCountP1)
        {
            winnerTxt.text = "Player " + nicknameP2.text + " was winner!";
        }
        else
        {
            winnerTxt.text = "Didn't have winners. Both are very powerful!";
        }
    }

    public void PlayAgainBtn()
    {
        photonView.RPC("PlayAgain", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, true);
    }

    [PunRPC]
    public void PlayAgain(int num, bool confirmation)
    {
        //timePlayAgainTxt.gameObject.SetActive(true);
        //photonView.RPC("CanStartCountPlayAgain", RpcTarget.MasterClient);        

        if (num == 1)
            willP1PlayAgain = confirmation;


        if (num == 2)
            willP2PlayAgain = confirmation;


        if (PhotonNetwork.IsMasterClient && willP1PlayAgain && willP2PlayAgain)
        {
            NetworkController.instance.photonView.RPC("StartGameMP", RpcTarget.All);
        }
    }

    //[PunRPC]
    //public void CanStartCountPlayAgain()
    //{
    //    canStartCountPlayAgain = true;
    //}

    public void ExitToMenuBtn()
    {
        if (!PhotonNetwork.OfflineMode)
            photonView.RPC("LeaveMatch", RpcTarget.All);
        else
            PhotonNetwork.LoadLevel(0);
    }

    [PunRPC]
    public void LeaveMatch()
    {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LeaveLobby();
        PhotonNetwork.LoadLevel(0);
    }


    public void SpawnPlayer()
    {
        switch (PhotonNetwork.LocalPlayer.ActorNumber)
        {
            case 1:
                PhotonNetwork.Instantiate(Path.Combine("Prefabs", player.name), positionLeft.position, Quaternion.identity);
                nicknameP1.text = PhotonNetwork.PlayerList[0].NickName;
                if (PhotonNetwork.PlayerList.Length > 1)
                    nicknameP2.text = PhotonNetwork.PlayerList[1].NickName;
                break;
            case 2:
                PhotonNetwork.Instantiate(Path.Combine("Prefabs", player.name), positionRight.position, Quaternion.identity);
                nicknameP1.text = PhotonNetwork.PlayerList[0].NickName;
                if (PhotonNetwork.PlayerList.Length > 1)
                    nicknameP2.text = PhotonNetwork.PlayerList[1].NickName;
                break;
            default:
                break;
        }

        if (PhotonNetwork.OfflineMode && PhotonNetwork.LocalPlayer.ActorNumber == -1)
            PhotonNetwork.Instantiate(Path.Combine("Prefabs", player.name), positionLeft.position, Quaternion.identity);
    }

    public void RespawnPlayer(int num)
    {
        StartCoroutine(ReSpawnPlayer(num));
    }

    IEnumerator ReSpawnPlayer(int num)
    {
        yield return new WaitForSeconds(2f);
        StopAllCoroutines();
        if (canStart)
        {
            if (num == 1)
                PhotonNetwork.Instantiate(Path.Combine("Prefabs", player.name), positionLeft.position, Quaternion.identity);

            if (num == 2)
                PhotonNetwork.Instantiate(Path.Combine("Prefabs", player.name), positionRight.position, Quaternion.identity);
        }
    }


    public void AddPoint(int actor)
    {
        if (actor == 1)
            pointsCountP1++;
        else if (actor == 2)
            pointsCountP2++;

        scoreP1.text = pointsCountP1.ToString();
        scoreP2.text = pointsCountP2.ToString();
        photonView.RPC("RPC_UpdatePoints", RpcTarget.OthersBuffered, pointsCountP1, pointsCountP2);
    }

    [PunRPC]
    public void RPC_UpdatePoints(int p1, int p2)
    {
        pointsCountP1 = p1;
        pointsCountP2 = p2;

        scoreP1.text = pointsCountP1.ToString();
        scoreP2.text = pointsCountP2.ToString();
    }

}
