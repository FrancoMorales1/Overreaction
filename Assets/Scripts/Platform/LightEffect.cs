using UnityEngine;
using UnityEngine.Rendering.Universal; // Necesario para acceder a las luces 2D

public class EfectoRespiracion : MonoBehaviour
{
    private Light2D luz;
    public float intensidadMin = 0.0f;
    public float intensidadMax = 3.0f;
    public float velocidad = 1.5f;

    void Start()
    {
        luz = GetComponent<Light2D>();
    }

    void Update()
    {
        // Mathf.PingPong crea un valor que sube y baja constantemente
        float tiempo = Time.time * velocidad;
        luz.intensity = Mathf.Lerp(intensidadMin, intensidadMax, (Mathf.Sin(tiempo) + 1.0f) / 2.0f);
    }
}