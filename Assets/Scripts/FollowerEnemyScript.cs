using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float speed = 1.5f;
    public float rotationSpeed = 5f; // Para que el giro no sea brusco

    private Transform player;

    void Start()
    {
        // Buscamos al Player por su Tag al iniciar
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void Update()
    {
        if (player != null)
        {
            FollowPlayer();
            FlipSprite();
        }
    }

    void FollowPlayer()
    {
        // Calculamos la dirección hacia el player
        Vector3 direction = (player.position - transform.position).normalized;

        // Movemos al enemigo hacia esa dirección
        transform.position += direction * speed * Time.deltaTime;
    }

    void FlipSprite()
    {
        // Si el player está a la derecha, escala positiva. Si está a la izquierda, negativa.
        if (player.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
}