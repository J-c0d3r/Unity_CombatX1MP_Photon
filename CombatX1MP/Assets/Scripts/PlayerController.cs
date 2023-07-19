using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
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

    private Rigidbody2D rig;
    private Animator anim;
    private SpriteRenderer spriteR;
    public BoxCollider2D boxCollider;
    public BoxCollider2D boxCollider2;
    [SerializeField] public Transform spawnPoint;
    [SerializeField] private Transform bulletPointRight;
    [SerializeField] private Transform bulletPointLeft;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Canvas canva;


    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        spriteR = GetComponentInChildren<SpriteRenderer>();

        isAlive = true;
    }

    private void FixedUpdate()
    {
        rig.velocity = new Vector2(horizontal * velocMove, rig.velocity.y);
    }

    void Update()
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


    private void Move()
    {
        horizontal = Input.GetAxis("Horizontal");


        if (horizontal > 0)
        {
            canShoot = false;
            startedShoot = false;
            spriteR.flipX = false;

            if (!isJumping)
                anim.SetInteger("transition", 1);
        }
        else if (horizontal < 0)
        {
            canShoot = false;
            startedShoot = false;
            spriteR.flipX = true;

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
                    obj = Instantiate(bulletPrefab, bulletPointRight.position, Quaternion.identity);
                    obj.gameObject.GetComponent<SpriteRenderer>().flipX = false;
                    obj.gameObject.GetComponent<Rigidbody2D>().velocity = bulletPointRight.transform.right * velocShoot;
                }

                if (spriteR.flipX)
                {
                    obj = Instantiate(bulletPrefab, bulletPointLeft.position, Quaternion.identity);
                    obj.gameObject.GetComponent<SpriteRenderer>().flipX = true;
                    obj.gameObject.GetComponent<Rigidbody2D>().velocity = bulletPointLeft.transform.right * velocShoot;
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
