using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{

    public int idPlayer;


    void Start()
    {
        Destroy(gameObject, 5f);
    }


    void Update()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Player") && collision.gameObject.GetComponent<PlayerController>().id != idPlayer)
        {
            Destroy(collision.gameObject);
        }
    }
}
