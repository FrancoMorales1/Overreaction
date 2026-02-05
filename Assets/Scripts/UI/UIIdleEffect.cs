using UnityEngine;

public class UIIdleEffect : MonoBehaviour
{
    [Header("Movimiento (Posición)")]
    public bool enablePosition = true;
    public float moveSpeed = 2f;      // Qué tan rápido se mueve
    public float moveAmountX = 5f;    // Cuántos píxeles se mueve a los lados
    public float moveAmountY = 5f;    // Cuántos píxeles sube y baja

    [Header("Respiración (Escala)")]
    public bool enableBreathing = false;
    public float breathSpeed = 1f;
    public float breathAmount = 0.02f; // Cuánto se estira (muy poquito es mejor)

    private Vector3 startPosition;
    private Vector3 startScale;

    void Start()
    {
        // Guardamos la posición y escala iniciales como referencia
        startPosition = transform.localPosition;
        startScale = transform.localScale;
    }

    void Update()
    {
        // 1. Lógica de Movimiento (Flotar)
        if (enablePosition)
        {
            // Usamos Seno y Coseno con tiempos distintos para que el movimiento no sea un círculo perfecto aburrido
            float newX = startPosition.x + Mathf.Sin(Time.time * moveSpeed) * moveAmountX;
            float newY = startPosition.y + Mathf.Cos(Time.time * moveSpeed * 0.8f) * moveAmountY;

            transform.localPosition = new Vector3(newX, newY, startPosition.z);
        }

        // 2. Lógica de Respiración (Escalar suavemente)
        if (enableBreathing)
        {
            float scaleOffset = Mathf.Sin(Time.time * breathSpeed) * breathAmount;
            transform.localScale = startScale + new Vector3(scaleOffset, scaleOffset, 0);
        }
    }
}
