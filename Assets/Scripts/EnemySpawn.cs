using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class EnemySpawn : MonoBehaviourPun
{
    public static EnemySpawn instance;
    [SerializeField] GameObject enemyPrefab;
    public int poolSize = 10;
    public float spawnInterval;
    public float spawnXRange;
    Queue<GameObject> enemyPool;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            InitializePool();
            InvokeRepeating(nameof(SpawnEnemy), 1f, spawnInterval);
        }
    }

    void InitializePool()
    {
        enemyPool = new Queue<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            // 적을 네트워크를 통해 생성
            GameObject enemy = PhotonNetwork.Instantiate(enemyPrefab.name, Vector3.zero, Quaternion.identity);
            enemy.SetActive(false);
            enemyPool.Enqueue(enemy);
        }
    }

    GameObject GetEnemyFromPool()
    {
        if (enemyPool.Count > 0)
        {
            GameObject enemy = enemyPool.Dequeue();
            enemy.SetActive(true);
            return enemy;
        }
        return null;
    }

    public void ReturnEnemyToPool(GameObject enemy)
    {
        enemy.SetActive(false);
        enemyPool.Enqueue(enemy);
    }

    void SpawnEnemy()
    {
        if (GameManager.instance.isGameOver) return;

        float randomX = Random.Range(-spawnXRange, spawnXRange);
        Vector3 spawnPosition = new Vector3(randomX, 5f, 18f);

        var enemy = GetEnemyFromPool();
        if (enemy != null)
        {
            enemy.transform.position = spawnPosition;
            enemy.transform.rotation = Quaternion.identity;

            // 적의 위치를 동기화
            photonView.RPC("SyncEnemySpawn", RpcTarget.Others, spawnPosition);
        }
    }

    [PunRPC]
    void SyncEnemySpawn(Vector3 spawnPosition)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            var enemy = GetEnemyFromPool();
            if (enemy != null)
            {
                enemy.transform.position = spawnPosition;
                enemy.transform.rotation = Quaternion.identity;
            }
        }
    }
}
