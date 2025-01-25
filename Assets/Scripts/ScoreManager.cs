using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [SerializeField] private int _maxComboMultiplier;
    [SerializeField] private AudioSource _hitSFX;
    [SerializeField] private AudioSource _missSFX;

    [Space]

    [SerializeField] private GameObject _comboPanel;
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private TextMeshProUGUI _comboText;

    private int _comboScore = 0;
    private int _comboMultiplier = 0;
    private int _score = 0;

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
        _comboPanel.SetActive(false);
    }

    public void Hit()
    {
        _comboScore += 1;
        _comboMultiplier = Mathf.Min(_comboScore, _maxComboMultiplier);
        _comboPanel.SetActive(true);
        _comboText.text = $"{_comboScore} (x{Mathf.Min(_comboMultiplier, _maxComboMultiplier)})";
        _score += _comboMultiplier;
        _scoreText.text = $"Score: {_score}";
        _hitSFX.Play();
    }

    public void Miss()
    {
        _comboScore = 0;
        _comboPanel.SetActive(false);
        _missSFX.Play();
    }
}
