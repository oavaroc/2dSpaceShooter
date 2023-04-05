using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3f;

    private enum PowerUpType
    {
        TripleShotPowerup, SpeedPowerUp, ShieldPowerUp, AmmoPowerUp, HealthPowerUp, HomingPowerUp, SlowNPowerUp, BeamPowerUp
    }

    [SerializeField]
    private PowerUpType _powerUpName;

    [SerializeField]
    private float _powerUpDuration = 5f;
    [SerializeField]
    private int _powerUpAmount = 5;
    private Player _player;
    private AudioSource _powerUpSound;
    [SerializeField]
    private int _weight = 0;

    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if(_player == null)
        {
            Debug.Log("Player is null in Power up object!");
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
    }

    void Move()
    {
        if (Input.GetKey(KeyCode.C))
        {
            MoveTowardPlayer();
        }
        else
        {
            transform.Translate( Vector3.down * _speed * Time.deltaTime);
        }
        
        if (transform.position.y < -6)
        {
            Destroy(gameObject);
        }
    }

    private void MoveTowardPlayer()
    {
        transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, _speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.GetComponent<Player>() != null)
            {
                switch (_powerUpName.ToString())
                {
                    case "TripleShotPowerup":
                        Debug.Log(_powerUpName.ToString() + " starting");
                        _player.ActivateTripleShotSequence(_powerUpDuration);
                        break;
                    case "SpeedPowerUp":
                        Debug.Log(_powerUpName.ToString() + " starting");
                        _player.ActivateSpeedSequence(_powerUpDuration);
                        break;
                    case "ShieldPowerUp":
                        Debug.Log(_powerUpName.ToString() + " starting");
                        _player.ActivateShieldSequence(_powerUpDuration);
                        break;
                    case "AmmoPowerUp":
                        Debug.Log(_powerUpName.ToString() + " starting");
                        _player.ActivateAmmoSequence(_powerUpAmount);
                        break;
                    case "HealthPowerUp":
                        Debug.Log(_powerUpName.ToString() + " starting");
                        _player.ActivateHealthSequence(_powerUpAmount);
                        break;
                    case "HomingPowerUp":
                        Debug.Log(_powerUpName.ToString() + " starting");
                        _player.ActivateHomingSequence(_powerUpDuration);
                        break;
                    case "SlowNPowerUp":
                        Debug.Log(_powerUpName.ToString() + " starting");
                        _player.ActivateSlowSequence(_powerUpDuration);
                        break;
                    case "BeamPowerUp":
                        Debug.Log(_powerUpName.ToString() + " starting");
                        _player.ActivateBeamSequence(_powerUpDuration);
                        break;
                    default:
                        Debug.Log("Unknown powerup");
                        break;
                }
                _powerUpSound.Play();
                Destroy(transform.gameObject);
            }
        }
        if (other.CompareTag("EnemyLaser"))
        {
            Destroy(other.gameObject);
            Destroy(transform.gameObject);
        }
    }
    public int GetWeight()
    {
        return _weight;
    }
}
