using UnityEngine;

public class FinishLineScript : MonoBehaviour
{
    public GMPlatformScript gameManager;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.SetInFinishZone(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.SetInFinishZone(false);
        }
    }
}