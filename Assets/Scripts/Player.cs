using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private MainCamera _mainCamera;

    [SerializeField]
    private float _speed = 5f;
    [SerializeField]
    private float _thrusterSpeed = 10f;
    [SerializeField]
    private float _maxThruster = 100f;
    [SerializeField]
    private float _thrusterCounter = 100f;
    [SerializeField]
    private float _thrusterUsageRate = 2f;
    [SerializeField]
    private float _thrusterRechargeRate = 1f;
    private bool _thrustersOverheated = false;

    [SerializeField]
    private float _shootCoolDown = 0.1f;
    [SerializeField]
    private int _ammoCount = 20;
    [SerializeField]
    private int _ammoMax = 20;
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
    private Coroutine _homingCoroutine;
    private Coroutine _slowCoroutine;
    [SerializeField]
    private GameObject _beam;
    [SerializeField]
    private GameObject _tripleShot;
    [SerializeField]
    private GameObject _HomingShot;
    [SerializeField]
    private GameObject _HomingTripleShot;
    [SerializeField]
    private bool _homingActive;
    private Coroutine _beamCoroutine;

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
    [SerializeField]
    private int _maxLives = 3;
    private UIManager _uIManager;
    private SpawnManager _spawnManager;

    private int _score = 0;

    [SerializeField]
    private GameObject[] _playerHurt;
    private Queue<GameObject> _playerHurtActive = new Queue<GameObject>();

    [SerializeField]
    private GameObject _explosion;

    private AudioSource _laserSound;
    private AudioSource _explosionSound;

    [SerializeField]
    private bool _invulnerble=false;

    private bool _slowActive = false;

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
            _uIManager.UpdateAmmo(_ammoCount, _ammoMax);
            _uIManager.UpdateThrustersDisplay(_thrusterCounter,_maxThruster, _thrustersOverheated);
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

    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Shoot();
    }


    private void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _lastShot+_shootCoolDown && _ammoCount >= 1)
        {
            Instantiate(CalculateShot(), 
                        _laserSpawnPos.transform.position, Quaternion.identity, _laserParent.transform);
            _laserSound.Play();
            _lastShot = Time.time;
        }
    }

    private GameObject CalculateShot()
    {
        _uIManager.UpdateAmmo(--_ammoCount, _ammoMax);
        if(_homingActive && _tripleShotActive)
        {
            return _HomingTripleShot;
        }else if (_homingActive) 
        { 
            return _HomingShot; 
        }else if (_tripleShotActive) 
        { 
            return _tripleShot; 
        }
        return _laser;
    }

    //Move the player
    private void Move()
    {
        if (Input.GetKey(KeyCode.LeftShift) && _thrusterCounter > 0 && !_thrustersOverheated)
        {
            CalculateMovement(_slowActive ? _thrusterSpeed / 2 : _thrusterSpeed);
            UseThrusters();
        }
        else
        {
            CalculateMovement(_slowActive ? _speed / 2:_speed);
            RegenerateThrusters();
        }    
        transform.position = new Vector3(Mathf.Abs(transform.position.x) > 10 ? Mathf.Clamp(transform.position.x * -1, -10f, 10f) : transform.position.x , 
                                            Mathf.Clamp(transform.position.y, -4, 2), 
                                            0);
    }
    private void UseThrusters()
    {
        _thrusterCounter = Mathf.Clamp(_thrusterCounter - ( _thrusterUsageRate * Time.deltaTime), 0, _maxThruster);
        if (_thrusterCounter <= 10f)
        {
            _thrustersOverheated = true;
        }
        _uIManager.UpdateThrustersDisplay(_thrusterCounter,_maxThruster, _thrustersOverheated);
    }
    private void RegenerateThrusters()
    {
        _thrusterCounter = Mathf.Clamp(_thrusterCounter + ( _thrusterRechargeRate * Time.deltaTime), 0,  _maxThruster);
        if(_thrusterCounter > _maxThruster*0.75f)
        {
            _thrustersOverheated = false;
        }
        _uIManager.UpdateThrustersDisplay(_thrusterCounter,_maxThruster, _thrustersOverheated);
    }
    private void CalculateMovement(float chosenSpeed)
    {
        transform.Translate(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0) 
                                        * (_speedPowerActive ? chosenSpeed * 2 : chosenSpeed) * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_invulnerble)
            return;
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Player hit by: " + other.transform.name);
            other.GetComponent<Enemy>().CalculateDamage();
            CalculateDamage();
        }
        else if (other.CompareTag("EnemyLaser"))
        {
            Debug.Log("Player hit by: " + other.transform.name);
            Destroy(other.gameObject);
            CalculateDamage();
        }
        else if (other.CompareTag("EnemyBeam"))
        {
            Debug.Log("Player hit by: " + other.transform.name);
            CalculateDamage();
        }
        else if (other.CompareTag("Illusion"))
        {
            Debug.Log("Player hit by: " + other.transform.name);
            Destroy(other.gameObject);
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
            StartCoroutine(_mainCamera.ShakeCamera(0.1f, 0.05f));
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
        _playerHurtActive.Enqueue(_playerHurt[i]);
    }

    private void RemoveVisualDamage(int health)
    {
        for (int i = 0; i < health; i++)
        {
            if (_playerHurtActive.TryDequeue(out GameObject _playerHurtObj))
            {
                _playerHurtObj.SetActive(false);
            }
        }
    }
    public void ActivateBeamSequence(float duration)
    {
        CheckCoroutine(_beamCoroutine);
        _beamCoroutine = StartCoroutine(Beam(duration));
    }
    public void ActivateHomingSequence(float duration)
    {
        CheckCoroutine(_homingCoroutine);
        _homingCoroutine = StartCoroutine(Homing(duration));
    }
    public void ActivateSlowSequence(float slowDuration)
    {
        CheckCoroutine(_slowCoroutine);
        _slowCoroutine = StartCoroutine(Slow(slowDuration));
    }
    public void ActivateHealthSequence(int health)
    {
        if (_lives < _maxLives)
        {
            _lives += health;
            _lives = Mathf.Min(_lives + health, _maxLives);
            _uIManager.UpdateLives(_lives);
            RemoveVisualDamage(health);
        }
    }
    public void ActivateAmmoSequence(int ammo)
    {

        _ammoCount = Mathf.Clamp(_ammoCount + ammo, 0, _ammoMax);
        _uIManager.UpdateAmmo(_ammoCount, _ammoMax);
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
    IEnumerator Beam(float duration)
    {
        Debug.Log("Beam Enabled");
        _beam.gameObject.SetActive(true);
        yield return new WaitForSeconds(duration);
        _beam.gameObject.SetActive(false);
        Debug.Log("Beam Disabled");
    }
    IEnumerator Slow(float duration)
    {
        Debug.Log("Slow Enabled");
        _slowActive = true;
        yield return new WaitForSeconds(duration);
        _slowActive = false;
        Debug.Log("Slow Disabled");
    }
    IEnumerator Homing(float duration)
    {
        Debug.Log("Homing Enabled");
        _homingActive = true;
        yield return new WaitForSeconds(duration);
        _homingCoroutine = null;
        _homingActive = false;
        Debug.Log("Homing Disabled");
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