using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private float _enemySpawnRate = 5f;
    [SerializeField]
    private float _powerUpSpawnRate = 5f;

    [SerializeField]
    private GameObject _enemy;
    [SerializeField]
    private GameObject _enemyParent;

    [SerializeField]
    private List<GameObject> _powerUpList;
    [SerializeField]
    private GameObject _powerUpParent;

    [SerializeField]
    private bool _keepSpawning=true;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemies());
        StartCoroutine(SpawnPowerups());
    }

    IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(_enemySpawnRate);
        while (_keepSpawning) { 
            Instantiate(_enemy, new Vector3(Random.Range(-10f, 10f), 9, 0), Quaternion.identity, _enemyParent.transform);
            yield return new WaitForSeconds(Random.Range(_enemySpawnRate * 1.1f, _enemySpawnRate*0.9f));
        }
    }
    IEnumerator SpawnPowerups()
    {
        yield return new WaitForSeconds(_powerUpSpawnRate);
        while (_keepSpawning)
        {
            Instantiate(_powerUpList[Random.Range(0,_powerUpList.Count)], new Vector3(Random.Range(-10f, 10f), 9, 0), 
                        Quaternion.identity, _powerUpParent.transform);
            yield return new WaitForSeconds(Random.Range(_powerUpSpawnRate * 1.5f, _powerUpSpawnRate * 0.5f));
        }
    }

    public void StopSpawning()
    {
        _keepSpawning = false;
        Destroy(_enemyParent);
    }
}
