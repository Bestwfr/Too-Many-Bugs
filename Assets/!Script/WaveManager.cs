using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using FlamingOrange.Enemies;
using PurrNet;
using TMPro;

namespace FlamingOrange
{
    public class WaveManager : NetworkBehaviour
    {
        [Header("Enemy Settings")]
        [SerializeField] private List<GameObject> EnemyPrefabs = new List<GameObject>();
        [SerializeField] private int poolSizePerEnemy = 50;

        private Dictionary<GameObject, Queue<GameObject>> enemyPools = new Dictionary<GameObject, Queue<GameObject>>();

        [Header("Wave Settings")]
        [SerializeField] private int maxWave;
        [SerializeField] private int currentWave;
        [SerializeField] private bool isWaveFinished;

        [Header("Spawn Settings")]
        [SerializeField] private float spawnRadius = 10f;
        [SerializeField] private float spawnInterval = 1.5f;
        [SerializeField] private float InvtervalbetweenWaves = 2f;

        [Header("Difficulty Settings")]
        [SerializeField] private float difficultyPerSecond = 0.1f;

        [Header("Wave Scaling (S-curve)")]
        [SerializeField] private int minEnemiesPerWave = 4;
        [SerializeField] private int maxEnemiesPerWave = 45;
        [SerializeField] private int midWave = 10;
        [SerializeField] private float rampSteepness = 0.35f;
        [SerializeField] private float permaDifficultyBoost = 0.5f;
        [SerializeField] private float randomness = 0.10f;
        
        [Header("Wave UI")]
        [SerializeField] private TextMeshProUGUI currentWaveUI;
        [SerializeField] private TextMeshProUGUI waveTimeUI;

        [Header("Gizmo Settings")]
        [SerializeField] private Color gizmoColor = new Color(0.2f, 0.7f, 1f, 0.8f);

        private float _difficultyLevel;
        private int _spawnCount;
        private int _enemiesPerWave = 5;

        protected override void OnSpawned()
        {
            if (!isServer) return;
            
            InitializePools();

            currentWave = 1;
            StartCoroutine(SpawnRoutine());
        }

        private void Update()
        {
            if (!isServer) return;

            _difficultyLevel += difficultyPerSecond * Time.deltaTime;
            CheckKilled();

            UpdateWaveUI();
        }

        private void UpdateWaveUI()
        {
            if (currentWaveUI)
                currentWaveUI.text = $"Wave {currentWave}";
    
            if (waveTimeUI)
                waveTimeUI.text = isWaveFinished 
                    ? "Next wave starting..." 
                    : $"Enemies Remaining: {GetActiveEnemiesCount()}";
        }
        
        private void InitializePools()
        {
            foreach (var prefab in EnemyPrefabs)
            {
                if (!enemyPools.ContainsKey(prefab))
                {
                    Queue<GameObject> pool = new Queue<GameObject>();

                    for (int i = 0; i < poolSizePerEnemy; i++)
                    {
                        GameObject obj = Instantiate(prefab, transform);
                        obj.SetActive(false);
                        pool.Enqueue(obj);
                    }

                    enemyPools.Add(prefab, pool);
                }
            }
        }
        
        private GameObject GetEnemyFromPool(GameObject prefab)
        {
            if (!enemyPools.ContainsKey(prefab))
            {
                Debug.LogWarning($"No pool found for {prefab.name}, creating one dynamically.");
                enemyPools[prefab] = new Queue<GameObject>();
            }

            if (enemyPools[prefab].Count == 0)
            {
                // Expand pool if empty
                GameObject newObj = Instantiate(prefab, transform);
                newObj.SetActive(false);
                enemyPools[prefab].Enqueue(newObj);
            }

            GameObject enemy = enemyPools[prefab].Dequeue();
            enemy.SetActive(true);
            return enemy;
        }
        
        private int GetActiveEnemiesCount()
        {
            int count = 0;
            foreach (Transform child in transform)
            {
                if (child.gameObject.activeSelf)
                    count++;
            }
            return count;
        }
        
        public void ReturnEnemyToPool(GameObject enemy, GameObject prefabKey)
        {
            enemy.SetActive(false);
            enemy.transform.SetParent(transform);
            enemyPools[prefabKey].Enqueue(enemy);
        }
        
        IEnumerator SpawnRoutine()
        {
            _spawnCount = 0;
            isWaveFinished = false;

            UpdateWaveUI(); // show wave start immediately

            while (_spawnCount < _enemiesPerWave)
            {
                SpawnOnEdge();
                yield return new WaitForSeconds(spawnInterval);
                _spawnCount++;
            }
        }

        private void SpawnOnEdge()
        {
            if (EnemyPrefabs == null || EnemyPrefabs.Count == 0)
            {
                Debug.LogError("EnemyPrefabs list is empty! Please assign at least one prefab in the WaveManager.");
                return;
            }

            float angle = Random.Range(0f, Mathf.PI * 2f);
            Vector2 spawnPos = (Vector2)transform.position + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnRadius;

            GameObject prefab = EnemyPrefabs[Random.Range(0, EnemyPrefabs.Count)];
            GameObject enemy = GetEnemyFromPool(prefab);

            enemy.transform.position = spawnPos;
            enemy.transform.rotation = Quaternion.identity;
            
            var reused = enemy.GetComponent<Entity>();
            if (reused != null)
                reused.OnReusedFromPool();

            ApplyDifficultyToEnemy(enemy);
        }

        private void CheckKilled()
        {
            bool allInactive = true;

            foreach (Transform child in transform)
            {
                if (child.gameObject.activeSelf)
                {
                    allInactive = false;
                    break;
                }
            }

            if (!isWaveFinished && allInactive)
            {
                isWaveFinished = true;
                currentWave++;
                UpdateWaveUI();
                StartCoroutine(NextWaveRoutine());
            }
        }

        private IEnumerator NextWaveRoutine()
        {
            CalculateEnemiesPerWave();
            yield return new WaitForSeconds(InvtervalbetweenWaves);
            StartCoroutine(SpawnRoutine());
        }

        private void CalculateEnemiesPerWave()
        {
            float logistic = minEnemiesPerWave + (maxEnemiesPerWave - minEnemiesPerWave) / (1f + Mathf.Exp(-rampSteepness * (currentWave - midWave)));
            int difficultyBonus = Mathf.FloorToInt(_difficultyLevel * permaDifficultyBoost);
            int baseCount = Mathf.RoundToInt(logistic) + difficultyBonus;
            int jitter = Mathf.RoundToInt(baseCount * Random.Range(-randomness, randomness));
            _enemiesPerWave = Mathf.Clamp(baseCount + jitter, minEnemiesPerWave, maxEnemiesPerWave);
        }

        private void ApplyDifficultyToEnemy(GameObject enemy)
        {
            // TODO: scale HP / Damage by _difficultyLevel or currentWave
        }

        void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;
            int segments = 64;
            Vector3 prevPoint = transform.position + new Vector3(spawnRadius, 0, 0);
            for (int i = 1; i <= segments; i++)
            {
                float angle = i * Mathf.PI * 2f / segments;
                Vector3 nextPoint = transform.position + new Vector3(Mathf.Cos(angle) * spawnRadius, Mathf.Sin(angle) * spawnRadius, 0);
                Gizmos.DrawLine(prevPoint, nextPoint);
                prevPoint = nextPoint;
            }
        }
    }
}