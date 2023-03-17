using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private Text livesText;

    // Start is called before the first frame update
    void Start()
    {
        livesText = GameObject.Find("Lives").GetComponent<Text>();
        if (livesText == null)
        {
            Debug.Log("Cannot find Lives Text component!");
        }
    }

    public void UpdateLives(int life)
    {
        livesText.text = "Lives: "+life;
    }
}
