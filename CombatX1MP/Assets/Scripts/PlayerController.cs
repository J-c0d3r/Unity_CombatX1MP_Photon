using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviourPunCallbacks
{
    private bool canShoot;
    private bool startedShoot;
    private bool isJumping;
    private bool isDoubleJumping;
    public bool isAlive;


    private float qtyShoot;
    private float timeCountReset;
    [SerializeField] private float timeCountMaxReset;
    [SerializeField] private float qtyMaxShoot;
    [SerializeField] private float velocShoot;
    [SerializeField] private float velocMove;
    [SerializeField] private float jumpForce;
    private float horizontal;

    private Vector2 otherClientPos;
    private Rigidbody2D rig;
    private Animator anim;
    private SpriteRenderer spriteR;
    public BoxCollider2D boxCollider;
    public BoxCollider2D boxCollider2;
    private Transform spawnPoint;
    [SerializeField] private Transform bulletPointRight;
    [SerializeField] private Transform bulletPointLeft;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Canvas canva;
    [SerializeField] private Text nicknameTxt;

    GameObject bulletInstatieted;
    PhotonView bulletPV;

    private void Awake()
    {
        if (photonView.IsMine)
        {
            nicknameTxt.text = PhotonNetwork.NickName;
        }
        else
        {
            nicknameTxt.text = photonView.Owner.NickName;
        }
    }

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        spriteR = GetComponentInChildren<SpriteRenderer>();

        isAlive = true;

        if (PhotonNetwork.LocalPlayer.ActorNumber == 1)
            spawnPoint = GameManager.instance.positionLeft;

        if (PhotonNetwork.LocalPlayer.ActorNumber == 2)
            spawnPoint = GameManager.instance.positionRight;

        if (!photonView.IsMine)
            Destroy(rig);
    }

    private void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            rig.velocity = new Vector2(horizontal * velocMove, rig.velocity.y);
        }
        else
        {
            //SmoothMovement();
        }
    }

    void Update()
    {
        if (photonView.IsMine)
        {
            if (isAlive)
            {
                Move();
                Shoot();
            }

            if (qtyShoot > qtyMaxShoot)
            {
                timeCountReset += Time.deltaTime;
                if (timeCountReset >= timeCountMaxReset)
                {
                    qtyShoot = 0;
                    timeCountReset = 0f;
                }
            }
        }
        else
        {
            //SmoothMovement();
        }
    }


    private void Move()
    {
        horizontal = Input.GetAxis("Horizontal");

        if (horizontal > 0)
        {
            canShoot = false;
            startedShoot = false;
            spriteR.flipX = false;
            photonView.RPC("SwitchSpriteFlip", RpcTarget.Others, false);

            if (!isJumping)
                anim.SetInteger("transition", 1);
        }
        else if (horizontal < 0)
        {
            canShoot = false;
            startedShoot = false;
            spriteR.flipX = true;
            photonView.RPC("SwitchSpriteFlip", RpcTarget.Others, true);

            if (!isJumping)
                anim.SetInteger("transition", 1);
        }
        else if (horizontal == 0 && !isJumping && !startedShoot)
        {
            anim.SetInteger("transition", 0);
        }


        if (Input.GetKeyDown(KeyCode.Space) && !isDoubleJumping)
        {
            if (isJumping)
                isDoubleJumping = true;

            rig.AddForce(Vector2.up * jumpForce, ForceMode2D.Force);
            anim.SetInteger("transition", 2);
            isJumping = true;
            canShoot = false;
        }
    }

    [PunRPC]
    private void SwitchSpriteFlip(bool flipX)
    {
        spriteR.flipX = flipX;
    }

    private void SmoothMovement()
    {
        //transform.position = Vector2.Lerp(transform.position, otherClientPos, Time.fixedDeltaTime);
        rig.position = Vector2.MoveTowards(rig.position, otherClientPos, Time.fixedDeltaTime);
        //rig.position = otherClientPos;
    }

    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.IsWriting)
    //    {
    //        //stream.SendNext(transform.position);
    //        stream.SendNext(rig.position);
    //        stream.SendNext(rig.velocity);
    //    }
    //    else if (stream.IsReading)
    //    {
    //        otherClientPos = (Vector2)stream.ReceiveNext();
    //        rig.velocity = (Vector2)stream.ReceiveNext();

    //        float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
    //        otherClientPos += (rig.velocity * lag);
    //        //rig.position += rig.velocity * lag;
    //    }
    //}



    private void Shoot()
    {
        if ((Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1)) && !isJumping && qtyShoot <= qtyMaxShoot)
        {
            StartCoroutine(ShootCo());

            if (canShoot)
            {


                if (!spriteR.flipX)
                {
                    //bulletInstatieted = Instantiate(bulletPrefab, bulletPointRight.position, Quaternion.identity);
                    //PhotonView bulletPV;
                    bulletPV = PhotonNetwork.Instantiate(Path.Combine("Prefabs", bulletPrefab.name), bulletPointRight.position, Quaternion.identity).GetComponent<PhotonView>();
                    //bulletPV.GetComponent<BulletController>().InstanceConfig(false, bulletPointRight.right);

                    //bulletPV.GetComponent<SpriteRenderer>().flipX = false;
                    //bulletPV.GetComponent<Rigidbody2D>().velocity = bulletPointRight.right * velocShoot;


                    //bulletInstatieted.gameObject.GetComponent<SpriteRenderer>().flipX = false;
                    //bulletInstatieted.gameObject.GetComponent<Rigidbody2D>().velocity = bulletPointRight.transform.right * velocShoot;
                    //photonView.RPC("InstantiateShoot", RpcTarget.All, bulletPrefab.name, true, false);
                    //bulletPV.RPC("InstanceConfig", RpcTarget.All, true, false);
                }

                if (spriteR.flipX)
                {
                    //bulletInstatieted = Instantiate(bulletPrefab, bulletPointLeft.position, Quaternion.identity);
                    //PhotonView bulletPV;
                    bulletPV = PhotonNetwork.Instantiate(Path.Combine("Prefabs", bulletPrefab.name), bulletPointLeft.position, Quaternion.identity).GetComponent<PhotonView>();
                    bulletPV.transform.rotation = Quaternion.Euler(0, -180f, 0);
                    //bulletPV.GetComponent<BulletController>().InstanceConfig(true, bulletPointLeft.right);

                    //bulletPV.GetComponent<SpriteRenderer>().flipX = true;
                    //bulletPV.GetComponent<Rigidbody2D>().velocity = bulletPointLeft.right * velocShoot;

                    //bulletInstatieted.gameObject.GetComponent<SpriteRenderer>().flipX = true;
                    //bulletInstatieted.gameObject.GetComponent<Rigidbody2D>().velocity = bulletPointLeft.transform.right * velocShoot;
                    //photonView.RPC("InstantiateShoot", RpcTarget.All, bulletPrefab.name, false, true);
                    //bulletPV.RPC("InstanceConfig", RpcTarget.All, false, true);
                }

                qtyShoot++;
            }

        }
    }

    IEnumerator ShootCo()
    {
        startedShoot = true;
        rig.velocity = Vector2.zero;
        anim.SetInteger("transition", 3);
        yield return new WaitForSeconds(0.12f);
        canShoot = true;
    }


    [PunRPC]
    private void InstantiateShoot(string bulletPrefName, bool isRight, bool isFlipX)
    {
        if (isRight)
        {
            //GameObject bulletInstatieted = PhotonNetwork.Instantiate(Path.Combine("Prefabs", bulletPrefName), bulletPointRight.position, Quaternion.identity);
            //GameObject bulletInstatieted = Instantiate(bulletPrefab, bulletPointRight.position, Quaternion.identity);
            //bulletInstatieted.gameObject.GetComponent<SpriteRenderer>().flipX = isFlipX;
            //bulletInstatieted.gameObject.GetComponent<Rigidbody2D>().velocity = bulletPointRight.transform.right * velocShoot;
            bulletPV.GetComponent<SpriteRenderer>().flipX = isFlipX;
            bulletPV.GetComponent<Rigidbody2D>().velocity = bulletPointRight.right * velocShoot;
        }
        else
        {
            //GameObject bulletInstatieted = PhotonNetwork.Instantiate(Path.Combine("Prefabs", bulletPrefName), bulletPointLeft.position, Quaternion.identity);
            //GameObject bulletInstatieted = Instantiate(bulletPrefab, bulletPointLeft.position, Quaternion.identity);
            //bulletInstatieted.gameObject.GetComponent<SpriteRenderer>().flipX = isFlipX;
            //bulletInstatieted.gameObject.GetComponent<Rigidbody2D>().velocity = bulletPointLeft.transform.right * velocShoot;
            bulletPV.GetComponent<SpriteRenderer>().flipX = isFlipX;
            bulletPV.GetComponent<Rigidbody2D>().velocity = bulletPointLeft.right * velocShoot;
        }
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
            isDoubleJumping = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Lava"))
        {
            Die();
        }
    }

    public void Die()
    {

        isAlive = false;
        //spriteR.enabled = false;
        //boxCollider.enabled = false;
        //boxCollider2.enabled = false;
        //canva.enabled = false;
        if (photonView.IsMine)
            rig.gravityScale = 0;
        RPC_Die();
        photonView.RPC("RPC_Die", RpcTarget.Others);
        StartCoroutine(ReSpawnPlayer());

    }

    [PunRPC]
    private void RPC_Die()
    {
        spriteR.enabled = false;
        boxCollider.enabled = false;
        boxCollider2.enabled = false;
        canva.enabled = false;
    }

    IEnumerator ReSpawnPlayer()
    {
        yield return new WaitForSeconds(2f);
        transform.position = spawnPoint.position;
        RPC_Respawn();
        if (photonView.IsMine)
            rig.gravityScale = 2.5f;
        photonView.RPC("RPC_Respawn", RpcTarget.Others);
        //spriteR.enabled = true;
        //boxCollider.enabled = true;
        //boxCollider2.enabled = true;
        //canva.enabled = true;
        isAlive = true;
    }

    [PunRPC]
    private void RPC_Respawn()
    {
        spriteR.enabled = true;
        boxCollider.enabled = true;
        boxCollider2.enabled = true;
        canva.enabled = true;
    }

}
