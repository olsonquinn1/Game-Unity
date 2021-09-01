using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement2 : MonoBehaviour
{
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private Rigidbody2D rb;

    public Animator anim;
    public float moveSpeed;

    public float moveX, moveY;
    private bool isMoving;

    private Vector3 moveDir;
    private void Update()
    {

        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");

        if(moveX != 0 || moveY != 0)
        {
            anim.SetFloat(name: "Horizontal",
                          value: moveX);
            anim.SetFloat(name: "Vertical",
                          value: moveY);
            if (!isMoving)
            {
                isMoving = true;
                anim.SetBool("IsMoving", isMoving);
            }
        }
        else
        {

            if (isMoving)
            {
                isMoving = false;
                anim.SetBool("IsMoving", isMoving);
                StopMoving();

            }

        }

        moveDir = new Vector3(moveX, moveY).normalized;

    }

    private void FixedUpdate()
    {
        rb.velocity = moveDir * moveSpeed * Time.deltaTime;
    }

    private void StopMoving()
    {
        rb.velocity = Vector3.zero;
    }

}
