using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public string category;
    public bool isCorrect;
    
    public Sprite itemIcon;

    public InventoryItem(string name, string cat, bool correct)
    {
        itemName = name;
        category = cat;
        isCorrect = correct;

        string path = "Items/" + cat;

        Sprite[] categorySprites = Resources.LoadAll<Sprite>(path);

        if (categorySprites.Length > 0)
        {
            int randomIndex = Random.Range(0, categorySprites.Length);
            itemIcon = categorySprites[randomIndex];
        }
        else
        {
            Debug.LogError($"No se encontraron imágenes en: Resources/{path}. Revisa que el nombre de la carpeta coincida con la categoría.");
        }
    }
}

public class GMPlatformScript : MonoBehaviour
{
    public TempData Data;

    [Header("Configuración Respawn")]
    public Transform respawnPoint;

    [Header("Referencias del Mundo")]
    public List<TargetScript> allWorldItems = new List<TargetScript>();

    [Header("UI Referencias")]
    public List<InventorySlot> uiSlots;

    [Header("UI Feedback")]
    public GameObject notificationPanel;
    public float displayTime = 3f;
    private bool notificationShown = false;

    [Header("Inventario Local")]
    public List<InventoryItem> inventory = new List<InventoryItem>();

    [Header("UI Pause")]
    public GameObject pausePanel;

    public int maxItems = 3;
    public List<string> correctItemNames = new List<string>();

    private bool isGameOver = false;
    private bool isPaused = false;
    private bool isInFinishZone = false;

    void Start()
    {
        Time.timeScale = 1f;
        ResetUI();
        notificationShown = false;
        if (Data.GetIsRestarting())
        {
            Data.SetIsRestarting(false);
        }
        else
        {
            Data.FullReset();
            inventory.Clear();
        }
    }

    void Awake()
    {
        // Forzamos que el panel esté apagado antes de cualquier otra cosa
        if (notificationPanel != null)
        {
            notificationPanel.SetActive(false);
        }
        
        // También reseteamos la variable por seguridad
        notificationShown = false;
    }
    void ResetUI()
    {
        foreach (var slot in uiSlots)
        {
            slot.CleanSlot();
        }
    }

    void Update()
    {
        if (isGameOver || isPaused) return;

        if (Data.timeLeft > 0)
        {
            Data.timeLeft -= Time.deltaTime;
        }
        else
        {
            Data.timeLeft = 0;
            GameOver(); // Se acabó el tiempo
        }
    }

    public void RegisterWorldItem(TargetScript item) 
    {
        if(!allWorldItems.Contains(item)) allWorldItems.Add(item);
    }

    public bool AddItem(string name, string category)
    {
        foreach (var item in inventory)
        {
            if (item.category == category) return false;
        }

        if (inventory.Count < maxItems)
        {
            bool correct = correctItemNames.Contains(name);
            InventoryItem newItem = new InventoryItem(name, category, correct);
            inventory.Add(newItem);

            UpdateSlotUI(category, correct, false);

            if (inventory.Count >= maxItems && !notificationShown)
            {
                StartCoroutine(ShowNotificationRoutine());
            }
            return true;
        }
        return false;
    }

    private IEnumerator ShowNotificationRoutine()
    {
        notificationShown = true;
        if (notificationPanel != null)
        {
            notificationPanel.SetActive(true);
            
            // Espera unos segundos (displayTime)
            yield return new WaitForSeconds(displayTime);
            
            notificationPanel.SetActive(false);
        }
    }
    public void PlayerHit(Vector2 hitDirection)
    {
        List<InventoryItem> itemsToRemove = new List<InventoryItem>();
        bool atLeastOneWasCorrect = false;

        foreach (var item in inventory)
        {
            if (item.isCorrect && item.status > 0)
            {
                atLeastOneWasCorrect = true;
                item.status -= 1;
                if (item.status <= 0)
                {
                    itemsToRemove.Add(item);
                } else
                {
                    UpdateSlotUI(item.category, item.isCorrect, true);
                }
                Debug.Log("¡Se rompió!: " + item.category);
                // Opcional: Si el status llega a 0, ¿deja de contar como punto?
            }
        }

        // 3. Procesamos los objetos que deben ser eliminados y reaparecer
        if (itemsToRemove.Count > 0)
        {
            notificationShown = false;
            foreach (var itemDead in itemsToRemove)
            {
                string cat = itemDead.category;
                string name = itemDead.itemName;

                // Quitar del inventario lógico
                inventory.Remove(itemDead);

                // Limpiar su slot en la UI
                foreach (var slot in uiSlots)
                {
                    if (slot.category == cat) { slot.CleanSlot(); break; }
                }

                // Reaparecer el objeto en el mapa
                foreach (var worldItem in allWorldItems)
                {
                    if (worldItem.itemName == name && worldItem.itemCategory == cat)
                    {
                        worldItem.gameObject.SetActive(true);
                        Debug.Log($"Reapareciendo {name} en el mundo.");
                        break;
                    }
                }
            }
        }

        // 4. Si John no tenía NINGÚN objeto correcto sano al ser golpeado
        if (!atLeastOneWasCorrect)
        {
            RespawnPlayer();
        }
    }

