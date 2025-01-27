using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void OnAccept(InputAction.CallbackContext context)
    {
        if (context.started)
            SceneManager.LoadScene("SongSelect");
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started)
            Application.Quit();
    }
}
