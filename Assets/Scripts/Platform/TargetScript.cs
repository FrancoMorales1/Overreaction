using UnityEngine;

public class TargetScript : MonoBehaviour
{
    [Header("Referencias")]
    public GMPlatformScript gameManager; 

    [Header("Configuracion del Objeto")]
    public string itemName;
    public string itemCategory;

    [Header("Audio")]
    public AudioClip pickupSound;
    [Range(0f, 10f)] public float volume = 10f;

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
                    if (pickupSound != null)
                    {
                        Vector3 spawnPos = Camera.main.transform.position;
                        spawnPos.z = Camera.main.transform.position.z + 1f;

                        AudioSource.PlayClipAtPoint(pickupSound, spawnPos, volume);
                    }

                    gameObject.SetActive(false);
                }
            }
        }
    }
}