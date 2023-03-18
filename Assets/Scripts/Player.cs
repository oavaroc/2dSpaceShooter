using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5f;

    [SerializeField]
    private float _shootCoolDown = 0.1f;
    private float _lastShot = 0f;

    [SerializeField]
    private GameObject _laserParent;
    [SerializeField]
    private GameObject _laser;
    [SerializeField]
    private GameObject _laserSpawnPos;

    [SerializeField]
    private int _lives = 3;
    private UIManager _uIManager;
    private SpawnManager _spawnManager;


    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);

        _uIManager = GameObject.Find("UI").GetComponent<UIManager>();
        if(_uIManager == null)
        {
            Debug.Log("Cannot find the UIManager component");
        }
        else
        {
            _uIManager.UpdateLives(_lives);
        }

        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.Log("Cannot find the SpawnManager component");
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
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _lastShot+_shootCoolDown)
        {
            Instantiate(_laser, _laserSpawnPos.transform.position, Quaternion.identity, _laserParent.transform);
            _lastShot = Time.time;
        }
    }

    //Move the player
    private void Move()
    {
        transform.Translate(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"),0) * _speed * Time.deltaTime);
        transform.position = new Vector3(Mathf.Abs(transform.position.x) > 10 ? transform.position.x * -1 : transform.position.x , 
                                        Mathf.Clamp(transform.position.y, -4, 2), 
                                        0);
    }

    private void OnTriggerEnter2D(Collider2D other)
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
        _uIManager.UpdateLives(--_lives);
        if(_lives <= 0)
        {
            _spawnManager.StopSpawning();
            Destroy(this.gameObject);
        }
    }
}