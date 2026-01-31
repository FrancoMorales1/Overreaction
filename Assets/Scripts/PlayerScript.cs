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
    [Header("Efecto de Daño")]
    public float knockbackForce = 10f;
    public GMPlatformScript gameManager;

    public float invulnerabilityTime = 1.5f;
    private bool isInvulnerable = false;
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
            transform.localScale = new Vector3(-4f, 4f, 4f);
        }
        else if (Horizontal > 0.0f)
        {
            transform.localScale = new Vector3(4f, 4f, 4f);
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && !isInvulnerable)
        {
            // 1. Calculamos dirección contraria al golpe
            Vector2 damageDirection = (transform.position - collision.transform.position).normalized;
            
            // 2. Aplicamos fuerza de salto/retroceso
            Rigidbody2D.linearVelocity = Vector2.zero; // Limpiamos velocidad actual
            Rigidbody2D.AddForce(new Vector2(damageDirection.x, 1f) * knockbackForce, ForceMode2D.Impulse);
            
            // 3. Avisamos al GameManager
            gameManager.PlayerHit(damageDirection);

            StartCoroutine(BecomeInvulnerable());
        }
    }

    private IEnumerator BecomeInvulnerable()
    {
        isInvulnerable = true;
        
        // OPCIONAL: Efecto visual de parpadeo
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        float timer = 0;
        while (timer < invulnerabilityTime)
        {
            sprite.enabled = !sprite.enabled; // Parpadea
            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }
        
        sprite.enabled = true; // Aseguramos que quede visible
        isInvulnerable = false;
    }
}