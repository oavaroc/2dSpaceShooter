using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3f;

    private enum PowerUpType
    {
        TripleShotPowerup, SpeedPowerUp, ShieldPowerUp
    }

    [SerializeField]
    private PowerUpType _powerUpName;

    [SerializeField]
    private float _powerUpDuration = 5f;
    private Player _player;
    private void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if(_player == null)
        {
            Debug.Log("Player is null in Power up object!");
        }
    }
    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
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
                    default:
                        Debug.Log("Unknown powerup");
                        break;
                }

                Destroy(transform.gameObject);
            }
        }
    }

    public float GetDuration()
    {
        return _powerUpDuration;
    }
    public string GetPowerUpName()
    {
        return _powerUpName.ToString();
    }
}
