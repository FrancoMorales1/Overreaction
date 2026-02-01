using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // Instancia estática para acceder desde cualquier script
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicMenu;
    public AudioSource musicGame;

    [Header("Settings")]
    public float transitionSpeed = 1.5f;
    private bool playingGameMusic = false;

    void Awake()
    {
        // Lógica del Singleton: ¿Ya existe uno?
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // No te destruyas al cambiar de escena
        }
        else
        {
            Destroy(gameObject); // Si ya hay uno, muere, tú eres un impostor
            return;
        }
    }

    void Update()
    {
        // Controlamos los volúmenes suavemente
        float targetMenu = playingGameMusic ? 0f : 1f;
        float targetGame = playingGameMusic ? 1f : 0f;

        musicMenu.volume = Mathf.MoveTowards(musicMenu.volume, targetMenu, transitionSpeed * Time.unscaledDeltaTime);
        musicGame.volume = Mathf.MoveTowards(musicGame.volume, targetGame, transitionSpeed * Time.unscaledDeltaTime);
    }

    public void PlayGameMusic() => playingGameMusic = true;
    public void PlayMenuMusic() => playingGameMusic = false;
}
