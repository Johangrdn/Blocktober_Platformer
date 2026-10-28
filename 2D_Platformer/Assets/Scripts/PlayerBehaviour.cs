using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehaviour : MonoBehaviour
{
    private float move;

    public float speed;
    public float jump;
    public bool isJumping;

    private Rigidbody2D rigidbody2D; 

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        move = Input.GetAxis("Horizontal");
        rigidbody2D.velocity = new Vector2(speed * move, rigidbody2D.velocity.y);

        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            rigidbody2D.AddForce(new Vector2(rigidbody2D.velocity.x, jump));
            isJumping = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D pCollision)
    {
        if (pCollision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
        }

        if (pCollision.gameObject.CompareTag("M_Platform") && isJumping == true)
        {
            this.transform.parent = pCollision.transform;
            isJumping = false;
        }
    }

    private void OnCollisionExit2D(Collision2D pCollision)
    {
        if (pCollision.gameObject.CompareTag("Ground"))
        {
            isJumping = true;
        }
    }
}
