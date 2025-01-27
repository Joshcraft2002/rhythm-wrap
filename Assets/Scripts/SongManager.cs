using UnityEngine;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using System.IO;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public class SongManager : MonoBehaviour
{
    private enum SongState
    {
        Waiting,
        Playing,
        Finished
    }

    public static SongManager Instance { get; private set; }
    
    [SerializeField] private int _songDelayInSeconds;
    public double marginOfError; // in seconds
    public int InputDelayInMilliSeconds; // something about keyboard issue possibility
    public float NoteDuration;
    public float NoteSpawnY;
    [SerializeField] private float _noteTapY;

    private UnityEvent<bool> _pauseToggled = new();

    private string _mapFileLocation;
    private SongState _songState = SongState.Waiting;
    private bool _UINavigable = false;
    private bool _isGamePaused = false;

    [Space]

    [SerializeField] private Lane[] Lanes;
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private GameObject _completionScreen;
    [SerializeField] private GameObject _fullClearObject;
    [SerializeField] private GameObject _pauseScreen;

    public UnityEvent<bool> PauseToggled => _pauseToggled;
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
        _musicSource.clip = SongLoader.Instance.SongClip;
        _mapFileLocation = SongLoader.Instance.MapPath;

        _completionScreen.SetActive(false);
        _pauseScreen.SetActive(false);

        PauseToggled.AddListener(SetSongPaused);

        for (int i = 0; i < Lanes.Length; i++)
        {
            Lanes[i].NoteRestriction = SongLoader.Instance.MapNotes[i];
        }

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
        if (_songState != SongState.Finished && !_musicSource.isPlaying && _musicSource.time >= _musicSource.clip.length)
        {
            _songState = SongState.Finished;
            _completionScreen.SetActive(true);
            if (ScoreManager.Instance.HasMissed)
            {
                _fullClearObject.SetActive(false);
            }
            _UINavigable = true;
        }
    }

    private IEnumerator ReadFromWebsite()
    {
        using UnityWebRequest www = UnityWebRequest.Get(Application.streamingAssetsPath + "/" + _mapFileLocation);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError(www.error);
        }
        else
        {
            byte[] results = www.downloadHandler.data;
            using var stream = new MemoryStream(results);
            MapFile = MidiFile.Read(stream);
            GetDataFromMidi();
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
        _songState = SongState.Playing;
    }

    public void SetSongPaused(bool paused)
    {
        if (paused)
            _musicSource.Pause();
        else
            _musicSource.Play();
    }

    public static double GetAudioSourceTime()
    {
        return (double)Instance._musicSource.timeSamples / Instance._musicSource.clip.frequency;
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        // if song completed, cancel key should only return to menu
        if (!context.started || _songState == SongState.Finished)
            return;

        _isGamePaused = !_isGamePaused;
        _UINavigable = _isGamePaused;
        _pauseScreen.SetActive(_isGamePaused);
        Time.timeScale = _isGamePaused ? 0f : 1f;
        PauseToggled.Invoke(_isGamePaused);
    }

    public void OnAccept(InputAction.CallbackContext context)
    {
        if (context.started && _UINavigable)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("SongLevel");
        }
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.started && _UINavigable)
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("SongSelect");
        }
    }
}
