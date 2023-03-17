using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float shootCoolDown = 0.1f;
    private float lastShot = 0f;

    [SerializeField]
    private GameObject _laserParent;
    [SerializeField]
    private GameObject _laser;
    [SerializeField]
    private GameObject _laserSpawnPos;

    [SerializeField]
    private int lives = 3;
    private UIManager uIManager;


    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        uIManager = GameObject.Find("UI").GetComponent<UIManager>();
        if(uIManager == null)
        {
            Debug.Log("Cannot find the UIManager component");
        }
        else
        {
            uIManager.UpdateLives(lives);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Shoot();
        
    }

    private void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > lastShot+shootCoolDown)
        {
            Instantiate(_laser, _laserSpawnPos.transform.position, Quaternion.identity, _laserParent.transform);
            lastShot = Time.time;
        }
    }

    //Move the player
    private void Move()
    {
        transform.Translate(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"),0) * speed * Time.deltaTime);
        transform.position = new Vector3(Mathf.Abs(transform.position.x) > 10 ? transform.position.x * -1 : transform.position.x , 
                                        Mathf.Clamp(transform.position.y, -4, 2), 
                                        0);
    }

    //Determine horizontal movement
    private float MoveHorizontal()
    {
        switch (Input.GetAxis("Horizontal"))
        {
            case >0:
                return transform.position.x > 10 ? 0 : Input.GetAxis("Horizontal");
            case < 0:
                return transform.position.x < -10 ? 0 : Input.GetAxis("Horizontal");
            case 0:
            default:
                return 0;
        }
    }

    //Determine vertical movement
    private float MoveVertical()
    {
        switch (Input.GetAxis("Vertical"))
        {
            case >0:
                return Input.GetAxis("Vertical");
                //return transform.position.y > 2 ? 0 : Input.GetAxis("Vertical");
            case <0:
                //return transform.position.y < -4 ? 0 : Input.GetAxis("Vertical");
            case 0:
            default:
                return 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Player hit by: " + other.transform.name);
            Destroy(other.gameObject);
            Damage();
        }
    }

    private void Damage()
    {
        uIManager.UpdateLives(--lives);
        if(lives <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}