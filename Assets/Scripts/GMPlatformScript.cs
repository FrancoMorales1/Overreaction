using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public string category;
    public bool isCorrect;
    public int status; // 3 = Intacto, <3 = Dañado

    public InventoryItem(string name, string cat, bool correct)
    {
        itemName = name;
        category = cat;
        isCorrect = correct;
        status = 3;
    }
}

public class GMPlatformScript : MonoBehaviour
{
    public TempData Data;

    [Header("Configuración Respawn")]
    public Transform respawnPoint;

    [Header("UI Referencias")]
    public List<InventorySlot> uiSlots;

    [Header("Inventario Local")]
    public List<InventoryItem> inventory = new List<InventoryItem>();

    public int maxItems = 3;
    public List<string> correctItemNames = new List<string>();

    private bool isGameOver = false;
    private bool isPaused = false;
    private bool isInFinishZone = false;

    void Start()
    {
        Time.timeScale = 1f;
        ResetUI();
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
            return true;
        }
        return false;
    }

    public void PlayerHit(Vector2 hitDirection)
    {
        bool foundSomethingToBreak = false;

        foreach (var item in inventory)
        {
            if (item.isCorrect)
            {
                item.status -= 1;
                UpdateSlotUI(item.category, item.isCorrect, true);
                foundSomethingToBreak = true;
                Debug.Log("¡Se rompió!: " + item.category);
                // Opcional: Si el status llega a 0, ¿deja de contar como punto?
            }
        }

        if (!foundSomethingToBreak) RespawnPlayer();
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
            Debug.Log("Sin objetos sanos. Volviendo al inicio.");
        }
    }

    public void SetInFinishZone(bool value)
    {
        isInFinishZone = value;
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
                if (item.status > 0) 
                {
                    score += item.status; 
                }
            }
        }
        return score;
    }

    public void FinishButton()
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
        Time.timeScale = isPaused ? 0f : 1f; 
        Debug.Log(isPaused ? "Juego Pausado" : "Juego Reanudado");
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