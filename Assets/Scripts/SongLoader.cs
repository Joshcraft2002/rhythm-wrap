using UnityEngine;

public class SongLoader : MonoBehaviour
{
    public static SongLoader Instance { get; private set; }

    public AudioClip SongClip;
    public string MapPath;
    public Melanchall.DryWetMidi.MusicTheory.NoteName[] MapNotes = new Melanchall.DryWetMidi.MusicTheory.NoteName[3];

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
