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
    private GameObject _target;

    [SerializeField]
    private float _raycastRange;

    [SerializeField]
    private LayerMask _layerMask;
    private void Start()
    {
        if (_enemyLaser&& _homing)
        {
            _player = GameObject.Find("Player");
            if (_player == null)
            {
                Debug.Log("Enemy unable to find player!");
            }
            AngleTowardsTargetPosition(_player);
        }
        if (!_enemyLaser&& _homing)
        {
            _enemyParent = GameObject.Find("Enemies");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        if (!_enemyLaser)
        {
            WarnEvasiveEnemy();

        }
    }

    private void WarnEvasiveEnemy()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, _raycastRange, _layerMask);
        Debug.DrawRay(transform.position,transform.up);

        if(hit.collider != null)
        {
            Debug.Log(hit.collider.tag);
            if (hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("Incoming Laser!");
                Enemy enemy = hit.collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    //Tell the enemy to move towards the perpendicular vector to avoid the laser
                    Vector3 laserDirection = new Vector3(hit.point.x,hit.point.y) - transform.position;
                    Vector3 moveTo = hit.collider.transform.position - new Vector3(-laserDirection.y, laserDirection.x, 0);
                    enemy.EvasiveManeuvers(moveTo);
                }
                else
                {
                    Debug.Log("Enemy not found for evasive maneuver");
                }
            }

        }
    }

    private void Move()
    {
        if (!_enemyLaser && _homing)
        {
            if ((_target==null || _target.CompareTag("Dead") ) && _enemyParent != null && _enemyParent.transform.childCount>0)
            {
                _target = FindClosestTarget();
            }
            if (_target != null)
            {
                HomingRotation(_target);
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
        if (_enemyLaser&&!_homing)
        {
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
            if (Mathf.Abs(transform.position.y) > 7 || Mathf.Abs(transform.position.x) > 11)
            {
                Destroy(gameObject);
            }

        }
        else
        {
            transform.Translate(Vector3.up * _speed * Time.deltaTime);
            if(Mathf.Abs(transform.position.y) > 7 || Mathf.Abs(transform.position.x) > 11)
            {
                Destroy(gameObject);
            }

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
