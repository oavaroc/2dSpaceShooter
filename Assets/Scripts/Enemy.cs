using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3f;
    [SerializeField]
    private bool _isBoss = false;
    [SerializeField]
    private Transform[] _bossTeleportLocations;
    [SerializeField]
    private GameObject _bossIllusion;

    [SerializeField]
    private bool _zigzag = false;
    private bool _zigzagRedirect = false;
    private bool _zigzagRight = false;
    private float _zigzagDuration = 0f;
    private float _zigzagLastMove = 0f;

    [SerializeField]
    private int _scoreWorth = 1;
    [SerializeField]
    private int _enemyHealth = 1;
    [SerializeField]
    private int _enemyHealthMax = 1;
    private Player _player;

    private Animator _animator;
    [SerializeField]
    private AudioSource _explosionSound;

    [SerializeField]
    private AudioSource _laserSound;

    [SerializeField]
    private GameObject _laser;

    [SerializeField]
    private bool _keepFiring = true;
    private GameObject _laserParent;
    [SerializeField]
    private GameObject _laserSpawnPoint;
    private Collider2D _collider;

    [SerializeField]
    private bool _beamEnemy=false;
    [SerializeField]
    private GameObject _beam;
    [SerializeField]
    private int _weight;

    [SerializeField]
    private bool _shieldActive = false;

    [SerializeField]
    private GameObject _enemyShield;
    [SerializeField]
    private int _shieldHits = 1;
    private int _shieldHitsLeft;

    [SerializeField]
    private bool _aggressive = false;
    [SerializeField]
    private bool _smart = false;
    [SerializeField]
    private bool _evasive=false;
    private Vector3 _evasiveDirection = Vector3.down;
    private GameObject _enemyParent;

    private UIManager _uIManager;
    private Coroutine _bossIllusionCoroutine;

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if(_player == null)
        {
            Debug.Log("Enemy unable to find player!");
        }
        _laserParent = GameObject.Find("Lasers");
        if (_laserParent == null)
        {
            Debug.Log("Enemy unable to find Lasers!");
        }

        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.Log("Enemy unable to find animator!");
        }
        _explosionSound = GameObject.Find("ExplosionSound").GetComponent<AudioSource>();
        if (_explosionSound == null)
        {
            Debug.Log("Cannot find the ExplosionSound component");
        }
        _laserSound = GameObject.Find("LaserSound").GetComponent<AudioSource>();
        if (_laserSound == null)
        {
            Debug.Log("Cannot find the LaserSound component");
        }

        _collider = gameObject.GetComponent<Collider2D>();
        if(_collider == null)
        {
            Debug.Log("Cannot find collider component");
        }
        if (Random.Range(0, 10f) <= 1 && !_isBoss && !_aggressive && !CompareTag("Illusion"))
        {
            _shieldActive = true;
        }
        if (_shieldActive)
        {
            _shieldHitsLeft = _shieldHits;
            _enemyShield.SetActive(true);
        }
        if (!_aggressive )
        {
            StartCoroutine(FireLaser());

        }
        if (_isBoss)
        {
            _uIManager = GameObject.Find("UI").GetComponent<UIManager>();
            if (_uIManager == null)
            {
                Debug.Log("Cannot find the UIManager component");
            }
            else
            {
                _uIManager.ActivateBossHealthBar();
                _uIManager.UpdateBossHealth((float)_enemyHealth / _enemyHealthMax);
            }
            _bossTeleportLocations = GameObject.Find("BossPositions").GetComponentsInChildren<Transform>();
            if (_bossTeleportLocations == null)
            {
                Debug.Log("Enemy unable to find BossPositions object!");
            }
            _enemyParent = GameObject.Find("Enemies");
            if (_enemyParent == null)
            {
                Debug.Log("Enemy unable to find parent object!");
            }
            _bossIllusionCoroutine= StartCoroutine(BossIllusions());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isBoss)
        {
            RegularMove();

        }
    }
    //Boss moves to center screen
    //after boss is attacked, moves to predefined locations
    //every 20s (quicker as the boss is damaged) the boss will create illusions of itself
    //the illusions will fire fake lasers
    //the illusions will disappear after 1 hit and after the real boss has been attacked
    //possible animation added to boss teleporting (just fade in/out)?
    //have a boss health bar on screen when active?
    IEnumerator BossIllusions()
    {
        while (_keepFiring)
        {
            yield return new WaitForSeconds(10f);
            if(_enemyParent.transform.childCount > 1)
            {
                _enemyHealth = Mathf.Clamp(_enemyHealth+ _enemyParent.transform.childCount - 1,0,_enemyHealthMax);
                _uIManager.UpdateBossHealth(_enemyHealth / (float)_enemyHealthMax);
                foreach (Enemy enemy in _enemyParent.GetComponentsInChildren<Enemy>())
                {
                    if (enemy.CompareTag("Illusion"))
                    {
                        enemy.DestroyEnemy();
                    }
                }

            }
            int bossPlacement = Random.Range(0, _bossTeleportLocations.Length);
            foreach(Transform location in _bossTeleportLocations)
            {
                if (_bossTeleportLocations[bossPlacement] == location)
                {
                    transform.position = location.position;
                }
                else
                {
                    Instantiate(_bossIllusion,location.position, Quaternion.identity, _enemyParent.transform);
                }
            }

        }
    }

    void RegularMove()
    {
        if(_aggressive)
        {
            if((_player.transform.position - transform.position).sqrMagnitude <= 10f && !CompareTag("Dead"))
            {
                //AngleTowardsTargetPosition(_player.gameObject);
                transform.up = -(_player.transform.position - transform.position);
                _speed = 11f;

            }
            transform.Translate(-Vector3.up * _speed * Time.deltaTime);
            if (transform.position.y < -6)
            {
                transform.position = new Vector3(Random.Range(-10f, 10f), 9, 0);
            }

        }
        else if (_zigzag)
        {
            if (_zigzagRedirect)
            {
                _zigzagRight = !_zigzagRight;
                _zigzagDuration = Random.Range(1f, 3f);
                _zigzagRedirect = false;
                _zigzagLastMove = Time.time;
            }
            if (Time.time > _zigzagLastMove + _zigzagDuration)
            {
                _zigzagRedirect = true;
            }
            else
            {
                transform.Translate((_zigzagRight ? Vector3.right : Vector3.left) * Time.deltaTime * _speed * 2);
            }
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }
        else if (_evasive)
        {
            transform.Translate( _evasiveDirection.normalized * _speed * Time.deltaTime);
            if (transform.position.y < -6)
            {
                transform.position = new Vector3(Random.Range(-10f, 10f), 9, 0);
            }
            _evasiveDirection = Vector3.MoveTowards(_evasiveDirection, Vector3.down,  Time.deltaTime);
        }
        else
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
            if(transform.position.y < -6)
            {
                transform.position = new Vector3(Random.Range(-10f,10f),9,0);
            }

        }
        transform.position = new Vector3(Mathf.Abs(transform.position.x) > 10 ? Mathf.Clamp(transform.position.x * -1, -10f, 10f) : transform.position.x,
                                            transform.position.y < -6 ? 9 : transform.position.y,
                                            0);
    }
    private void AngleTowardsTargetPosition(GameObject target)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation,
            Quaternion.AngleAxis(
                (Mathf.Atan2(
                    (transform.position.y - target.transform.position.y),
                    (transform.position.x - target.transform.position.x)) * Mathf.Rad2Deg) + 90f,
                Vector3.forward), _speed * Time.deltaTime *2);

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser")|| other.CompareTag("PlayerBeam"))
        {
            if (_shieldActive)
            {
                _shieldHitsLeft--;
                if (_shieldHitsLeft <= 0)
                {
                    _shieldActive = false;
                    _enemyShield.SetActive(false);

                }

            }
            else
            {
                Debug.Log("Enemy hit by: " + other.transform.name);
                if (other.CompareTag("Laser"))
                {
                    Destroy(other.gameObject);
                }
                CalculateDamage();

            }
            if (_isBoss)
            {
                _uIManager.UpdateBossHealth(_enemyHealth/(float)_enemyHealthMax);
                foreach (Enemy enemy in _enemyParent.GetComponentsInChildren<Enemy>())
                {
                    if (enemy.CompareTag("Illusion"))
                    {
                        enemy.DestroyEnemy();
                        Debug.Log("Destroying illusion: " + enemy.name);

                    }
                }
            }
        }
    }

    IEnumerator Relocate()
    {
        _animator.SetTrigger("Hit");
        _collider.enabled = false;
        yield return new WaitForSeconds(0.2f);
        transform.position = _bossTeleportLocations[Random.Range(0, _bossTeleportLocations.Length)].position;
        _collider.enabled = true;

    }

    private void DestroyEnemy()
    {
        if (_shieldActive)
        {
            _shieldHitsLeft--;
            Debug.Log("Removing 1 shield hit from enemy");
            if (_shieldHitsLeft <= 0)
            {
                _shieldActive = false;
                _enemyShield.SetActive(false);

            }
        }
        else
        {
            if (_beamEnemy)
            {
                _beam.gameObject.SetActive(false);
            }
            if (CompareTag("Illusion"))
            {
                Debug.Log("Destroying illusion: " + gameObject.name);
                _keepFiring = false;
                _animator.SetTrigger("Hit");
                _collider.enabled = false;
                _speed = 0f;
                Destroy(gameObject, 0.4f);

            }
            else
            {
                Debug.Log("Destroying enemy: " + gameObject.name);
                _player.AddScore(_scoreWorth);
                transform.tag = "Dead";
                _keepFiring = false;
                _animator.SetTrigger("Dead");
                _collider.enabled = false;
                _speed = 0f;
                _explosionSound.Play();
                Destroy(gameObject, 2.38f);

            }

        }

    }

    public void CalculateDamage()
    {
        _enemyHealth--;
        if (_enemyHealth <= 0)
        {
            DestroyEnemy();
        }
        if (_isBoss && _enemyHealth >= 1)
        {
            StartCoroutine(Relocate());
        }else if (_isBoss && _enemyHealth <= 0)
        {

            StopCoroutine(_bossIllusionCoroutine);
        }
    }
    public int GetWeight()
    {
        return _weight;
    }

    public void EvasiveManeuvers(Vector3 direction)
    {
        _evasiveDirection = (direction+_evasiveDirection).normalized;
        Debug.Log(_evasiveDirection);
    }

    IEnumerator FireLaser()
    {
        if (_isBoss)
        {
            yield return new WaitForSeconds(3f);
        }
        while (_keepFiring)
        {
            yield return new WaitForSeconds(_smart?1:Random.Range(1,3));
            if (_keepFiring)
            {
                if (_beamEnemy)
                {
                    yield return new WaitForSeconds(1);
                    _beam.gameObject.SetActive(true);
                    yield return new WaitForSeconds(1);
                    _beam.gameObject.SetActive(false);
                    yield return new WaitForSeconds(1);//inc52233705

                }
                else
                {
                    if (_smart)
                    {
                        if (transform.position.y < (_player.transform.position.y - 1f))
                            Fire();
                    }
                    else
                    {
                        Fire();
                    }

                }
            }
        }
    }

    public void Fire()
    {
        Instantiate(_laser, _laserSpawnPoint.transform.position, Quaternion.identity, _laserParent.transform);
        _laserSound.Play();
        Debug.Log("Enemy Firing Laser");

    }
}
