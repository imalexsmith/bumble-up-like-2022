using UnityEngine;
using TMPro;


public class RestartView : MonoBehaviour
{
    // ==============================================================================
    public TMP_InputField PlayerName;
    public NameAndScoreView NameAndScoreTemplate;
    public GameObject[] ActivateThese;

    private Player _player;
    private ScoreCounter _scoreCounter;


    // ==============================================================================
    public void SaveScore()
    {
        _scoreCounter.SaveScore(PlayerName.text);
    }

    protected void Awake()
    {
        foreach (var item in ActivateThese)
            item.SetActive(false);

        NameAndScoreTemplate.gameObject.SetActive(false);
    }

    protected void Start()
    {
        _scoreCounter = FindObjectOfType<ScoreCounter>();
        _player = FindObjectOfType<Player>();
        _player.OnKilled += PlayerKilled;
    }

    protected void OnDestroy()
    {
        if (_player)
            _player.OnKilled -= PlayerKilled;
    }

    private void PlayerKilled()
    {
        foreach (var item in ActivateThese)
            item.SetActive(true);

        foreach (var item in _scoreCounter.BestScores)
        {
            var ns = Instantiate(NameAndScoreTemplate, NameAndScoreTemplate.transform.parent);
            ns.NameText.text = item.Name;
            ns.ScoreText.text = item.Score.ToString();
            ns.gameObject.SetActive(true);
        }
    }
}