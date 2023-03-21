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

    void Start()
    {
        _player = GameObject.Find("Player").GetComponent<Player>();
        if(_player == null)
        {
            Debug.Log("Enemy unable to find player!");
        }

        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.Log("Enemy unable to find animator!");
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
            _player.addScore(_scoreWorth);
            Destroy(other.gameObject);
            DestroyEnemy();
        }
    }

    public void DestroyEnemy()
    {
        _animator.SetTrigger("Dead");
        gameObject.GetComponent<BoxCollider2D>().enabled = false;
        _speed = 0f;
        Destroy(this.gameObject, 2.38f);
    }
}
