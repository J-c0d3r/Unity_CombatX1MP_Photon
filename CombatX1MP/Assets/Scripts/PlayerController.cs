using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int id;
    
    private bool isJumping;
    private bool isDoubleJumping;


    [SerializeField] private float veloc;
    [SerializeField] private float jumpForce;
    private float horizontal;

    private Rigidbody2D rig;
    private Animator anim;
    private SpriteRenderer spriteR;
    private Transform bulletPoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Canvas canva;


    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        spriteR = GetComponentInChildren<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        rig.velocity = new Vector2(horizontal * veloc, rig.velocity.y);
    }

    void Update()
    {
        Move();
        Shoot();
    }


    private void Move()
    {
        horizontal = Input.GetAxis("Horizontal");


        if (horizontal > 0)
        {
            //spriteR.flipX = false;
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
            canva.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            if (!isJumping)
                anim.SetInteger("transition", 1);
        }
        else if (horizontal < 0)
        {
            //spriteR.flipX = true;
            transform.rotation = Quaternion.Euler(0f, 180, 0f);
            canva.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

            if (!isJumping)
                anim.SetInteger("transition", 1);
        }
        else if (horizontal == 0 && !isJumping)
        {
            anim.SetInteger("transition", 0);
        }


        if (Input.GetKeyDown(KeyCode.W) && !isDoubleJumping)
        {
            if (isJumping)
                isDoubleJumping = true;

            rig.AddForce(Vector2.up * jumpForce, ForceMode2D.Force);
            anim.SetInteger("transition", 2);
            isJumping = true;
        }

    }

    private void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Instantiate(bulletPrefab, bulletPoint.position, Quaternion.identity);
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


}
