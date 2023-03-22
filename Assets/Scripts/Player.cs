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

    [SerializeField]
    private GameObject _explosion;

    private AudioSource _laserSound;
    private AudioSource _explosionSound;
    private AudioSource _powerUpSound;

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

        _laserSound = GameObject.Find("LaserSound").GetComponent<AudioSource>();
        if (_laserSound == null)
        {
            Debug.Log("Cannot find the LaserSound component");
        }

        _explosionSound = GameObject.Find("ExplosionSound").GetComponent<AudioSource>();
        if (_explosionSound == null)
        {
            Debug.Log("Cannot find the ExplosionSound component");
        }

        _powerUpSound = GameObject.Find("PowerUpSound").GetComponent<AudioSource>();
        if (_powerUpSound == null)
        {
            Debug.Log("Cannot find the PowerUpSound component");
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
            _laserSound.Play();
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
        Debug.Log("Player hit by: " + other.transform.name);
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().DestroyEnemy();
            CalculateDamage();
        }
        else if (other.CompareTag("EnemyLaser"))
        {
            Destroy(other.gameObject);
            CalculateDamage();
        }
        
    }

    private void CalculateDamage()
    {
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

    private void Damage()
    {
        _uIManager.UpdateLives(--_lives);
        _playerHurt.FindAll(hurt => !hurt.gameObject.activeInHierarchy)
            [Random.Range(0, _playerHurt.FindAll(hurt => !hurt.gameObject.activeInHierarchy).Count)].SetActive(true);

        if (_lives <= 0)
        {
            _explosionSound.Play();
            Destroy(Instantiate(_explosion, transform.position, Quaternion.identity),3f);
            _spawnManager.StopSpawning();
            Destroy(this.gameObject);
        }
    }
    public void SetPowerupEnabled(Powerup powerup)
    {
        _powerUpSound.Play();
        Debug.Log(powerup.GetPowerUpName() + " starting");
        switch (powerup.GetPowerUpName())
        {
            case "TripleShotPowerup":
                CheckCoroutine(_tripleShotCoroutine);
                _tripleShotCoroutine = StartCoroutine(TripleShotExpired(powerup.GetDuration()));
                break;
            case "SpeedPowerUp":
                CheckCoroutine(_speedPowerCoroutine);
                _speedPowerCoroutine = StartCoroutine(SpeedExpired(powerup.GetDuration()));
                break;
            case "ShieldPowerUp":
                CheckCoroutine(_shieldCoroutine);
                _shieldCoroutine = StartCoroutine(ShieldExpired(powerup.GetDuration()));
                break;
            default:
                Debug.Log("Unknown powerup");
                break;
        }
    }
    private void CheckCoroutine(Coroutine coroutine)
    {
        if (coroutine != null)
        {
            StopCoroutine(coroutine);
        }
    }

    public void AddScore(int score)
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