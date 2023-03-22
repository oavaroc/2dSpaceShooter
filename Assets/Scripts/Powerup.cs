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
                other.GetComponent<Player>().SetPowerupEnabled(this);
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
