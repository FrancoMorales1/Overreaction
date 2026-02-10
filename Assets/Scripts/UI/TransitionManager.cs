using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;

    [Header("UI References")]
    public RectTransform transitionPanel; // Tu panel con el Sprite

    [Header("Blink References")]
    public RectTransform topEyelid;
    public RectTransform bottomEyelid;

    [Header("Settings")]
    public float transitionSpeed = 1.0f;
    public float blinkSpeed = 0.3f;

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    public void Blink(Action accionWhileClosed)
    {
        StartCoroutine(BlinkRoutine(accionWhileClosed));
    }
    
    public void LoadSceneWithBlink(string sceneName)
    {
        StartCoroutine(BlinkLoadRoutine(sceneName));
    }

    // Derecha a Izquierda (Ida al minijuego)
    public void LoadSceneWithTransition(string sceneName)
    {
        StartCoroutine(HorizontalTransitionLoop(sceneName, false));
    }

    // Izquierda a Derecha (Vuelta del minijuego)
    public void LoadSceneWithTransitionReverse(string sceneName)
    {
        StartCoroutine(HorizontalTransitionLoop(sceneName, true));
    }

    private IEnumerator HorizontalTransitionLoop(string sceneName, bool isReverse)
    {
        // Obtenemos el ancho escalado del Canvas a través del padre del panel
        // Si el panel es hijo directo del Canvas, esto nos da el ancho correcto en unidades de UI
        float canvasWidth = ((RectTransform)transitionPanel.parent).rect.width;

        float startX = isReverse ? -canvasWidth : canvasWidth;
        float exitX = isReverse ? canvasWidth : -canvasWidth;

        // Primer movimiento: Entra el panel (llega a 0)
        yield return MovePanel(startX, 0);

        SceneManager.LoadScene(sceneName);
        
        // Esperar un frame para que la escena cargue y evitar tirones
        yield return null;

        // Segundo movimiento: Sale el panel (va hacia el lado opuesto)
        yield return MovePanel(0, exitX);
    }

    IEnumerator MovePanel(float startX, float endX)
    {
        float elapsedTime = 0f;
        transitionPanel.anchoredPosition = new Vector2(startX, 0);

        while (elapsedTime < transitionSpeed)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionSpeed;
            t = Mathf.SmoothStep(0, 1, t); 
            
            float currentX = Mathf.Lerp(startX, endX, t);
            transitionPanel.anchoredPosition = new Vector2(currentX, 0);
            yield return null;
        }   
        transitionPanel.anchoredPosition = new Vector2(endX, 0);
    }

    IEnumerator BlinkRoutine(Action accionWhileClosed)
    {
        float middleWindow = ((RectTransform)topEyelid.parent).rect.height / 2f;
        float elapsedTime = 0f;
        
        while (elapsedTime < blinkSpeed) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / blinkSpeed;
            float currentY = Mathf.Lerp(0, middleWindow, t);
            topEyelid.sizeDelta = new Vector2(topEyelid.sizeDelta.x, currentY);
            bottomEyelid.sizeDelta = new Vector2(bottomEyelid.sizeDelta.x, currentY);
            yield return null;
        }
        topEyelid.sizeDelta = new Vector2(topEyelid.sizeDelta.x, middleWindow);
        bottomEyelid.sizeDelta = new Vector2(bottomEyelid.sizeDelta.x, middleWindow);

        if (accionWhileClosed != null) accionWhileClosed.Invoke();
        yield return new WaitForSeconds(0.2f);

        elapsedTime = 0f;
        while (elapsedTime < blinkSpeed) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / blinkSpeed;
            float currentY = Mathf.Lerp(middleWindow, 0, t);
            topEyelid.sizeDelta = new Vector2(topEyelid.sizeDelta.x, currentY);
            bottomEyelid.sizeDelta = new Vector2(bottomEyelid.sizeDelta.x, currentY);
            yield return null;
        }
        topEyelid.sizeDelta = new Vector2(topEyelid.sizeDelta.x, 0);
        bottomEyelid.sizeDelta = new Vector2(bottomEyelid.sizeDelta.x, 0);
    }

    IEnumerator BlinkLoadRoutine(string sceneName)
    {
        float middleWindow = Screen.height / 2f;
        float elapsedTime = 0f;
        while (elapsedTime < blinkSpeed) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / blinkSpeed;
            float currentY = Mathf.Lerp(0, middleWindow, t);
            topEyelid.sizeDelta = new Vector2(topEyelid.sizeDelta.x, currentY);
            bottomEyelid.sizeDelta = new Vector2(bottomEyelid.sizeDelta.x, currentY);
            yield return null;
        }
        topEyelid.sizeDelta = new Vector2(topEyelid.sizeDelta.x, middleWindow);
        bottomEyelid.sizeDelta = new Vector2(bottomEyelid.sizeDelta.x, middleWindow);

        SceneManager.LoadScene(sceneName);
        yield return null; 

        elapsedTime = 0f;
        while (elapsedTime < blinkSpeed) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / blinkSpeed;
            float currentY = Mathf.Lerp(middleWindow, 0, t);
            topEyelid.sizeDelta = new Vector2(topEyelid.sizeDelta.x, currentY);
            bottomEyelid.sizeDelta = new Vector2(bottomEyelid.sizeDelta.x, currentY);
            yield return null;
        }
        topEyelid.sizeDelta = new Vector2(topEyelid.sizeDelta.x, 0);
        bottomEyelid.sizeDelta = new Vector2(bottomEyelid.sizeDelta.x, 0);
    }
}