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
    private List<Enemy> _enemyList;
    [SerializeField]
    private GameObject _enemyParent;

    [SerializeField]
    private List<Powerup> _powerUpList;
    [SerializeField]
    private GameObject _powerUpParent;

    [SerializeField]
    private GameObject _boss;

    [SerializeField]
    private bool _keepSpawning=true;

    private int _sumOfWeightsPU=0;
    private int _sumOfWeightsE = 0;

    [SerializeField]
    private int _waveCounter = 1;
    private UIManager _uIManager;

    private bool _stillSpawningEnemies=false;
    private bool _stillSpawningPowerups=false;

    private void Start()
    {
        foreach(Powerup powerup in _powerUpList)
        {
            _sumOfWeightsPU += powerup.GetWeight();
        }
        foreach (Enemy enemy in _enemyList)
        {
            _sumOfWeightsE += enemy.GetWeight();
        }
        _uIManager = GameObject.Find("UI").GetComponent<UIManager>();
        if (_uIManager == null)
        {
            Debug.Log("Cannot find the UIManager component");
        }
    }

    public void StartSpawning()
    {
        _keepSpawning = true;
        StartCoroutine(StartWave());
    }


    IEnumerator StartWave()
    {
        while (_keepSpawning)
        {
            if (_waveCounter % 5 == 0 && _stillSpawningPowerups == false && _stillSpawningEnemies == false && _enemyParent.transform.childCount == 0 && _powerUpParent.transform.childCount == 0)
            {

                _uIManager.StartWave("Boss Wave");
                Instantiate(_boss, new Vector3(0, 3, 0), Quaternion.identity, _enemyParent.transform);
                StartCoroutine(SpawnPowerups(100));
                if (_waveCounter == 5)
                {
                    _keepSpawning = false;

                }
                _waveCounter++;
            }
            else if (_waveCounter % 5 > 0 && _stillSpawningPowerups == false && _stillSpawningEnemies == false && _enemyParent.transform.childCount ==0 && _powerUpParent.transform.childCount == 0)
            {
                _uIManager.StartWave("Wave " + _waveCounter);
                StartCoroutine(SpawnEnemies(_waveCounter * 2));
                StartCoroutine(SpawnPowerups(_waveCounter * 2));
                _waveCounter++;
            }
            Debug.Log("Waiting for the player to finish the wave");
            yield return new WaitForSeconds(1);
        }
        while (_keepSpawning==false)
        {
            if(_enemyParent.transform.childCount == 0 && _powerUpParent.transform.childCount == 0)
            {
                _uIManager.EndlessModeTextActive();
            }
            yield return new WaitForSeconds(1);
        }
    }

    IEnumerator SpawnEnemies(int enemysLeft)
    {
        _stillSpawningEnemies = true;
        yield return new WaitForSeconds(5);
        while (_keepSpawning && enemysLeft>0) { 
            Instantiate(GetWeightedRandomEnemy(), new Vector3(Random.Range(-10f, 10f), 9, 0), Quaternion.identity, _enemyParent.transform);
            enemysLeft--;
            yield return new WaitForSeconds(Random.Range(_enemySpawnRate*0.7f, _enemySpawnRate * 1.2f));
        }
        _stillSpawningEnemies = false;
    }

    IEnumerator SpawnPowerups(int powerUpsLeft)
    {
        _stillSpawningPowerups = true;
        //spawn 1 ammo powerup at the start of each wave
        Instantiate(_powerUpList[0], new Vector3(Random.Range(-10f, 10f), 9, 0),
                    Quaternion.identity, _powerUpParent.transform);
        //spawn 1 health powerup at the start of each wave
        Instantiate(_powerUpList[1], new Vector3(Random.Range(-10f, 10f), 9, 0),
                    Quaternion.identity, _powerUpParent.transform);
        yield return new WaitForSeconds(5);
        while (powerUpsLeft>0)
        {
            Instantiate(GetWeightedRandomPowerup(), new Vector3(Random.Range(-10f, 10f), 9, 0), 
                        Quaternion.identity, _powerUpParent.transform);
            powerUpsLeft--;
            yield return new WaitForSeconds(Random.Range(_powerUpSpawnRate * 0.5f, _powerUpSpawnRate * 1.5f));
            if(_stillSpawningEnemies == false && _enemyParent!= null && _enemyParent.transform.childCount == 0)
            {
                break;
            }
        }
        _stillSpawningPowerups = false;
    }

    private GameObject GetWeightedRandomPowerup()
    {
        int cumulativeWeight = 0;
        int randomVal = Random.Range(0, _sumOfWeightsPU);
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
    private GameObject GetWeightedRandomEnemy()
    {
        int cumulativeWeight = 0;
        int randomVal = Random.Range(0, _sumOfWeightsE);
        foreach (Enemy enemy in _enemyList)
        {
            cumulativeWeight += enemy.GetWeight();
            if (cumulativeWeight > randomVal)
            {
                return enemy.gameObject;
            }
        }
        Debug.Log("Enemy not found in weighted calculation, sending element 0");
        return _enemyList[0].gameObject;
    }

    public void StopSpawning()
    {
        _keepSpawning = false;
        Destroy(_enemyParent);
        Destroy(_powerUpParent);
    }
}
