using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitingRoomController : MonoBehaviourPunCallbacks
{

    [SerializeField] private Text playersList;
    [SerializeField] private Button startGame;

    [PunRPC]
    public void UpdateList()
    {
        playersList.text = NetworkController.instance.GetPlayersList();
        startGame.interactable = NetworkController.instance.GetRoomOwner();
    }

}
