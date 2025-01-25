using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    private UnityEvent<bool> _pauseToggled = new();

    public UnityEvent<bool> PauseToggled => _pauseToggled;

    private bool _isGamePaused = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ToggleGamePause()
    {
        _isGamePaused = !_isGamePaused;
        Time.timeScale = _isGamePaused ? 0f : 1f;
        PauseToggled.Invoke(_isGamePaused);
    }
}
