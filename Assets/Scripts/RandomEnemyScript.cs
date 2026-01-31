using UnityEngine;

public class RandomEnemy : MonoBehaviour
{
    [Header("Configuración")]
    public float speed = 3f;
    
    private Vector2 moveDirection;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Iniciamos con una dirección aleatoria
        SetRandomDirection();
    }

    void FixedUpdate()
    {
        // Movemos al enemigo en la dirección actual
        rb.linearVelocity = moveDirection * speed;
        
        // Rotar el sprite según la dirección (opcional)
        FlipSprite();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Al chocar con CUALQUIER cosa, cambiamos de dirección
        SetRandomDirection();
        
        Debug.Log("Choqué con: " + collision.gameObject.name + ". Cambiando rumbo...");
    }

    void SetRandomDirection()
    {
        // Generamos un ángulo aleatorio entre 0 y 360 grados
        float angle = Random.Range(0f, 360f);
        
        // Convertimos el ángulo a un vector de dirección (X, Y)
        moveDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
    }

    void FlipSprite()
    {
        if (moveDirection.x > 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (moveDirection.x < 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
}