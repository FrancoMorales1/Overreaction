using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Referencias")]
    public Transform target; // Arrastra a John aquí

    [Header("Configuración")]
    public float smoothing = 5f; // Qué tan "elástica" es la cámara
    public Vector3 offset = new Vector3(0, 0, -10); // Desplazamiento (Z debe ser -10)

    void FixedUpdate()
    {
        if (target != null)
        {
            // Calculamos la posición deseada (X e Y del player + el offset)
            Vector3 targetPosition = target.position + offset;

            // Movemos la cámara suavemente de su posición actual a la del objetivo
            // Usamos Lerp para que no sea un movimiento brusco y robótico
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing * Time.fixedDeltaTime);
        }
    }
}