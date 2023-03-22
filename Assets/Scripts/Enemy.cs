using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3f;

    [SerializeField]
    private int _scoreWorth = 1;
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
        StartCoroutine(FireLaser());
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
        if(transform.position.y < -6)
        {
            transform.position = new Vector3(Random.Range(-10f,10f),9,0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Laser"))
        {
            Debug.Log("Enemy hit by: " + other.transform.name);
            _player.AddScore(_scoreWorth);
            Destroy(other.gameObject);
            DestroyEnemy();
        }
    }

    public void DestroyEnemy()
    {
        _keepFiring = false;
        _animator.SetTrigger("Dead");
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        _speed = 0f;
        _explosionSound.Play();
        Destroy(this.gameObject, 2.38f);
    }

    IEnumerator FireLaser()
    {
        while (_keepFiring)
        {
            yield return new WaitForSeconds(Random.Range(1,3));
            if (_keepFiring)
            {
                GameObject laser = Instantiate(_laser, _laserSpawnPoint.transform.position, Quaternion.identity, _laserParent.transform);
                _laserSound.Play();
                Debug.Log("Enemy Firing Laser");
                laser.transform.SetPositionAndRotation(laser.transform.position, 
                    Quaternion.AngleAxis(
                        (Mathf.Atan2(
                            (laser.transform.position.y - _player.transform.position.y), 
                            (laser.transform.position.x - _player.transform.position.x)) * Mathf.Rad2Deg) + 90f, 
                        Vector3.forward));
            }
        }
    }
}
