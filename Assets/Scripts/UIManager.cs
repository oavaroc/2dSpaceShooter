using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private TMP_Text _livesText;
    private TMP_Text _scoreText;

    // Start is called before the first frame update
    void Start()
    {
        _livesText = GameObject.Find("Lives").GetComponent<TMP_Text>();
        if (_livesText == null)
        {
            Debug.Log("Cannot find Lives Text component!");
        }

        _scoreText = GameObject.Find("Score").GetComponent<TMP_Text>();
        if (_scoreText == null)
        {
            Debug.Log("Cannot find Lives Text component!");
        }
    }

    public void UpdateLives(int life)
    {
        _livesText.text = "Lives: "+life;
    }
    public void UpdateScore(int score)
    {
        _scoreText.text = "Score: " + score;
    }
}
