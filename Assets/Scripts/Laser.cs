using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 10f;
    [SerializeField]
    private bool _homing = false;
    [SerializeField]
    private bool _enemyLaser = false;
    private GameObject _player;

    private GameObject _enemyParent;
    private GameObject target;
    

    private void Start()
    {
        if (_enemyLaser)
        {
            _player = GameObject.Find("Player");
            if (_player == null)
            {
                Debug.Log("Enemy unable to find player!");
            }
            AngleTowardsTargetPosition(_player);
        }
        if (_homing)
        {
            _enemyParent = GameObject.Find("Enemies");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        if (_homing)
        {
            if ((target==null || target.CompareTag("Dead") ) && _enemyParent.transform.childCount>0)
            {
                target = FindClosestTarget();
            }
            if (target != null)
            {
                HomingRotation(target);
            }
            else
            {
                Debug.Log("No eligible targets on the map");
            }
        }
        RegularMovement();
    }
    private void HomingRotation(GameObject target)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, 
            Quaternion.AngleAxis(
                (Mathf.Atan2(
                    (transform.position.y - target.transform.position.y),
                    (transform.position.x - target.transform.position.x)) * Mathf.Rad2Deg) + 90f,
                Vector3.forward),_speed*Time.deltaTime/4);
    }
    private void AngleTowardsTargetPosition(GameObject target)
    {
        transform.SetPositionAndRotation(transform.position,
            Quaternion.AngleAxis(
                (Mathf.Atan2(
                    (transform.position.y - target.transform.position.y),
                    (transform.position.x - target.transform.position.x)) * Mathf.Rad2Deg) + 90f,
                Vector3.forward));

    }
    private void RegularMovement()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
        if(Mathf.Abs(transform.position.y) > 7 || Mathf.Abs(transform.position.x) > 11)
        {
            Destroy(gameObject);
        }

    }

    private GameObject FindClosestTarget()
    {
        float distance = Mathf.Infinity;
        Enemy closestEnemy = null;

        foreach(Enemy enemy in _enemyParent.GetComponentsInChildren<Enemy>())
        {
            float distanceToCurrent = (enemy.transform.position - transform.position).sqrMagnitude;
            if(distanceToCurrent < distance && !enemy.CompareTag("Dead"))
            {
                distance = distanceToCurrent;
                closestEnemy = enemy;
            }
        }
        return closestEnemy!=null ? closestEnemy.gameObject : null;
    }

}
