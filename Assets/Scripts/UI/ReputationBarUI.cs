using UnityEngine;
using UnityEngine.UI; 
using System.Collections;

public class ReputationBarUI : MonoBehaviour
{
    public Slider sliderbar;
    public float smoothSpeed = 5.0f;

    void Start()
    {
        if (QuestFlowManager.Instance != null)
        {
            // Configuramos los límites
            sliderbar.maxValue = QuestFlowManager.Instance.maxReputation;
            sliderbar.minValue = 0;

            // 1. Ponemos la barra donde estaba ANTES de la misión
            sliderbar.value = QuestFlowManager.Instance.lastReputation;

            // 2. Iniciamos la animación hacia el valor NUEVO
            // (Aquí faltaba el punto y coma)
            StartCoroutine(BarAnimation(QuestFlowManager.Instance.currentReputation)); 
        }
        else
        {
            // Valor por defecto para pruebas
            sliderbar.maxValue = 100;
            sliderbar.value = 50;
        }
    }

    IEnumerator BarAnimation(float targetValue)
    {
        // CORRECCIÓN: Usamos > (mayor que) para decir "mientras estemos lejos del objetivo"
        while (Mathf.Abs(sliderbar.value - targetValue) > 0.01f)
        {
            sliderbar.value = Mathf.Lerp(sliderbar.value, targetValue, smoothSpeed * Time.deltaTime);
            yield return null; // Esperar al siguiente frame
        }

        // Aseguramos el valor exacto al final
        sliderbar.value = targetValue;
        
        // Actualizamos el 'lastReputation' para que la próxima vez no anime desde el pasado lejano
        if (QuestFlowManager.Instance != null)
        {
            QuestFlowManager.Instance.lastReputation = QuestFlowManager.Instance.currentReputation;
        }
    }
}