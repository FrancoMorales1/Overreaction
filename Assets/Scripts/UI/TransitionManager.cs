using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;

    [Header("UIReferences")]
    public RectTransform transitionPanel;

    [Header("BlinkReferences")]
    public RectTransform topEyelid;
    public RectTransform bottomEyelid;


    [Header("Transition Settings")]
    public float transitionSpeed = 1.0f;
    public float blinkSpeed = 0.3f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Blink(Action accionWhileClosed)
    {
        StartCoroutine(BlinkRoutine(accionWhileClosed));
    }

    IEnumerator BlinkRoutine(Action accionWhileClosed)
    {
        float middleWindow = Screen.height / 2f;
        float elapsedTime = 0f;

        while (elapsedTime < blinkSpeed)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / blinkSpeed;
            float currentY = Mathf.Lerp(0, middleWindow, t);

            topEyelid.sizeDelta = new Vector2(topEyelid.sizeDelta.x, currentY);
            bottomEyelid.sizeDelta = new Vector2(bottomEyelid.sizeDelta.x, currentY);
            yield return null;  

        }
        topEyelid.sizeDelta = new Vector2(topEyelid.sizeDelta.x, middleWindow);
        bottomEyelid.sizeDelta = new Vector2(bottomEyelid.sizeDelta.x, middleWindow);

        if (accionWhileClosed != null)
        {
            accionWhileClosed.Invoke();
        }
        yield return new WaitForSeconds(0.2f);

        elapsedTime = 0f;
        while (elapsedTime < blinkSpeed)
        {
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
    // --- ESTA ES LA FUNCIÓN PÚBLICA QUE LLAMAS DESDE OTROS SCRIPTS ---
    // Tipo 'void' para que sea fácil de llamar
    public void LoadSceneWithTransition(string sceneName)
    {
        StartCoroutine(TransitionLoop(sceneName));
    }

    // --- ESTA ES LA LÓGICA INTERNA ---
    // Tipo 'IEnumerator' porque usa 'yield return' (esperas)
    private IEnumerator TransitionLoop(string sceneName)
    {
        float panelWidth = Screen.width;

        // 1. Mover panel hacia el centro (Tapar pantalla)
        yield return MovePanel(panelWidth, 0);

        // 2. Cargar escena
        SceneManager.LoadScene(sceneName);

        // 3. Esperar un frame para asegurar que Unity cargó todo
        yield return null;

        // 4. Mover panel hacia la izquierda (Destapar pantalla)
        yield return MovePanel(0, -panelWidth);
    }

    IEnumerator MovePanel(float startX, float endX)
    {
        float elapsedTime = 0f;
        
        // Aseguramos la posición inicial antes de empezar el bucle
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

    public void LoadSceneWithBlink(string sceneName)
    {
        StartCoroutine(BlinkLoadRoutine(sceneName));
    }

    IEnumerator BlinkLoadRoutine(string sceneName)
    {
        float middleWindow = Screen.height / 2f;
        float elapsedTime = 0f;

        // 1. CERRAR OJOS
        while (elapsedTime < blinkSpeed)
        {
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

        // Esperamos un frame para que Unity termine de cargar y despertar los objetos de la nueva escena
        yield return null; 

        // 3. ABRIR OJOS
        elapsedTime = 0f;
        while (elapsedTime < blinkSpeed)
        {
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