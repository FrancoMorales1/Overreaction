using UnityEngine;

public class TargetScript : MonoBehaviour
{
    [Header("Referencias")]
    public GMPlatformScript gameManager; 

    [Header("Configuración del Objeto")]
    public string itemName;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameManager != null)
            {
                gameManager.AddItem(itemName);
                Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("¡Te olvidaste de asignar el GameManager en el objeto " + gameObject.name + "!");
            }
        }
    }
}