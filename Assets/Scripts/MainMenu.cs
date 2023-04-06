using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _howToPlay;
    [SerializeField]
    private GameObject _titleScreen;
    public void LoadScene()
    {
        SceneManager.LoadScene(1);
    }
    public void ToggleHowToPlay()
    {
        _howToPlay.SetActive(!_howToPlay.activeInHierarchy);
        _titleScreen.SetActive(!_titleScreen.activeInHierarchy);
    }
}
