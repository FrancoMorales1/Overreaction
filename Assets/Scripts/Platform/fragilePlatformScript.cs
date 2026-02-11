using UnityEngine;
using System.Collections;

public class fragilePlatformScript : MonoBehaviour
{
    [Header("Times")]
    public float timeForBreak = 0.5f;
    public float respawnTime = 5.0f;
    public float fadeInDuration = 0.8f; // Cuánto tarda en aparecer

    [Header("References")]
    private BoxCollider2D myCollider;
    private SpriteRenderer mySpriteRenderer;
    private Animator myAnimator;

    private bool isBroken = false;
    
    void Awake()
    {
        myCollider = GetComponent<BoxCollider2D>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        myAnimator = GetComponent<Animator>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.y < -0.5f && !isBroken)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                StartCoroutine(BreakPlatform());
            }
        }
    }

    IEnumerator BreakPlatform()
    {
        isBroken = true;
        
        if (myAnimator != null)
        {
            myAnimator.SetTrigger("Break");
        }

        yield return new WaitForSeconds(timeForBreak);
        
        myCollider.enabled = false;
        
        // Esperamos un pelín más para que se vea el final de la rotura antes de ocultar
        yield return new WaitForSeconds(0.2f); 
        mySpriteRenderer.enabled = false;

        // Tiempo que permanece rota
        yield return new WaitForSeconds(respawnTime);
        
        // Iniciar el proceso de reaparición con Fade In
        StartCoroutine(FadeInAndReset());
    }

    IEnumerator FadeInAndReset()
    {
        // 1. Preparamos el sprite (totalmente transparente)
        Color c = mySpriteRenderer.color;
        c.a = 0;
        mySpriteRenderer.color = c;
        mySpriteRenderer.enabled = true;

        // 2. Volvemos al Idle para que no se vea rota mientras aparece
        if (myAnimator != null)
        {
            myAnimator.Play("Idle", 0, 0f);
            myAnimator.ResetTrigger("Break");
        }

        // 3. Fade In (Subir el Alpha gradualmente)
        float timer = 0;
        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;
            c.a = Mathf.Lerp(0, 1, timer / fadeInDuration);
            mySpriteRenderer.color = c;
            yield return null;
        }

        // Aseguramos que quede opaca al 100%
        c.a = 1;
        mySpriteRenderer.color = c;

        // 4. Activamos el collider para que el player pueda pisarla de nuevo
        myCollider.enabled = true;
        isBroken = false;
    }
}