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
    private bool _tripleShotActive = false;
    [SerializeField]
    private bool _speedPowerActive = false;
    [SerializeField]
    private bool _shieldActive = false;

    private Coroutine _tripleShotCoroutine;
    private Coroutine _speedPowerCoroutine;
    private Coroutine _shieldCoroutine;

    [SerializeField]
    private GameObject _tripleShot;

    [SerializeField]
    private GameObject _playerShield;


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

    private int _score = 0;

    [SerializeField]
    private List<GameObject> _playerHurt;

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
            _uIManager.UpdateScore(_score);
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
            Instantiate(_tripleShotActive ? _tripleShot : _laser, 
                        _laserSpawnPos.transform.position, Quaternion.identity, _laserParent.transform);
            _lastShot = Time.time;
        }
    }

    //Move the player
    private void Move()
    {
        transform.Translate(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"),0) * ( _speedPowerActive ? _speed * 2 : _speed) * Time.deltaTime);
        transform.position = new Vector3(Mathf.Abs(transform.position.x) > 10 ? Mathf.Clamp(transform.position.x * -1, -10f, 10f) : transform.position.x , 
                                        Mathf.Clamp(transform.position.y, -4, 2), 
                                        0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Player hit by: " + other.transform.name);
            other.GetComponent<Enemy>().DestroyEnemy();
            if (_shieldActive)
            {
                _shieldActive = false;
                _playerShield.SetActive(false);
                Debug.Log("Shield Used");
                StopCoroutine(_shieldCoroutine);
                _shieldCoroutine = null;
            }
            else
            {
                Damage();
            }
            
        }
        
    }

    private void Damage()
    {
        _uIManager.UpdateLives(--_lives);
        _playerHurt.FindAll(hurt => !hurt.gameObject.activeInHierarchy)
            [Random.Range(0, _playerHurt.FindAll(hurt => !hurt.gameObject.activeInHierarchy).Count)].SetActive(true);

        if (_lives <= 0)
        {
            _spawnManager.StopSpawning();
            Destroy(this.gameObject);
        }
    }
    public void setPowerupEnabled(Powerup powerup)
    {
        Debug.Log(powerup.GetPowerUpName() + " starting");
        switch (powerup.GetPowerUpName())
        {
            case "TripleShotPowerup":
                StartTripleShot(powerup);
                break;
            case "SpeedPowerUp":
                StartSpeed(powerup);
                break;
            case "ShieldPowerUp":
                StartShield(powerup);
                break;
            default:
                Debug.Log("Unknown powerup");
                break;
        }
    }

    private void StartTripleShot(Powerup powerup)
    {
        if (_tripleShotCoroutine == null)
        {
            _tripleShotCoroutine = StartCoroutine(TripleShotExpired(powerup.GetDuration()));
        }
        else
        {
            StopCoroutine(_tripleShotCoroutine);
            _tripleShotCoroutine = StartCoroutine(TripleShotExpired(powerup.GetDuration()));
        }
    }

    private void StartSpeed(Powerup powerup)
    {
        if (_speedPowerCoroutine == null)
        {
            _speedPowerCoroutine = StartCoroutine(SpeedExpired(powerup.GetDuration()));
        }
        else
        {
            StopCoroutine(_speedPowerCoroutine);
            _speedPowerCoroutine = StartCoroutine(SpeedExpired(powerup.GetDuration()));
        }
    }
    private void StartShield(Powerup powerup)
    {
        if (_shieldCoroutine == null)
        {
            _shieldCoroutine = StartCoroutine(ShieldExpired(powerup.GetDuration()));
        }
        else
        {
            StopCoroutine(_shieldCoroutine);
            _shieldCoroutine = StartCoroutine(ShieldExpired(powerup.GetDuration()));
        }
    }

    public void addScore(int score)
    {
        _score += score;
        _uIManager.UpdateScore(_score);
    }

    IEnumerator TripleShotExpired(float duration)
    {
        Debug.Log("Triple Shot Enabled");
        _tripleShotActive = true;
        yield return new WaitForSeconds(duration);
        _tripleShotCoroutine = null;
        _tripleShotActive = false;
        Debug.Log("Triple Shot Disabled");
    }

    IEnumerator SpeedExpired(float duration)
    {
        Debug.Log("Speed Enabled");
        _speedPowerActive = true;
        yield return new WaitForSeconds(duration);
        _speedPowerCoroutine = null;
        _speedPowerActive = false;
        Debug.Log("Speed Disabled");
    }

    IEnumerator ShieldExpired(float duration)
    {
        Debug.Log("Shield Enabled");
        _shieldActive = true;
        _playerShield.SetActive(true);
        yield return new WaitForSeconds(duration);
        _shieldCoroutine = null;
        _shieldActive = false;
        _playerShield.SetActive(false);
        Debug.Log("Shield Disabled");
    }
}