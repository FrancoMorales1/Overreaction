using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GMPlatformScript : MonoBehaviour
{
    public TempData Data;

    [Header("Inventario Local")]
    // Manejamos el inventario aquí como pediste
    public List<string> inventory = new List<string>();
    public int maxItems = 3;

    private bool isGameOver = false;

    void Start()
    {
        if (Data.GetIsRestarting())
        {
            Data.SetIsRestarting(false); 
            Debug.Log("Escena Reiniciada: Continuando con " + Data.timeLeft + " segundos.");
        }
        else
        {
            Data.FullReset();
            inventory.Clear();
            Debug.Log("Nueva Partida: Tiempo inicial " + Data.totalTime);
        }
    }

    void Update()
    {
        if (isGameOver) return;

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

    public void AddItem(string item)
    {
        if (inventory.Count < maxItems)
        {
            inventory.Add(item);
            Debug.Log("Item capturado: " + item);
        }
    }

    public void Finish()
    {
        Data.SetIsRestarting(true);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Restart()
    {
        Data.SetIsRestarting(true);

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void GameOver()
    {
        isGameOver = true;
        Debug.Log("¡Perdiste! Te quedaste sin tiempo.");
    }
}