using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;

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
        string path = "Items/" + cat + "/"+ name;

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

    public void CorruptItem()
    {
        string nameToAvoid = itemName;
        isCorrect = false;

        string[] suffixes = { "A", "B", "C" };
        
        List<string> incorrectOptions = new List<string>();
        foreach (string s in suffixes)
        {
            string potentialName = category + s;
            if (potentialName != nameToAvoid)
            {
                incorrectOptions.Add(potentialName);
            }
        }

        itemName = incorrectOptions[Random.Range(0, incorrectOptions.Count)];

        string path = "Items/" + category + "/" + itemName;
        Sprite[] categorySprites = Resources.LoadAll<Sprite>(path);

        if (categorySprites.Length > 0)
        {
            itemIcon = categorySprites[Random.Range(0, categorySprites.Length)];
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

    [Header("Audio")]
    public AudioClip deliverySound;
    public float deliveryVolume = 1f;

    void Start()
    {
        AudioManager.Instance.PlayGameMusic();
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
        if (notificationPanel != null)
        {
            notificationPanel.SetActive(false);
        }
        ResetUI();
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
        if (Keyboard.current != null)
        {
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                RestartButton();
            }

            if (Keyboard.current.pKey.wasPressedThisFrame)
            {
                PauseButton();
            }
        }
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

            UpdateSlotUI(category, correct);

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
            
            yield return new WaitForSeconds(displayTime);
            
            notificationPanel.SetActive(false);
        }
    }

    public void PlayerHit(Vector2 hitDirection)
    {
        bool atLeastOneWasCorrect = false;

        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i].isCorrect)
            {
                atLeastOneWasCorrect = true;

                inventory[i].CorruptItem();
                
                UpdateSlotUI(inventory[i].category, inventory[i].isCorrect);
                
                Debug.Log($"¡Item dañado! Ahora es incorrecto: {inventory[i].itemName}");

                break;
            }
        }

        if (!atLeastOneWasCorrect)
        {
            RespawnPlayer();
        }
    }

    private void UpdateSlotUI(string category, bool isCorrect)
    {
        foreach (var slot in uiSlots)
        {
            if (slot.category == category)
            {
                slot.SetStatus(isCorrect);
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
            if (deliverySound != null)
            {
                Debug.Log("Reproduciendo sonido de entrega.");
                Vector3 spawnPos = Camera.main.transform.position;
                spawnPos.z = Camera.main.transform.position.z + 1f;

                AudioSource.PlayClipAtPoint(deliverySound, spawnPos, deliveryVolume);
            }

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
            isGameOver = true;
            Time.timeScale = 0f;
            Debug.Log("Victoria detectada. Congelando y esperando para cambiar de escena...");
            StartCoroutine(WaitAndLoadRoutine());
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

    private IEnumerator WaitAndLoadRoutine()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        Debug.Log("¡Nivel Completado!");
        // 1. Calculamos el puntaje
        int finalScore = CalculateFinalScore();

        // 2. Contactamos al Manager
        if (QuestFlowManager.Instance != null)
        {
            // Le pasamos los datos
            QuestFlowManager.Instance.lastPointReached = finalScore;
            QuestFlowManager.Instance.questCompleted = true; // Avisamos que volvemos con éxito
            
            foreach (var item in inventory)
            {
                if (item.category == "Cejas") QuestFlowManager.Instance.faceBrows = item.itemIcon;
                else if (item.category == "Ojos") QuestFlowManager.Instance.faceEyes = item.itemIcon;
                else if (item.category == "Boca") QuestFlowManager.Instance.faceMouth = item.itemIcon;
            }
            Debug.Log("Guardando parte de la cara: " + QuestFlowManager.Instance.faceBrows);
            Debug.Log("Guardando parte de la cara: " + QuestFlowManager.Instance.faceEyes);
            Debug.Log("Guardando parte de la cara: " + QuestFlowManager.Instance.faceMouth);
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
        List<string> expectedCategories = new List<string> { "Cejas", "Ojos", "Boca" };

        if (isGameOver) return;
        isGameOver = true;
        Debug.Log("Se acabó el tiempo.");
        
        foreach (string cat in expectedCategories)
        {
            bool hasCategory = false;
            foreach (var item in inventory)
            {
                if (item.category == cat)
                {
                    hasCategory = true;
                    break;
                }
            }
            if (!hasCategory)
            {
                InventoryItem fakeItem = new InventoryItem(cat + "A", cat, false);
                
                fakeItem.CorruptItem(); 
                
                inventory.Add(fakeItem);
                UpdateSlotUI(cat, false);
                Debug.Log($"Generado item de relleno incorrecto para: {cat}");
            }
        }

        isInFinishZone = true;

        CheckVictory();
    }
}