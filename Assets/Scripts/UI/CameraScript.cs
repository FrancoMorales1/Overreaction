using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Referencias")]
    public Transform target; // Arrastra a John aquí

    [Header("Configuración")]
    public float smoothing = 5f; 
    public Vector3 offset = new Vector3(0, 0, -10); 
    
    private float fixedX; // Guardaremos la X inicial de la cámara

    void Start()
    {
        // Al empezar, guardamos la posición X actual de la cámara
        // Esto permite que ubiques la cámara donde quieras en el editor y se quede ahí.
        fixedX = transform.position.x;
    }

    void FixedUpdate()
    {
        if (target != null)
        {
            // Creamos la posición deseada:
            // X: Usamos la posición fija que guardamos al inicio.
            // Y: Seguimos la Y del jugador + el desplazamiento vertical del offset.
            // Z: Mantenemos la Z de la cámara (offset.z).
            Vector3 targetPosition = new Vector3(fixedX, target.position.y + offset.y, offset.z);

            // Lerp para suavizado (Smoothing)
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothing * Time.fixedDeltaTime);
        }
    }
}