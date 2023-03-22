using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    private TMP_Text _scoreText;

    [SerializeField]
    private TMP_Text _gameOverText;
    [SerializeField]
    private TMP_Text _restartText;

    [SerializeField]
    private Texture[] _livesRemaining;
    private RawImage _livesDisplay;

    // Start is called before the first frame update
    void Start()
    {

        _scoreText = GameObject.Find("Score").GetComponent<TMP_Text>();
        if (_scoreText == null)
        {
            Debug.Log("Cannot find Score Text component!");
        }

        _livesDisplay = GameObject.Find("LivesDisplay").GetComponent<RawImage>();
        if (_livesDisplay == null)
        {
            Debug.Log("Cannot find Lives Image component!");
        }

    }
    void Update()
    {
        if (_restartText.IsActive() && Input.GetKeyDown(KeyCode.R))
        {
            Restart();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void UpdateScore(int score)
    {
        _scoreText.text = "Score: " + score;
    }

    public void UpdateLives(int life)
    {
        _livesDisplay.texture = _livesRemaining[life];
        if(life <= 0)
        {
            StartCoroutine(Flicker());
            _restartText.gameObject.SetActive(true);
        }
    }

    IEnumerator Flicker()
    {
        _gameOverText.gameObject.SetActive(!_gameOverText.gameObject.activeInHierarchy);
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(Flicker());
    }
}
