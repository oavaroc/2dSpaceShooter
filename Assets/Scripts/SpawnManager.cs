using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private float spawnRate = 5f;

    [SerializeField]
    private GameObject _enemy;
    [SerializeField]
    private GameObject enemyParent;

    [SerializeField]
    private bool keepSpawning=true;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemies());
    }

    IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(spawnRate);
        while (keepSpawning) { 
            Instantiate(_enemy, new Vector3(Random.Range(-10, 10), 9, 0), Quaternion.identity, enemyParent.transform);
            yield return new WaitForSeconds(spawnRate);
        }
    }

    public void StopSpawning()
    {
        keepSpawning = false;
        Destroy(enemyParent);
    }
}
