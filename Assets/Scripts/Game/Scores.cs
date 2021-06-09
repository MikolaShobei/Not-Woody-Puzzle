using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scores : MonoBehaviour
{
    public Text scoreText;

    private int _currentScores;
    
    void Start()
    {
        _currentScores = 0;
        UpdateTextScore();
    }

    private void OnEnable() {
        GameEvents.AddScores += AddScores;
        
    }

    private void OnDisable() {
        GameEvents.AddScores -= AddScores;
    }

    private void AddScores(int scores)
    {
        _currentScores += scores;
        UpdateTextScore();
    }

    private void UpdateTextScore()
    {
        scoreText.text = _currentScores.ToString();
    }
}
