using UnityEngine;

public class RandomEnemy : MonoBehaviour
{
    [Header("Configuración")]
    public float speed = 4f;
    [Range(0, 45)]
    public float angleVariation = 15f; // Cuántos grados de caos sumamos al rebote

    private Vector2 moveDirection;
    private Rigidbody2D rb;

    private Vector2 lastPosition;
    private float stuckTimer;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        // Dirección inicial aleatoria
        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        moveDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * speed;

        // Detectar si está atorado
        if (Vector2.Distance(transform.position, lastPosition) < 0.01f)
        {
            stuckTimer += Time.fixedDeltaTime;
            if (stuckTimer > 0.5f) // Si lleva medio segundo sin moverse
            {
                // Forzar dirección aleatoria total para salir del bache
                float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
                moveDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                stuckTimer = 0;
            }
        }
        else
        {
            stuckTimer = 0;
        }
        lastPosition = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. Obtenemos la normal del punto de contacto (hacia dónde apunta la pared)
        Vector2 normal = collision.contacts[0].normal;

        // 2. Calculamos el rebote perfecto usando Vector2.Reflect
        Vector2 bounceDirection = Vector2.Reflect(moveDirection, normal);

        // 3. Añadimos la variación aleatoria en grados
        float randomOffset = Random.Range(-angleVariation, angleVariation);
        
        // Rotamos el vector de rebote un poco
        moveDirection = RotateVector(bounceDirection, randomOffset).normalized;

        transform.position += (Vector3)normal * 0.5f;
    }

    // Función auxiliar para rotar un vector
    Vector2 RotateVector(Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
        
        float tx = v.x;
        float ty = v.y;
        
        return new Vector2((cos * tx) - (sin * ty), (sin * tx) + (cos * ty));
    }

    void FlipSprite()
    {
        if (moveDirection.x > 0.1f)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (moveDirection.x < -0.1f)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }
}