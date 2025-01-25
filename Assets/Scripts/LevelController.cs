using UnityEngine;

public class LevelController : MonoBehaviour
{
    public static LevelController Instance;

    public Vector2 ColOneNoteCreation;
    public Vector2 ColTwoNoteCreation;
    public Vector2 ColThreeNoteCreation;
    public Vector2 ColFourNoteCreation;
    public float NoteDeletionY;

    [Space]

    [SerializeField] private float _notePreviewWidth = 0.5f;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(ColOneNoteCreation, _notePreviewWidth);
        Gizmos.DrawSphere(ColTwoNoteCreation, _notePreviewWidth);
        Gizmos.DrawSphere(ColThreeNoteCreation, _notePreviewWidth);
        Gizmos.DrawSphere(ColFourNoteCreation, _notePreviewWidth);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector3(ColOneNoteCreation.x, NoteDeletionY, 0), _notePreviewWidth);
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
