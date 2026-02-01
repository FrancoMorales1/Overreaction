using UnityEngine;

public class LinearEnemyScript : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float speed = 2f;
    public bool movingRight = true;

    [Header("Detección de Suelo")]
    public Transform groundCheck; // Un objeto vacío colocado al frente y abajo del enemigo
    public float distance = 0.5f; // Qué tan largo es el rayo
    public LayerMask groundLayer; // Selecciona la capa de tus plataformas

    void Update()
    {
        // 1. Mover al enemigo en la dirección actual
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        // 2. Lanzar un rayo hacia abajo desde la posición del groundCheck
        RaycastHit2D groundInfo = Physics2D.Raycast(groundCheck.position, Vector2.down, distance, groundLayer);

        // 3. Dibujar el rayo en el editor para que puedas verlo (opcional)
        Debug.DrawRay(groundCheck.position, Vector2.down * distance, Color.red);

        // 4. Si el rayo no toca nada (suelo), damos la vuelta
        if (groundInfo.collider == false)
        {
            TurnAround();
        }
    }

    void TurnAround()
    {
        if (movingRight)
        {
            transform.eulerAngles = new Vector3(0, -180, 0);
            movingRight = false;
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
            movingRight = true;
        }
    }
}