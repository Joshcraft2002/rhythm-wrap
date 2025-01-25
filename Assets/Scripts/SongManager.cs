using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.IO;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.InputSystem;

public class SongManager : MonoBehaviour
{
    public static SongManager Instance;

    public Lane[] Lanes;
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private int _songDelayInSeconds;
    public double marginOfError; // in seconds
    public int InputDelayInMilliSeconds; // something about keyboard issue possibility
    
    public string MapFileLocation;
    public float NoteDuration;
    public float NoteSpawnY;
    public float NoteSpawnCooldown;
    [SerializeField] private float _noteTapY;
    public float NoteDespawnY => _noteTapY - (NoteSpawnY - _noteTapY);

    /// <summary>
    /// Midi file stored in RAM
    /// </summary>
    public static MidiFile MapFile;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        GameManager.Instance.PauseToggled.AddListener(SetSongPaused);

        if (Application.streamingAssetsPath.StartsWith("http://") || Application.streamingAssetsPath.StartsWith("https://"))
        {
            StartCoroutine(ReadFromWebsite());
        }
        else
        {
            ReadFromFile();
        }
    }

    private IEnumerator ReadFromWebsite()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(Application.streamingAssetsPath + "/" + MapFileLocation))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(www.error);
            }
            else
            {
                byte[] results = www.downloadHandler.data;
                using (var stream = new MemoryStream(results))
                {
                    MapFile = MidiFile.Read(stream);
                    GetDataFromMidi();
                }
            }

        }
    }

    private void ReadFromFile()
    {
        MapFile = MidiFile.Read(Application.streamingAssetsPath + "/" + MapFileLocation);
        GetDataFromMidi();
    }

    public void GetDataFromMidi()
    {
        var notes = MapFile.GetNotes();
        var array = new Note[notes.Count];
        notes.CopyTo(array, 0);

        foreach (var lane in Lanes)
        {
            lane.SetTimeStamps(array);
        }

        Invoke(nameof(StartSong), _songDelayInSeconds);
    }

    public void StartSong()
    {
        _musicSource.Play();
    }

    public void SetSongPaused(bool paused)
    {
        if (paused)
            _musicSource.Pause();
        else
            _musicSource.Play();
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (!context.started)
            return;

        GameManager.Instance.ToggleGamePause();
    }

    public static double GetAudioSourceTime()
    {
        return (double)Instance._musicSource.timeSamples / Instance._musicSource.clip.frequency;
    }
}
