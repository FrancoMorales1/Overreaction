using UnityEngine;

public class TargetScript : MonoBehaviour
{
    [Header("Referencias")]
    public GMPlatformScript gameManager; 

    [Header("Configuracion del Objeto")]
    public string itemName;
    public string itemCategory;

    private void Start()
    {
        if (gameManager != null) gameManager.RegisterWorldItem(this);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameManager != null)
            {
                if(gameManager.AddItem(itemName, itemCategory)) 
                {
                    gameObject.SetActive(false);
                }
            }
        }
    }
}