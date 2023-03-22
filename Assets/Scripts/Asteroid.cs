using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _rotationalSpeed = 1f;

    [SerializeField]
    private GameObject _explosion;

    private SpawnManager _spawnManager;

    private AudioSource _explosionSound;

    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.Log("Asteroid cannot find spawn manager!");
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
        transform.Rotate(new Vector3(0, 0, 1) * Time.deltaTime * _rotationalSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Laser"))
        {
            _spawnManager.StartSpawning();
            DestroyAsteroid();
        }
    }
    private void DestroyAsteroid()
    {
        _explosionSound.Play();
        gameObject.GetComponent<CircleCollider2D>().enabled = false;
        Destroy(Instantiate(_explosion, transform.position, Quaternion.identity), 3f);
        Destroy(gameObject, 1f);
    }
}
