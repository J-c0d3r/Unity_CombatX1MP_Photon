using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{

    [SerializeField] private GameObject chooseModePnl;
    [SerializeField] private GameObject createJoinRoomPnl;
    [SerializeField] private GameObject roomPnl;


    void Start()
    {
        chooseModePnl.SetActive(false);
        createJoinRoomPnl.SetActive(false);
        roomPnl.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            chooseModePnl.SetActive(false);
            createJoinRoomPnl.SetActive(false);
            roomPnl.SetActive(false);
        }
    }


    public void StartGameSP()
    {
        SceneManager.LoadScene(1);
    }



    public void OpenWindow(GameObject gameO)
    {
        chooseModePnl.SetActive(false);
        createJoinRoomPnl.SetActive(false);
        roomPnl.SetActive(false);

        gameO.SetActive(true);
    }

}
