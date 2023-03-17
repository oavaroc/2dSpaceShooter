using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float speed = 3f;

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    void Move()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
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
            Destroy(other.gameObject);
            Destroy(this.gameObject);
        }
    }

}
