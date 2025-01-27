using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Melanchall.DryWetMidi.MusicTheory;

public class SongSelectController : MonoBehaviour
{
    public static SongSelectController Instance { get; private set; }

    [SerializeField] Button _firstButton;
    private EventSystem eventSystem;

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

    private void Start()
    {
        eventSystem = EventSystem.current;
        eventSystem.SetSelectedGameObject(_firstButton.gameObject);
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.started)
            SceneManager.LoadScene("MainMenu");
    }

    public void OnUp(InputAction.CallbackContext context)
    {
        if (context.started)
            SelectNextButton(-1);
    }

    public void OnDown(InputAction.CallbackContext context)
    {
        if (context.started)
            SelectNextButton(1);
    }

    public void LoadSong(AudioClip songClip, string mapPath, NoteName note1, NoteName note2, NoteName note3)
    {
        SongLoader.Instance.SongClip = songClip;
        SongLoader.Instance.MapPath = mapPath;
        SongLoader.Instance.MapNotes = new NoteName[] { note1, note2, note3 };
        SceneManager.LoadScene("SongLevel");
    }

    private void SelectNextButton(int direction)
    {
        GameObject currentSelected = eventSystem.currentSelectedGameObject;
        if (currentSelected == null) return;

        Selectable currentSelectable = currentSelected.GetComponent<Selectable>();
        Selectable nextSelectable = direction > 0 ? currentSelectable.FindSelectableOnDown() : currentSelectable.FindSelectableOnUp();

        if (nextSelectable != null)
        {
            eventSystem.SetSelectedGameObject(nextSelectable.gameObject);
        }
    }
}
