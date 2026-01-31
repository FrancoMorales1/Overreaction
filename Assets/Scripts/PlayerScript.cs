using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JohnMovement : MonoBehaviour
{
    private Rigidbody2D Rigidbody2D;
    private float Horizontal;

    private bool Grounded;

    public float JumpForce = 5f;
    public float Speed = 2f;
    void Start()
    {
     Rigidbody2D = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (Keyboard.current != null)
        {
            float left = Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed ? -1f : 0f;
            float right = Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed ? 1f : 0f;
            Horizontal = left + right;
        }
        if (Horizontal < 0.0f)
        {
            transform.localScale = new Vector3(-5f, 5f, 5f);
        }
        else if (Horizontal > 0.0f)
        {
            transform.localScale = new Vector3(5f, 5f, 5f);
        }

        Debug.DrawRay(transform.position, Vector2.down * 0.6f, Color.blue);      
        if (Physics2D.Raycast(transform.position, Vector3.down, 0.6f))
        {
            Grounded = true;
        }
        else
        {
            Grounded = false;
        }

        if (Keyboard.current.wKey.wasPressedThisFrame && Grounded)
        {
            Jump();
        }
    }

    private void Jump()
    {
        Rigidbody2D.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
    }

    private void FixedUpdate()
    {
        Rigidbody2D.linearVelocity = new Vector2(Horizontal * Speed, Rigidbody2D.linearVelocity.y);
    }

}