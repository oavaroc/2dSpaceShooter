using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    private TMP_Text _restartText;
    [SerializeField]
    private TMP_Text _endlessMode;
    [SerializeField]
    private SpawnManager _spawnManager;

    void Update()
    {
        if(_endlessMode.IsActive() && Input.GetKeyDown(KeyCode.E))
        {
            EndlessMode();
        }
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

    private void EndlessMode()
    {
        _spawnManager.StartSpawning();
        _endlessMode.gameObject.SetActive(false);
    }
}
