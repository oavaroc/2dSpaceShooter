using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private float _spawnRate = 5f;

    [SerializeField]
    private GameObject _enemy;
    [SerializeField]
    private GameObject _enemyParent;

    [SerializeField]
    private bool _keepSpawning=true;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(_spawnRate);
        while (_keepSpawning) { 
            Instantiate(_enemy, new Vector3(Random.Range(-10, 10), 9, 0), Quaternion.identity, _enemyParent.transform);
            yield return new WaitForSeconds(_spawnRate);
        }
    }

    public void StopSpawning()
    {
        _keepSpawning = false;
        Destroy(_enemyParent);
    }
}
