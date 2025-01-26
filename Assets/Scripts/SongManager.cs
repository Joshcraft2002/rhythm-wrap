using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.IO;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class SongManager : MonoBehaviour
{
    public static SongManager Instance { get; private set; }

    public Lane[] Lanes;
    [SerializeField] private int _songDelayInSeconds;
    public double marginOfError; // in seconds
    public int InputDelayInMilliSeconds; // something about keyboard issue possibility

    private string _mapFileLocation;
    private bool _songHasStarted = false;
    private bool _UINavigable = false;
    public float NoteDuration;
    public float NoteSpawnY;
    public float NoteSpawnCooldown;
    [SerializeField] private float _noteTapY;

    [Space]

    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private GameObject _completionScreen;
    [SerializeField] private GameObject _pauseScreen;

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
        _completionScreen.SetActive(false);

        _musicSource.clip = SongLoader.Instance.SongClip;
        _mapFileLocation = SongLoader.Instance.MapPath;

        for (int i = 0; i < Lanes.Length; i++)
        {
            Lanes[i].NoteRestriction = SongLoader.Instance.MapNotes[i];
        }

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

    private void Update()
    {
        if (_songHasStarted && !_musicSource.isPlaying && _musicSource.time >= _musicSource.clip.length)
        {
            _completionScreen.SetActive(true);
            _UINavigable = true;
        }
    }

    private IEnumerator ReadFromWebsite()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(Application.streamingAssetsPath + "/" + _mapFileLocation))
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
        MapFile = MidiFile.Read(Application.streamingAssetsPath + "/" + _mapFileLocation);
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
        _songHasStarted = true;
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

    public void OnAccept(InputAction.CallbackContext context)
    {
        if (context.started && _UINavigable)
            SceneManager.LoadScene("SongLevel");
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.started && _UINavigable)
            SceneManager.LoadScene("SongSelect");
    }
}
