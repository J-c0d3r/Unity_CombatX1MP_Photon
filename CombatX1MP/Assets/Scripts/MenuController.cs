using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviourPunCallbacks
{

    [SerializeField] private GameObject chooseModePnl;
    [SerializeField] private GameObject connectedTxt;
    [SerializeField] private GameObject multiplayerBtn;
    [SerializeField] private GameObject createJoinRoomPnl;
    [SerializeField] private GameObject roomPnl;

    [SerializeField] private Text nickName;
    [SerializeField] private Text roomsName;


    void Start()
    {
        chooseModePnl.SetActive(false);
        connectedTxt.SetActive(false);
        createJoinRoomPnl.SetActive(false);
        roomPnl.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PhotonNetwork.IsConnected)
                PhotonNetwork.Disconnect();

            chooseModePnl.SetActive(false);
            createJoinRoomPnl.SetActive(false);
            roomPnl.SetActive(false);
            multiplayerBtn.GetComponent<Button>().interactable = false;
        }
    }


    public void StartGameSP()
    {
        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();

        StartCoroutine(changeConnection());

        //PhotonNetwork.OfflineMode = true;
        //PhotonNetwork.CreateRoom("SinglePlayer");
        //SceneManager.LoadScene(1);
        //PhotonNetwork.LoadLevel(1);
    }

    IEnumerator changeConnection()
    {
        while (PhotonNetwork.IsConnected)
        {
            yield return null;
        }

        PhotonNetwork.OfflineMode = true;
        PhotonNetwork.CreateRoom("SinglePlayer");
        PhotonNetwork.LoadLevel(1);
    }


    public void StartGameMP()
    {
        NetworkController.instance.photonView.RPC("StartGameMP", RpcTarget.All);
    }


    public void OpenWindow(GameObject gameO)
    {
        chooseModePnl.SetActive(false);
        connectedTxt.SetActive(false);
        createJoinRoomPnl.SetActive(false);
        roomPnl.SetActive(false);

        gameO.SetActive(true);
    }


    public void OpenChooseModePnl()
    {
        chooseModePnl.SetActive(true);
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        connectedTxt.SetActive(true);
        multiplayerBtn.GetComponent<Button>().interactable = true;
    }

    public void CreateRoomBtn()
    {
        PhotonNetwork.NickName = nickName.text;
        PhotonNetwork.CreateRoom(roomsName.text, new RoomOptions { MaxPlayers = 4 }, null);
    }

    public void JoinRoomBtn()
    {
        PhotonNetwork.NickName = nickName.text;
        PhotonNetwork.JoinRoom(roomsName.text, null);
    }

    public override void OnJoinedRoom()
    {
        OpenWindow(roomPnl);
        roomPnl.GetComponent<PhotonView>().RPC("UpdateList", RpcTarget.All);
        //roomPnl.GetComponent<WaitingRoomController>().UpdateList();
    }

    public void LeaveRoomBtn()
    {
        PhotonNetwork.LeaveRoom();

        chooseModePnl.SetActive(false);
        createJoinRoomPnl.SetActive(false);
        roomPnl.SetActive(false);
        multiplayerBtn.GetComponent<Button>().interactable = false;
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        roomPnl.GetComponent<PhotonView>().RPC("UpdateList", RpcTarget.All);
    }

}
