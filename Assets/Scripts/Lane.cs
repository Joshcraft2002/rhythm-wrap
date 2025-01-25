using Melanchall.DryWetMidi.Interaction;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Lane : MonoBehaviour
{
    public Melanchall.DryWetMidi.MusicTheory.NoteName NoteRestriction;
    [SerializeField] private GameObject NotePrefab;
    List<MusicNote> notes = new();
    public List<double> HitTimeStamps = new();

    private int _spawnIndex = 0;
    private int _inputIndex = 0;

    public void SetTimeStamps(Note[] array)
    {
        foreach (var note in array)
        {
            if (note.NoteName == NoteRestriction)
            {
                var metricTimeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(note.Time, SongManager.MapFile.GetTempoMap());
                HitTimeStamps.Add((double)metricTimeSpan.Minutes * 60f + metricTimeSpan.Seconds + (double)metricTimeSpan.Milliseconds/1000f);
            }
        }
    }

    void Update()
    {
        if (_spawnIndex < HitTimeStamps.Count)
        {
            if (SongManager.GetAudioSourceTime() >= HitTimeStamps[_spawnIndex] - SongManager.Instance.NoteDuration)
            {
                var note = Instantiate(NotePrefab, transform);
                notes.Add(note.GetComponent<MusicNote>());
                note.GetComponent<MusicNote>().AssignedTapTime = (float)HitTimeStamps[_spawnIndex];
                _spawnIndex++;
            }
        }

        if (_inputIndex < HitTimeStamps.Count)
        {
            double timeStamp = HitTimeStamps[_inputIndex];
            double marginOfError = SongManager.Instance.marginOfError;
            double audioTime = SongManager.GetAudioSourceTime() - (SongManager.Instance.InputDelayInMilliSeconds / 1000.0);

            if (timeStamp + marginOfError <= audioTime)
            {
                Miss();
                print($"Missed {_inputIndex} note");
                _inputIndex++;
            }
        }
    }

    public void OnPress(InputAction.CallbackContext context)
    {
        if (!context.started)
            return;

        if (_inputIndex < HitTimeStamps.Count)
        {
            double timeStamp = HitTimeStamps[_inputIndex];
            double marginOfError = SongManager.Instance.marginOfError;
            double audioTime = SongManager.GetAudioSourceTime() - (SongManager.Instance.InputDelayInMilliSeconds / 1000.0);
            
            if (Math.Abs(audioTime - timeStamp) < marginOfError )
            {
                Hit();
                print($"Hit on {_inputIndex} note");
                //Destroy(notes[_inputIndex].gameObject);
                notes[_inputIndex].OnHit();
                _inputIndex++;
            }
            else
            {
                Miss();
                print($"Hit inaccurate on {_inputIndex} note with {Math.Abs(audioTime - timeStamp)} delay");
            }
        }
        
    }

    private void Hit()
    {
        ScoreManager.Instance.Hit();
    }

    private void Miss()
    {
        ScoreManager.Instance.Miss();
    }
}
