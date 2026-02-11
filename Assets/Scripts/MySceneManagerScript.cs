using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour
{
    public GameObject panelCreditos;

    void Start()
    {
        AudioManager.Instance.PlayMenuMusic();
    }
    public void ChangeScene(string scenename)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(scenename);
    }

    public void ShowCredits()
    {
        panelCreditos.SetActive(true);
    }

    public void HideCredits()
    {
        panelCreditos.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
