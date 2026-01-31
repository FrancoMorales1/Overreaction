using UnityEngine;
using TMPro; // Necesario para usar TextMeshPro

public class UIManager : MonoBehaviour
{
    public TempData Data;           // Arrastra aquí tu archivo de ScriptableObject
    public TextMeshProUGUI timerText; // Arrastra aquí el objeto TimerText

    void Update()
    {
        if (Data != null && timerText != null)
        {
            // "F0" sirve para que no muestre decimales (ej: 59 en vez de 59.324)
            timerText.text = "Tiempo: " + Data.timeLeft.ToString("F0");

            // Opcional: Si queda poco tiempo, ponerlo en rojo
            if (Data.timeLeft < 10f)
            {
                timerText.color = Color.red;
            }
        }
    }
}