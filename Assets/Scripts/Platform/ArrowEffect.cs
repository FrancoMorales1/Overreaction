using UnityEngine;

public class FlechaAnimada : MonoBehaviour
{
    [Header("Configuración de Brillo")]
    public float velBrillo = 3.0f;
    public float minAlpha = 0.3f;
    public float maxAlpha = 1.0f;

    [Header("Configuración de Movimiento")]
    public float velMovimiento = 5.0f;
    public float amplitud = 0.2f; // Qué tanto se desplaza hacia los lados

    private Vector3 posicionInicial;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        posicionInicial = transform.localPosition;
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // 1. EFECTO DE PARPADEO (Alpha)
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(Time.time * velBrillo) + 1.0f) / 2.0f);
        Color c = spriteRenderer.color;
        c.a = alpha;
        spriteRenderer.color = c;

        // 2. EFECTO DE MOVIMIENTO HORIZONTAL
        // Calculamos el desplazamiento en X
        float desplazamientoX = Mathf.Sin(Time.time * velMovimiento) * amplitud;
        transform.localPosition = posicionInicial + new Vector3(desplazamientoX, 0, 0);
    }
}