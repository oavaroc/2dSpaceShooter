using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private Text _livesText;

    // Start is called before the first frame update
    void Start()
    {
        _livesText = GameObject.Find("Lives").GetComponent<Text>();
        if (_livesText == null)
        {
            Debug.Log("Cannot find Lives Text component!");
        }
    }

    public void UpdateLives(int life)
    {
        _livesText.text = "Lives: "+life;
    }
}
