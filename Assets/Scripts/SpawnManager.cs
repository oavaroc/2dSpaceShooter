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
    private List<Powerup> _powerUpList;
    [SerializeField]
    private GameObject _powerUpParent;

    [SerializeField]
    private bool _keepSpawning=true;

    private int _sumOfWeights=0;

    private void Start()
    {
        foreach(Powerup powerup in _powerUpList)
        {
            _sumOfWeights += powerup.GetWeight();
        }
    }

    public void StartSpawning()
    {
        StartCoroutine(SpawnEnemies());
        StartCoroutine(SpawnPowerups());

    }

    IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(_enemySpawnRate);
        while (_keepSpawning) { 
            Instantiate(_enemy, new Vector3(Random.Range(-10f, 10f), 9, 0), Quaternion.identity, _enemyParent.transform);
            yield return new WaitForSeconds(Random.Range(_enemySpawnRate*0.7f, _enemySpawnRate * 1.2f));
        }
    }
    IEnumerator SpawnPowerups()
    {
        yield return new WaitForSeconds(_powerUpSpawnRate);
        while (_keepSpawning)
        {
            Instantiate(GetWeightedRandomPowerup(), new Vector3(Random.Range(-10f, 10f), 9, 0), 
                        Quaternion.identity, _powerUpParent.transform);
            yield return new WaitForSeconds(Random.Range(_powerUpSpawnRate * 0.5f, _powerUpSpawnRate * 1.5f));
        }
    }

    private GameObject GetWeightedRandomPowerup()
    {
        int cumulativeWeight = 0;
        int randomVal = Random.Range(0, _sumOfWeights);
        foreach (Powerup powerup in _powerUpList)
        {
            cumulativeWeight += powerup.GetWeight();
            if(cumulativeWeight > randomVal)
            {
                return powerup.gameObject;
            }
        }
        Debug.Log("Power up not found in weighted calculation, sending element 0");
        return _powerUpList[0].gameObject;
    }

    public void StopSpawning()
    {
        _keepSpawning = false;
        Destroy(_enemyParent);
    }
}
