using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class JohnMovement : MonoBehaviour
{
    [Header("Audio")]
    public AudioClip hurtSound;
    private AudioSource audioSource;

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

    private Animator Animator;
    void Start()
    {
        Animator = GetComponent<Animator>();
        Rigidbody2D = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Keyboard.current != null)
        {
            float left = Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed ? -1f : 0f;
            float right = Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed ? 1f : 0f;
            Horizontal = left + right;

            Animator.SetBool("running", Horizontal != 0.0f);
        }
        if (Horizontal < 0.0f)
        {
            transform.localScale = new Vector3(-0.3f, 0.3f, 0.3f);
        }
        else if (Horizontal > 0.0f)
        {
            transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        }

        Debug.DrawRay(transform.position, Vector2.down * 1f, Color.blue);      
        if (Physics2D.Raycast(transform.position, Vector3.down, 1f))
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
        Animator.SetFloat("yVelocity", Rigidbody2D.linearVelocity.y);
        Animator.SetBool("isGrounded", Grounded);
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
            if (audioSource != null && hurtSound != null)
            {
                audioSource.PlayOneShot(hurtSound);
            }

            // 1. Calculamos si el enemigo está a la izquierda o derecha
            // Si la resta da positivo, el enemigo está a la izquierda (saltamos a la derecha)
            // Si da negativo, el enemigo está a la derecha (saltamos a la izquierda)
            float side = transform.position.x - collision.transform.position.x;
            float directionX = Mathf.Sign(side); // Esto nos da 1 o -1

            // 2. Creamos un vector de 45 grados (1 en X, 1 en Y)
            // Lo normalizamos para que la fuerza total sea siempre constante
            Vector2 knockbackDir = new Vector2(directionX, 1f).normalized;
            
            // 3. Aplicamos el golpe
            Rigidbody2D.linearVelocity = Vector2.zero; // Frenamos cualquier movimiento previo
            Rigidbody2D.AddForce(knockbackDir * knockbackForce, ForceMode2D.Impulse);
            
            // 4. Avisamos al GameManager (enviando el vector por si lo necesitas)
            gameManager.PlayerHit(knockbackDir);

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