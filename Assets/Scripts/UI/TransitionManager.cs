using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager Instance;

    [Header("UIReferences")]
    public RectTransform transitionPanel;

    [Header("Transition Settings")]
    public float transitionSpeed = 1.0f;

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
}