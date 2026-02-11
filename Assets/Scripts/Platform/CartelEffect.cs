using UnityEngine;

public class ParpadeoVerde : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public float velocidad = 3.0f;
    public float minAlpha = 0.4f; // Qué tan tenue se pone
    public float maxAlpha = 1.0f; // Qué tan brillante se pone

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Creamos una oscilación suave entre min y max
        float oscilacion = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(Time.time * velocidad) + 1.0f) / 2.0f);
        
        // Aplicamos el cambio al color manteniendo el verde original
        Color c = spriteRenderer.color;
        c.a = oscilacion;
        spriteRenderer.color = c;
    }
}