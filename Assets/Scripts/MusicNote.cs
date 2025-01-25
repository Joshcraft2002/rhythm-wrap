using UnityEngine;

public class MusicNote : MonoBehaviour
{
    public float AssignedTapTime; //when should be tapped

    private double _timeInstantiated;
    private bool _isPopped = false;
    private Animator _animator;

    private static string POP_ANIMATION = "Pop";

    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    private void Start()
    {
        _timeInstantiated = SongManager.GetAudioSourceTime();
        transform.localPosition = Vector3.up * SongManager.Instance.NoteSpawnY;
        GetComponent<SpriteRenderer>().enabled = true;
    }

    private void Update()
    {
        if (_isPopped)
            return;

        double timeSinceInstantiated = SongManager.GetAudioSourceTime() - _timeInstantiated;
        float t = (float)(timeSinceInstantiated / (SongManager.Instance.NoteDuration * 2));

        if (t > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.localPosition = Vector3.Lerp(Vector3.up * SongManager.Instance.NoteSpawnY, Vector3.up * SongManager.Instance.NoteDespawnY, t);
        }
    }

    public void OnHit()
    {
        _isPopped = true;
        _animator.Play(POP_ANIMATION);
    }

    public void OnPop()
    {
        Destroy(gameObject);
    }

    //private float _beat; //beat of this note
    //private Vector2 _noteCreationPos;
    //private Vector2 _noteDeletePos;

    //public void Setup(Vector2 noteCreationPos, Vector2 noteDeletePos, float beat)
    //{
    //    _noteCreationPos = noteCreationPos;
    //    _noteDeletePos = noteDeletePos;
    //    _beat = beat;
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    transform.position = Vector2.Lerp(
    //        _noteCreationPos,
    //        _noteDeletePos,
    //        (SongManager.Instance.BeatsShownInAdvance - (_beat - SongManager.Instance.songPosInBeats)) / SongManager.Instance.BeatsShownInAdvance
    //    );


    //}
}
