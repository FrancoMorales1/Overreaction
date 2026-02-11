using UnityEngine;

public class CrowdDancer : MonoBehaviour
{
    [Header("Configuración de Animación")]
    public string animationStateName = "Dance"; 
    
    [Header("Control de Velocidad ")]
    [Tooltip("1.0 es velocidad normal. 0.3 es cámara lenta. Pon valores bajos si tienes pocos frames.")]
    [Range(0.1f, 2.0f)]
    public float velocidadBase = 0.3f; // <--- AQUÍ ESTÁ EL TRUCO (Por defecto al 30%)

    [Header("Variación (Para que no parezcan robots)")]
    public bool randomizeStart = true;   
    public bool randomizeSpeed = true;   // Añade una pequeña variación a la velocidad base
    public bool randomizeMirror = true; 

    private Animator anim;
    private SpriteRenderer sr;

    void Start()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        ConfigurarBailarin();
    }

    void ConfigurarBailarin()
    {
        if (anim == null) return;

        // 1. Espejado Aleatorio
        if (randomizeMirror && sr != null)
        {
            sr.flipX = (Random.value > 0.5f);
        }

        // 2. Velocidad Controlada
        // Tomamos tu velocidad base (ej: 0.3) y le damos un toquecito de azar (+- 10%)
        float velocidadFinal = velocidadBase;
        
        if (randomizeSpeed)
        {
            // Variamos entre el 90% y el 110% de la velocidad base que elegiste
            velocidadFinal *= Random.Range(0.9f, 1.1f);
        }

        anim.speed = velocidadFinal;

        // 3. Inicio Desfasado
        if (randomizeStart)
        {
            anim.Play(animationStateName, 0, Random.value);
        }
    }
    
    // Un pequeño truco para ver los cambios en tiempo real mientras editas el juego
    void OnValidate()
    {
        if (Application.isPlaying && anim != null)
        {
            anim.speed = velocidadBase;
        }
    }
}