    private void UpdateSlotUI(string category, bool isCorrect, bool isDamaged)
    {
        foreach (var slot in uiSlots)
        {
            if (slot.category == category)
            {
                slot.SetStatus(isCorrect, isDamaged);
                break;
            }
        }
    }

    void RespawnPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && respawnPoint != null)
        {
            player.transform.position = respawnPoint.position;

            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.angularVelocity = 0f;
            }

            Debug.Log("Sin objetos sanos. Volviendo al inicio sin impulso.");
        }   
    }

    public void SetInFinishZone(bool value)
    {
        isInFinishZone = value;

        if (isInFinishZone && inventory.Count >= maxItems)
        {
            Debug.Log("Zona de meta alcanzada con todos los items. Finalizando automáticamente...");
            CheckVictory(); 
        }
    }

    // --- AQUÍ ESTÁ LA MAGIA ---
    // Función auxiliar para saber cuántos puntos ganó el jugador
    int CalculateFinalScore()
    {
        int score = 0;
        foreach (var item in inventory)
        {
            // Solo damos puntos si el objeto es el CORRECTO
            if (item.isCorrect)
            {
                // Sumamos el 'status' (ej: si está intacto suma 3, si está roto suma 1 o 2)
                // Esto permite que el diálogo cambie si el jugador llegó muy golpeado
                score += 3; 
            }
        }
        return score;
    }

    public void CheckVictory()
    {
        // CONDICIÓN DE VICTORIA
        if (isInFinishZone && inventory.Count >= maxItems)
        {
            Debug.Log("¡Nivel Completado!");
            isGameOver = true; // Frenamos el juego

            // 1. Calculamos el puntaje
            int finalScore = CalculateFinalScore();

            // 2. Contactamos al Manager
            if (QuestFlowManager.Instance != null)
            {
                // Le pasamos los datos
                QuestFlowManager.Instance.lastPointReached = finalScore;
                QuestFlowManager.Instance.questCompleted = true; // Avisamos que volvemos con éxito
                

                QuestFlowManager.Instance.AdjustReputation(finalScore);
                // 3. ¡SOLO CARGAMOS LA ESCENA!
                // No llamamos a EndDialogueManager todavía. Eso lo hará el texto cuando termine de leerse.
                UnityEngine.SceneManagement.SceneManager.LoadScene("QuestScene"); 
            }
            else
            {
                // Fallback
                Debug.LogWarning("No se encontró QuestFlowManager...");
                UnityEngine.SceneManagement.SceneManager.LoadScene("QuestScene");
            }
        }
        else if (!isInFinishZone)
        {
            Debug.Log("No puedes terminar: No estás en la zona de meta.");
        }
        else if (inventory.Count < maxItems)
        {
            Debug.Log("No puedes terminar: Te faltan objetos.");
        }
    }

    public void RestartButton()
    {
        Time.timeScale = 1f;
        Data.SetIsRestarting(true);
        // Usamos la ruta completa para evitar el error de nombres que tenías antes
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void PauseButton()
    {
        isPaused = !isPaused;
        
        if (isPaused)
        {
            Time.timeScale = 0f; // Congela el tiempo
            if(pausePanel != null) pausePanel.SetActive(true); // Muestra el menú
            Debug.Log("Juego Pausado");
        }
        else
        {
            Time.timeScale = 1f; // Reanuda el tiempo
            if(pausePanel != null) pausePanel.SetActive(false); // Oculta el menú
            Debug.Log("Juego Reanudado");
        }
    }

    public void ExitToMenu()
    {
        Time.timeScale = 1f;

        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
    }

    void GameOver()
    {
        // Esto pasa si se acaba el tiempo (perder)
        isGameOver = true;
        Debug.Log("Se acabó el tiempo.");
        
        // AQUÍ DECIDES: 
        // Opción A: Reiniciar el nivel automáticamente
        RestartButton();

        // Opción B: Volver al QuestScene diciendo que fallaste (0 puntos)
        /*
        if (QuestFlowManager.Instance != null) {
             QuestFlowManager.Instance.lastPointReached = 0;
             QuestFlowManager.Instance.questCompleted = true; 
             QuestFlowManager.Instance.EndDialogueManager();
        }
        */
    }
}