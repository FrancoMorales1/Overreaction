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
    public Sprite itemSprite;

    public InventoryItem(string name, string cat, bool correct)
    {
        itemName = name;
        category = cat;
        isCorrect = correct;
        string path = "Items/" + cat + "/" + name;

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

        string spritePath = "ItemsSprite/" + cat + "/" + name;
        itemSprite = Resources.Load<Sprite>(spritePath);
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

        string spritePath = "ItemsSprite/" + category + "/" + itemName;
        itemSprite = Resources.Load<Sprite>(spritePath);
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
    public List<InventoryHaveScript> haveSlots;

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
        foreach (var slot in uiSlots) slot.CleanSlot();
        foreach (var hSlot in haveSlots) hSlot.CleanSlot();
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
        if (!allWorldItems.Contains(item)) allWorldItems.Add(item);
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

            UpdateSlotUI(category, correct, newItem.itemSprite);

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

                UpdateSlotUI(inventory[i].category, inventory[i].isCorrect, inventory[i].itemSprite);

                Debug.Log($"¡Item dañado! Ahora es incorrecto: {inventory[i].itemName}");

                break;
            }
        }

        if (!atLeastOneWasCorrect)
        {
            RespawnPlayer();
        }
    }

    private void UpdateSlotUI(string category, bool isCorrect, Sprite currentSprite)
    {
        Debug.Log($"Actualizando UI para categoría: {category}, Correcto: {isCorrect}, Sprite: {currentSprite}");
        foreach (var slot in uiSlots)
        {
            if (slot.category == category)
            {
                slot.SetStatus(isCorrect);
                break;
            }
        }

        foreach (var hSlot in haveSlots)
        {
            if (hSlot.category == category)
            {
                hSlot.SetItem(currentSprite);
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

    int CalculateFinalScore()
    {
        int score = 0;
        foreach (var item in inventory)
        {
            if (item.isCorrect)
            {
                score += 3;
            }
        }
        return score;
    }

    public void CheckVictory()
    {
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

        Time.timeScale = 1f;
        AudioManager.Instance.PlayMenuMusic();

        Debug.Log("¡Nivel Completado!");
        
        int finalScore = CalculateFinalScore();

        if (QuestFlowManager.Instance != null)
        {
            QuestFlowManager.Instance.lastPointReached = finalScore;
            QuestFlowManager.Instance.questCompleted = true;

            foreach (var item in inventory)
            {
                if (item.category == "Cejas") QuestFlowManager.Instance.faceBrows = item.itemIcon;
                else if (item.category == "Ojos") QuestFlowManager.Instance.faceEyes = item.itemIcon;
                else if (item.category == "Boca") QuestFlowManager.Instance.faceMouth = item.itemIcon;
            }
            
            Debug.Log("Guardando partes de la cara...");
            QuestFlowManager.Instance.AdjustReputation(finalScore);

            // --- CAMBIO: Usamos la transición inversa (Izquierda -> Derecha) ---
            if (TransitionManager.Instance != null)
            {
                TransitionManager.Instance.LoadSceneWithTransitionReverse("QuestScene");
            }
            else
            {
                // Fallback por seguridad
                UnityEngine.SceneManagement.SceneManager.LoadScene("QuestScene");
            }
        }
        else
        {
            Debug.LogWarning("No se encontró QuestFlowManager...");
            
            // Fallback con transición si es posible
            if (TransitionManager.Instance != null)
            {
                TransitionManager.Instance.LoadSceneWithTransitionReverse("QuestScene");
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("QuestScene");
            }
        }
    }

    public void RestartButton()
    {
        Time.timeScale = 1f;
        Data.SetIsRestarting(true);
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void PauseButton()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            if (pausePanel != null) pausePanel.SetActive(true);
            Debug.Log("Juego Pausado");
        }
        else
        {
            Time.timeScale = 1f;
            if (pausePanel != null) pausePanel.SetActive(false);
            Debug.Log("Juego Reanudado");
        }
    }

    public void ExitToMenu()
    {
        Time.timeScale = 1f;

        // --- CAMBIO: Transición suave al menú principal ---
        if (TransitionManager.Instance != null)
        {
            TransitionManager.Instance.LoadSceneWithTransition("MainMenuScene");
        }
        else
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenuScene");
        }
    }

    void GameOver()
    {
        if (isGameOver) return;
        isGameOver = true;
        Debug.Log("Se acabó el tiempo. Generando fallos aleatorios...");

        string[] expectedCategories = { "Cejas", "Ojos", "Boca" };
        string[] suffixes = { "A", "B", "C" }; 

        foreach (string cat in expectedCategories)
        {
            // 1. Verificamos si el jugador ya tiene algo de esta categoría
            bool hasCategory = inventory.Exists(item => item.category == cat);

            if (!hasCategory)
            {
                // 2. Creamos una lista de candidatos que sabemos que son INCORRECTOS
                List<string> wrongOptions = new List<string>();
                
                foreach (string s in suffixes)
                {
                    string potentialName = cat + s;
                    // Solo lo agregamos si NO está en la lista de aciertos
                    if (!correctItemNames.Contains(potentialName))
                    {
                        wrongOptions.Add(potentialName);
                    }
                }

                // 3. Elegimos uno al azar de la lista de incorrectos
                string chosenWrongName;
                if (wrongOptions.Count > 0)
                {
                    chosenWrongName = wrongOptions[Random.Range(0, wrongOptions.Count)];
                }
                else
                {
                    // Fallback extremo si configuraste mal los nombres en el inspector
                    chosenWrongName = cat + "X"; 
                }

                // 4. Creamos el ítem y lo metemos al inventario
                InventoryItem fakeItem = new InventoryItem(chosenWrongName, cat, false);
                inventory.Add(fakeItem);
                
                // 5. Actualizamos la UI para que se vea el "regalito"
                UpdateSlotUI(cat, false, fakeItem.itemSprite);
                
                Debug.Log($"[GameOver] Faltaba {cat}. Se asignó aleatoriamente: {chosenWrongName}");
            }
        }

        isInFinishZone = true;
        CheckVictory();
    }
}