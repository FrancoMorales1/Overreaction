using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GMPlatformScript : MonoBehaviour
{
    public TempData Data;

    [Header("UI Referencias")]
    public List<InventorySlot> uiSlots;

    [Header("Inventario Local")]
    public List<string> inventory = new List<string>();
    public List<string> categoriesCollected = new List<string>();
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

    public bool AddItem(string item, string category)
    {
        if (categoriesCollected.Contains(category))
        {
            Debug.Log("Ya tienes un objeto de la categoría: " + category);
            return false; // El objeto NO se destruye en el mapa
        }
        if (inventory.Count < maxItems)
        {
            inventory.Add(item);
            categoriesCollected.Add(category);
            foreach (var slot in uiSlots)
            {
                if (slot.category == category)
                {
                    bool isCorrect = correctItemNames.Contains(item);
                    slot.SetStatus(isCorrect);
                    break; 
                }
            }
            Debug.Log("Item capturado: " + item);
            return true;
        } else
        {
            Debug.Log("Inventario lleno. No se puede capturar: " + item);
            return false;
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
        Time.timeScale = 0f;
        Debug.Log("El juego ha terminado.");
    }
}