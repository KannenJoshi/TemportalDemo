using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{
    // Start is called before the first frame update
    public static int Score { get; private set; }

    [SerializeField] private float scoreIncrementDelay = 1f;
    [SerializeField] private int scoreIncrementAmount = 1;
    
    //https://answers.unity.com/questions/1534931/how-to-increase-value-of-for-example-gold-every-se.html
    private float _timer;
    
    private void Awake()
    {
        Score = 0;
    }

    // Update is called once per frame
    private void Update()
    {
        if (PauseMenu.IsPaused || GameOverMenu.IsGameOver) return;

        _timer += Time.unscaledDeltaTime;

        if (_timer >= scoreIncrementDelay)
        {
            Score += scoreIncrementAmount;
            _timer = 0f;
        }
    }

    public static void AddScore(int points)
    {
        Score += points;
    }
}
