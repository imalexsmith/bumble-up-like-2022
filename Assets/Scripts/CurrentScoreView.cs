using UnityEngine;
using UniRx;
using TMPro;


public class CurrentScoreView : MonoBehaviour
{
    // ==============================================================================
    public TMP_Text TargetText;

    private ScoreCounter _scoreCounter;


    // ==============================================================================
    protected void Start()
    {
        _scoreCounter = FindObjectOfType<ScoreCounter>();
        _scoreCounter.CurrentScore.Subscribe(x => TargetText.text = x.ToString()).AddTo(gameObject);
    }
}