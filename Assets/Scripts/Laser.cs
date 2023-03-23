using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 10f;

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
        if(Mathf.Abs(transform.position.y) > 7 || Mathf.Abs(transform.position.x) > 11)
        {
            Destroy(gameObject);
        }
    }

}
