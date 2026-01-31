using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

[System.Serializable]
public class InventoryItem
{
    public string itemName;
    public string category;
    public bool isCorrect;
    public int status;

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
            GameOver();
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

        // Buscamos el primero que sea correcto y no esté dañado
        foreach (var item in inventory)
        {
            if (item.isCorrect)
            {
                item.status -= 1;
                
                UpdateSlotUI(item.category, item.isCorrect, true);
                
                foundSomethingToBreak = true;
                Debug.Log("¡Se rompió!: " + item.category);
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

    public void FinishButton()
    {
        if (isInFinishZone && inventory.Count >= maxItems)
        {
            Debug.Log("¡Nivel Completado!");
            GameOver();
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PauseButton()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0f : 1f; 
        Debug.Log(isPaused ? "Juego Pausado" : "Juego Reanudado");
    }

    void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene("QuestScene");
        Debug.Log("El juego ha terminado.");
    }
}