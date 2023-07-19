using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    //[SerializeField] private float veloc;

    //private Rigidbody2D rig;
    //private SpriteRenderer spriteR;

    void Start()
    {
        //rig = GetComponent<Rigidbody2D>();
        //spriteR = GetComponent<SpriteRenderer>();

        //rig.velocity = transform.right * velocMove;

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

        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(collision.gameObject);
        }
    }
}
