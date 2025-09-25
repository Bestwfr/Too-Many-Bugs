using UnityEngine;
using System.Collections.Generic;
using System.Collections;

namespace FlamingOrange
{
    public class WaveManager : MonoBehaviour
    {
        [SerializeField] private List<GameObject> EnemyList = new List<GameObject>();
        [Header("Wave Settings")]
        [SerializeField] private int maxWave;
        [SerializeField] private int currentWave;
        [Header("Spawn Settings")]
        [SerializeField] private float spawnRadius = 10f;
        [SerializeField] private float spawnInterval = 1.5f;
        [SerializeField] private bool isWaveFinished;
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
        [Header("Gizmo Settings")]
        [SerializeField] private Color gizmoColor = new Color(0.2f, 0.7f, 1f, 0.8f);

        public float _difficultyLevel;
        private int _currentWaveAmount;
        public int _enemiesPerWave = 5;
        private int _spawnCount = 0;
        private CoinManager _coinManager;

        void Start()
        {
            currentWave = 1;
            StartCoroutine(SpawnRoutine());
        }

        void Update()
        {
            _difficultyLevel += difficultyPerSecond * Time.deltaTime;
            CheckKilled();
        }

        IEnumerator SpawnRoutine()
        {
            _spawnCount = 0;
            isWaveFinished = false;
            while (_spawnCount != _enemiesPerWave)
            {
                SpawnOnEdge();
                yield return new WaitForSeconds(spawnInterval);
                _spawnCount++;
            }
        }

        void SpawnOnEdge()
        {
            float angle = Random.Range(0f, Mathf.PI * 2f);
            Vector2 spawnPos = (Vector2)transform.position + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * spawnRadius;
            GameObject enemyPrefab = EnemyList[Random.Range(0, EnemyList.Count)];
            GameObject enemyInstance = Instantiate(enemyPrefab, spawnPos, Quaternion.identity, transform);
            ApplyDifficultyToEnemy(enemyInstance);
        }

        void CheckKilled()
        {
            if (!isWaveFinished && transform.childCount == 0)
            {
                isWaveFinished = true;
                currentWave++;
                StartCoroutine(NextWaveRoutine());
            }
        }

        IEnumerator NextWaveRoutine()
        {
            calculateEnemiesPerWave();
            yield return new WaitForSeconds(InvtervalbetweenWaves);
            StartCoroutine(SpawnRoutine());
        }

        void calculateEnemiesPerWave()
        {
            float logistic = minEnemiesPerWave + (maxEnemiesPerWave - minEnemiesPerWave) / (1f + Mathf.Exp(-rampSteepness * (currentWave - midWave)));
            int difficultyBonus = Mathf.FloorToInt(_difficultyLevel * permaDifficultyBoost);
            int baseCount = Mathf.RoundToInt(logistic) + difficultyBonus;
            int jitter = Mathf.RoundToInt(baseCount * Random.Range(-randomness, randomness));
            _enemiesPerWave = Mathf.Clamp(baseCount + jitter, minEnemiesPerWave, maxEnemiesPerWave);
        }

        void ApplyDifficultyToEnemy(GameObject enemy)
        {
            // change variables inside enemy before spawning by difficulty level
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
