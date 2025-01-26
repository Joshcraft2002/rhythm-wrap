using Melanchall.DryWetMidi.MusicTheory;
using UnityEngine;
using UnityEngine.UI;

public class SongSelectButton : MonoBehaviour
{
    [SerializeField] private AudioClip _songClip;
    [SerializeField] private string _mapPath;
    [SerializeField] private NoteName _mapNote1;
    [SerializeField] private NoteName _mapNote2;
    [SerializeField] private NoteName _mapNote3;

    private Button _btn;

    private void Start()
    {
        _btn = GetComponent<Button>();
        _btn.onClick.AddListener(() => SongSelectController.Instance.LoadSong(_songClip, _mapPath, _mapNote1, _mapNote2, _mapNote3));
    }
}
