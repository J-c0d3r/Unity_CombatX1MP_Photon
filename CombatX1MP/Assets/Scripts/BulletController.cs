using Photon.Pun;
using Photon.Pun.Demo.Asteroids;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviourPunCallbacks
{
    public float velocShoot;

    private SpriteRenderer spriteR;
    private Rigidbody2D rig;

    void Start()
    {
        spriteR = GetComponent<SpriteRenderer>();
        rig = GetComponent<Rigidbody2D>();

        rig.velocity = transform.right * velocShoot;

        //Destroy(gameObject, 5f);
        StartCoroutine(DestroyCO(5f));
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            //Destroy(gameObject);
            //PhotonNetwork.Destroy(gameObject);
            Destroy();
        }

        PhotonView target = collision.gameObject.GetComponent<PhotonView>();

        if (target != null && target.gameObject.CompareTag("Player") && target.IsMine && target.gameObject.GetComponent<PlayerController>().isAlive)
        {
            //Debug.Log(collision.gameObject);
            //Destroy(collision.gameObject);
            //Debug.Log(photonView.OwnerActorNr);
            //Debug.Log(target.OwnerActorNr);
            GameManager.instance.RespawnPlayer(target.OwnerActorNr);
            PhotonNetwork.Destroy(target.gameObject);
            GameManager.instance.AddPoint(photonView.OwnerActorNr);
            photonView.RPC("Destroy", RpcTarget.All);
            //Destroy();
            //collision.GetComponent<PlayerController>().Die();            
            //Destroy(gameObject);

        }
    }

    IEnumerator DestroyCO(float time)
    {
        yield return new WaitForSeconds(time);
        Destroy();
        //photonView.RPC("Destroy", RpcTarget.All);
    }

    [PunRPC]
    public void Destroy()
    {
        if (photonView.IsMine)
            PhotonNetwork.Destroy(gameObject);
    }

    //[PunRPC]
    //public void InstanceConfig(bool isFlip, bool isRight)
    //{
    //    spriteR.flipX = isFlip;

    //    if (isRight)
    //    {
    //        bulletPV.GetComponent<Rigidbody2D>().velocity = bulletPointRight.right * velocShoot;
    //    }
    //    else
    //    {
    //        bulletPV.GetComponent<Rigidbody2D>().velocity = bulletPointLeft.right * velocShoot;
    //    }
    //    bulletPV.GetComponent<Rigidbody2D>().velocity = bulletPointLeft.right * velocShoot;
    //    rig.velocity = direction * velocShoot;
    //}

}
