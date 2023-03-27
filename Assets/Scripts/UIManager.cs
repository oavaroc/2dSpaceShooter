using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Slider _thrusterSlider;
    [SerializeField]
    private Image _thrusterFill;
    [SerializeField]
    private Gradient _gradient = null;
    [SerializeField]
    private TMP_Text _thrusterText;

    private TMP_Text _scoreText;

    [SerializeField]
    private TMP_Text _gameOverText;
    [SerializeField]
    private TMP_Text _restartText;

    [SerializeField]
    private Texture[] _livesRemaining;
    private RawImage _livesDisplay;

    private TMP_Text _ammoText;
    private bool _outOfAmmo = false;
    [SerializeField]
    private TMP_Text _outOfAmmoText;
    private Coroutine _ammoCoroutine;

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

        _ammoText = GameObject.Find("AmmoCount").GetComponent<TMP_Text>();
        if (_ammoText == null)
        {
            Debug.Log("Cannot find AmmoCount component!");
        }

    }

    public void UpdateScore(int score)
    {
        _scoreText.text = "Score: " + score;
    }
    public void UpdateAmmo(int ammo)
    {
        if(_ammoCoroutine != null && ammo > 0)
        {
            StopCoroutine(_ammoCoroutine);
            _outOfAmmo = false;
            _ammoCoroutine = null;
        }
        _ammoText.text = "Ammo: " + ammo;
        if(ammo <= 0)
        {
            _outOfAmmo = true;
            _ammoCoroutine = StartCoroutine(OutOfAmmoDisplay());
        }
    }

    public void UpdateLives(int life)
    {

        if(life <= 0)
        {
            StartCoroutine(Flicker());
            _restartText.gameObject.SetActive(true);
        }
        _livesDisplay.texture = _livesRemaining[Mathf.Clamp(life,0,3)];
    }
    public void UpdateThrustersDisplay(float thrusters, float maxThrusters, bool thrusterOverheated)
    {
        _thrusterSlider.value = thrusters;
        _thrusterFill.color=_gradient.Evaluate(thrusters/ maxThrusters);
        if (thrusterOverheated)
        {
            _thrusterText.text = "Thrusters: Overheated";

        }else if (thrusters < (maxThrusters/2))
        {
            _thrusterText.text = "Thrusters: Warning! Overheating";
        }
        else
        {
            _thrusterText.text = "Thrusters: Operational";
        }
    }

    IEnumerator Flicker()
    {
        _gameOverText.gameObject.SetActive(!_gameOverText.gameObject.activeInHierarchy);
        yield return new WaitForSeconds(0.2f);
        StartCoroutine(Flicker());
    }

    IEnumerator OutOfAmmoDisplay()
    {
        if (_outOfAmmo) { 
            _outOfAmmoText.gameObject.SetActive(!_outOfAmmoText.gameObject.activeInHierarchy);
            yield return new WaitForSeconds(0.2f);
            _ammoCoroutine = StartCoroutine(OutOfAmmoDisplay());
        }
    }
}
