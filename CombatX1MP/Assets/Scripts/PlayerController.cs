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
    private bool isAlive;


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
                GameObject obj;

                if (!spriteR.flipX)
                {
                    //obj = Instantiate(bulletPrefab, bulletPointRight.position, Quaternion.identity);
                    //obj.gameObject.GetComponent<SpriteRenderer>().flipX = false;
                    //obj.gameObject.GetComponent<Rigidbody2D>().velocity = bulletPointRight.transform.right * velocShoot;
                    photonView.RPC("InstantiateShoot", RpcTarget.Others, bulletPrefab.name, true, false);
                }

                if (spriteR.flipX)
                {
                    //obj = Instantiate(bulletPrefab, bulletPointLeft.position, Quaternion.identity);
                    //obj.gameObject.GetComponent<SpriteRenderer>().flipX = true;
                    //obj.gameObject.GetComponent<Rigidbody2D>().velocity = bulletPointLeft.transform.right * velocShoot;
                    photonView.RPC("InstantiateShoot", RpcTarget.Others, bulletPrefab.name, false, true);
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
            GameObject obj = PhotonNetwork.Instantiate(Path.Combine("Prefabs", bulletPrefName), bulletPointRight.position, Quaternion.identity);
            obj.gameObject.GetComponent<SpriteRenderer>().flipX = isFlipX;
            obj.gameObject.GetComponent<Rigidbody2D>().velocity = bulletPointRight.transform.right * velocShoot;
        }
        else
        {
            GameObject obj = PhotonNetwork.Instantiate(Path.Combine("Prefabs", bulletPrefName), bulletPointLeft.position, Quaternion.identity);
            obj.gameObject.GetComponent<SpriteRenderer>().flipX = isFlipX;
            obj.gameObject.GetComponent<Rigidbody2D>().velocity = bulletPointLeft.transform.right * velocShoot;
        }
    }
    //[PunRPC]
    //private void InstantiateShoot(Transform bulletPoint)
    //{
    //    GameObject obj = PhotonNetwork.Instantiate(Path.Combine("Prefabs", bulletPrefab.name), bulletPoint.position, Quaternion.identity);
    //    //obj.gameObject.GetComponent<SpriteRenderer>().flipX = isFlipX;
    //    //obj.gameObject.GetComponent<Rigidbody2D>().velocity = bulletPoint.transform.right * velocShoot;
    //}


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
            isAlive = false;
            StartCoroutine(SpawnPlayer());
            spriteR.enabled = false;
            boxCollider.enabled = false;
            boxCollider2.enabled = false;
            canva.enabled = false;
        }
    }

    IEnumerator SpawnPlayer()
    {
        yield return new WaitForSeconds(2f);
        transform.position = spawnPoint.position;
        spriteR.enabled = true;
        boxCollider.enabled = true;
        boxCollider2.enabled = true;
        canva.enabled = true;
        isAlive = true;
    }


}
