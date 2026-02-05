using UnityEngine;

public class FlyerEnemyScript : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float speed = 3f;
    private int direction = 1;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Vector2 nuevaPos = rb.position + Vector2.up * direction * speed * Time.fixedDeltaTime;
        rb.MovePosition(nuevaPos);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            direction = -direction;
        }
    }
}