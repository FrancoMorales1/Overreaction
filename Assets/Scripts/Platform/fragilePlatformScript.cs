using UnityEngine;
using System.Collections;

public class fragilePlatformScript : MonoBehaviour
{
    [Header("Times")]
    public float timeForBreak = 0.5f;
    public float respawnTime = 5.0f;

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
            StartCoroutine(BreakPlatform());
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
        DeactivatePlatform(true);
        yield return new WaitForSeconds(respawnTime);
        DeactivatePlatform(false);

        if (myAnimator != null)
        {
            myAnimator.Play("Idle"); //Representa el estado normal, o la animacion normal, habria que hacerla
            myAnimator.ResetTrigger("Respawn");
        }
        isBroken = false;
    }

    void DeactivatePlatform(bool state)
    {
        // Si estado es true (romper), desactivamos collider y sprite
        // Si estado es false (respawn), los activamos
        myCollider.enabled = !state;
        mySpriteRenderer.enabled = !state;
    }
}
