using UnityEngine;
// Necesitamos esta línea para acceder a las luces 2D de URP
using UnityEngine.Rendering.Universal; 

public class DiscoLight : MonoBehaviour
{
    [Header("Movimiento (Barrido)")]
    public bool activarMovimiento = true;
    public float velocidadBarrido = 2f; // Qué tan rápido va y viene
    public float anguloMaximo = 30f;   // Cuánto se inclina hacia los lados

    [Header("Cambio de Color")]
    public bool activarColores = true;
    public float velocidadColor = 0.5f;
    // Una lista de colores vibrantes para ir cambiando
    public Color[] coloresDisco = new Color[] { Color.red, Color.blue, Color.green, Color.magenta, Color.cyan, Color.yellow };

    private Light2D miLuz2D;
    private Quaternion rotacionInicial;
    private float tiempoColor;

    void Start()
    {
        // Obtenemos el componente de luz 2D
        miLuz2D = GetComponent<Light2D>();
        // Guardamos la rotación inicial para saber cuál es el "centro"
        rotacionInicial = transform.rotation;
    }

    void Update()
    {
        // 1. Lógica de Movimiento (Balanceo tipo péndulo)
        if (activarMovimiento)
        {
            // Usamos Seno para crear un movimiento suave de ida y vuelta
            float oscilacion = Mathf.Sin(Time.time * velocidadBarrido);
            // Calculamos el ángulo actual
            float anguloActual = oscilacion * anguloMaximo;
            
            // Aplicamos la rotación sobre la inicial (eje Z en 2D)
            transform.rotation = rotacionInicial * Quaternion.Euler(0, 0, anguloActual);
        }

        // 2. Lógica de Cambio de Color Suave
        if (activarColores && miLuz2D != null && coloresDisco.Length > 0)
        {
            tiempoColor += Time.deltaTime * velocidadColor;
            
            // Calculamos qué par de colores toca mezclar ahora
            int indiceColor1 = (int)tiempoColor % coloresDisco.Length;
            int indiceColor2 = (indiceColor1 + 1) % coloresDisco.Length;
            float mezcla = tiempoColor - (int)tiempoColor;

            // Lerp mezcla suavemente entre el color 1 y el 2
            miLuz2D.color = Color.Lerp(coloresDisco[indiceColor1], coloresDisco[indiceColor2], mezcla);
        }
    }
}