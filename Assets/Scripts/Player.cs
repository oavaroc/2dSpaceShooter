using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    [SerializeField]
    private float _speed = 5f;
    [SerializeField]
    private float _thrusterSpeed = 10f;

    [SerializeField]
    private float _shootCoolDown = 0.1f;
    private float _lastShot = 0f;

    [SerializeField]
    private bool _tripleShotActive = false;
    [SerializeField]
    private bool _speedPowerActive = false;
    [SerializeField]
    private bool _shieldActive = false;

    [SerializeField]
    private int _shieldHits = 3;
    private int _shieldHitsLeft;

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
    private GameObject[] _playerHurt;

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
        if (Input.GetKey(KeyCode.LeftShift))
        {
            CalculateMovement(_thrusterSpeed);
        }
        else
        {
            CalculateMovement(_speed);
        }    
        transform.position = new Vector3(Mathf.Abs(transform.position.x) > 10 ? Mathf.Clamp(transform.position.x * -1, -10f, 10f) : transform.position.x , 
                                            Mathf.Clamp(transform.position.y, -4, 2), 
                                            0);
    }

    private void CalculateMovement(float chosenSpeed)
    {
        transform.Translate(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) 
                                        * (_speedPowerActive ? chosenSpeed * 2 : chosenSpeed) * Time.deltaTime);
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
            ShieldDamage();
        }
        else
        {
            Damage();
        }
    }

    private void ShieldDamage()
    {
        _shieldHitsLeft--;
        Debug.Log("Shield hit Used, remaining hits: "+ _shieldHitsLeft);
        if(_shieldHitsLeft <=0)
        {
            Debug.Log("Shield gone");
            _shieldActive = false;
            _playerShield.SetActive(false);
            StopCoroutine(_shieldCoroutine);
            _shieldCoroutine = null;
        }
        else
        {
            SetShieldOpacity((float)_shieldHitsLeft/_shieldHits);
        }
    }
    private void SetShieldOpacity(float opacity)
    {
        Color color = _playerShield.GetComponent<SpriteRenderer>().material.color;
        color.a = opacity;
        _playerShield.GetComponent<SpriteRenderer>().material.color = color;
    }

    private void Damage()
    {
        _uIManager.UpdateLives(--_lives);
        ApplyVisualDamage();

        if (_lives <= 0)
        {
            _explosionSound.Play();
            Destroy(Instantiate(_explosion, transform.position, Quaternion.identity),3f);
            _spawnManager.StopSpawning();
            Destroy(this.gameObject);
        }
    }

    private void ApplyVisualDamage()
    {
        int i = Random.Range(0, _playerHurt.Length);
        while (_playerHurt[i].activeInHierarchy)
        {
            i = Random.Range(0, _playerHurt.Length);
        }
        _playerHurt[i].SetActive(true);
    }

    public void ActivateTripleShotSequence(float duration)
    {
        CheckCoroutine(_tripleShotCoroutine);
        _tripleShotCoroutine = StartCoroutine(TripleShot(duration));
    }
    public void ActivateSpeedSequence(float duration)
    {
        CheckCoroutine(_speedPowerCoroutine);
        _speedPowerCoroutine = StartCoroutine(SpeedBoost(duration));
    }
    public void ActivateShieldSequence(float duration)
    {
        CheckCoroutine(_shieldCoroutine);
        _shieldCoroutine = StartCoroutine(Shield(duration));
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

    IEnumerator TripleShot(float duration)
    {
        Debug.Log("Triple Shot Enabled");
        _tripleShotActive = true;
        yield return new WaitForSeconds(duration);
        _tripleShotCoroutine = null;
        _tripleShotActive = false;
        Debug.Log("Triple Shot Disabled");
    }

    IEnumerator SpeedBoost(float duration)
    {
        Debug.Log("Speed Enabled");
        _speedPowerActive = true;
        yield return new WaitForSeconds(duration);
        _speedPowerCoroutine = null;
        _speedPowerActive = false;
        Debug.Log("Speed Disabled");
    }

    IEnumerator Shield(float duration)
    {
        Debug.Log("Shield Enabled");
        _shieldHitsLeft = _shieldHits;
        SetShieldOpacity(1f);
        _shieldActive = true;
        _playerShield.SetActive(true);
        yield return new WaitForSeconds(duration);
        _shieldCoroutine = null;
        _shieldActive = false;
        _playerShield.SetActive(false);
        Debug.Log("Shield Disabled");
    }
}