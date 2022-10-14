using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UniRx;


public class ScoreCounter : MonoBehaviour
{
    public struct NameAndScore
    {
        public string Name { get; }
        public int Score { get; }

        public NameAndScore(string name, int score)
        {
            Name = name;
            Score = score;
        }
    }


    // ==============================================================================
    public IntReactiveProperty CurrentScore;
    public int MaxBestScore = 10;

    public int CurrentPlayerPlace { get; private set; } = 9999;
    public ReadOnlyCollection<NameAndScore> BestScores { get; private set; }

    private Player _player;
    private List<NameAndScore> _scores = new List<NameAndScore>();


    // ==============================================================================
    public void SaveScore(string playerName)
    {
        if (CurrentPlayerPlace < MaxBestScore)
            _scores.Insert(CurrentPlayerPlace, new NameAndScore(playerName, CurrentScore.Value));

        for (int i = 0; i < MaxBestScore; i++)
        {
            PlayerPrefs.SetString($"name_{i}", _scores[i].Name);
            PlayerPrefs.SetInt($"score_{i}", _scores[i].Score);
        }

        PlayerPrefs.Save();
    }

    protected void Start()
    {
        for (int i = 0; i < MaxBestScore; i++)
        {
            var ns = new NameAndScore(PlayerPrefs.GetString($"name_{i}", "NoName"), PlayerPrefs.GetInt($"score_{i}", 0));
            _scores.Add(ns);
        }

        _scores = _scores.OrderByDescending(x => x.Score).ToList();
        BestScores = _scores.AsReadOnly();

        _player = FindObjectOfType<Player>();
        _player.OnMoveForward += ScoreUp;
    }

    protected void OnDestroy()
    {
        if (_player)
            _player.OnMoveForward -= ScoreUp;
    }

    private void ScoreUp()
    {
        CurrentScore.Value++;

        for (int i = 0; i < MaxBestScore; i++)
        {
            if (CurrentScore.Value > _scores[i].Score)
            {
                CurrentPlayerPlace = i;
                return;
            }
        }
    }
}