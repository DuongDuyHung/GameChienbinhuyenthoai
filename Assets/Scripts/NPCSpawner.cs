using Fusion;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.Collections.Unicode;

public class NPCSpawner : MonoBehaviour
{
    public List<GameObject> enemyPrefabs;
    public Collider2D spawnArea;
    public Collider2D playerDetectionArea;
    public float minSpawnDelay = 1f;
    public float maxSpawnDelay = 3f;
    public int minAttackDamage = 10;
    public int maxAttackDamage = 15;
    public int minBaseArmor = 5;
    public int maxBaseArmor = 10;
    public int minBaseMagicResistance = 5;
    public int maxBaseMagicResistance = 10;
    private bool isSpawning = true;
    public int minEnemyLevel = 1;
    public int maxEnemyLevel = 10;
    public NetworkRunner Runner;
    private GameObject[] player; // Reference to the player GameObject
    private List<GameObject> spawnedEnemies = new List<GameObject>(); // List to store spawned enemies
    private int maxEnemiesSpawned = 2; // Maximum number of enemies to spawn

    private void Start()
    {
        // Tìm tất cả các đối tượng có tên "Player(Clone)" trong scene
        player = GameObject.FindGameObjectsWithTag("Playerr"); // Hoặc dùng Find nếu không có tag

        if (player.Length == 0)
        {
            Debug.LogWarning("No Player objects found with tag 'Playerr'.");
        }
        Runner = FindObjectOfType<NetworkRunner>();
        if (Runner == null)
        {
            Debug.LogError("NetworkRunner not found!");
        }
        StartCoroutine(WaitForPlayer());
        StartCoroutine(FindPlayerNetworkObject());
    }
    private void Update()
    {
        // Tìm tất cả các đối tượng có tên "Player(Clone)" trong scene
        player = GameObject.FindGameObjectsWithTag("Playerr"); // Hoặc dùng Find nếu không có tag

        if (player.Length == 0)
        {
            Debug.LogWarning("No Player objects found with tag 'Playerr'.");
        }
        Runner = FindObjectOfType<NetworkRunner>();
        if (Runner == null)
        {
            Debug.LogError("NetworkRunner not found!");
        }
    }
    private IEnumerator WaitForPlayer()
    {
        while (player == null || player.Length == 0) // Chờ cho đến khi tìm thấy ít nhất một player
        {
            player = GameObject.FindGameObjectsWithTag("Playerr");
            yield return null; // Chờ frame tiếp theo
        }
        Debug.Log($"Found {player.Length} players.");
        StartCoroutine(SpawnEnemies()); // Bắt đầu spawn kẻ địch
    }


    private IEnumerator FindPlayerNetworkObject()
    {
        List<GameObject> playerList = new List<GameObject>();
        while (player == null || player.Length == 0)
        {
            playerList.Clear(); // Xóa danh sách cũ
            foreach (var obj in FindObjectsOfType<Fusion.NetworkObject>())
            {
                if (obj.CompareTag("Playerr"))
                {
                    playerList.Add(obj.gameObject);
                }
            }
            player = playerList.ToArray(); // Cập nhật mảng player
            yield return null; // Chờ frame tiếp theo
        }
        Debug.Log($"Found {player.Length} players.");
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        while (isSpawning)
        {
            if (IsPlayerNearby() && spawnedEnemies.Count < maxEnemiesSpawned)
            {
                Vector2 spawnPosition = GetRandomSpawnPosition();

                if (!IsSpawnPositionValid(spawnPosition))
                {
                    yield return null;
                    continue;
                }

                GameObject enemyPrefab = GetRandomEnemyPrefab();
                if (Runner.IsServer)
                {
                    Fusion.NetworkObject networkObject = Runner.Spawn(enemyPrefab, spawnPosition, Quaternion.identity);
                    GameObject spawnedEnemy = networkObject.gameObject; // Chuyển đổi từ NetworkObject sang GameObject
                    spawnedEnemies.Add(spawnedEnemy); // Add spawned enemy to the list
                                                      // Gán player vào script Enemy trên spawnedEnemy
                    Enemy enemyComponent = spawnedEnemy.GetComponent<Enemy>();
                    if (enemyComponent != null && player.Length > 0)
                    {
                        // Tìm player gần nhất
                        Transform closestPlayer = null;
                        float closestDistance = Mathf.Infinity;

                        foreach (GameObject p in player)
                        {
                            if (p == null) continue;

                            float distance = Vector2.Distance(spawnedEnemy.transform.position, p.transform.position);
                            if (distance < closestDistance)
                            {
                                closestDistance = distance;
                                closestPlayer = p.transform;
                            }
                        }

                        enemyComponent.player = closestPlayer; // Gán player gần nhất cho enemy
                    }
                    LevelAndExperience playerLevel = FindObjectOfType<LevelAndExperience>();
                    // Attach the player's LevelAndExperience to the spawned enemy
                    DamageAble damageableComponent = spawnedEnemy.GetComponent<DamageAble>();
                    if (damageableComponent != null)
                    {
                        damageableComponent.playerLevel = playerLevel;
                    }
                    // Set baseArmor and baseMagicResistance of the spawned enemy
                    Defense defenseComponent = spawnedEnemy.GetComponent<Defense>();
                    if (defenseComponent != null)
                    {
                        defenseComponent.baseArmor = Random.Range(minBaseArmor, maxBaseArmor + 1);
                        defenseComponent.baseMagicResistance = Random.Range(minBaseMagicResistance, maxBaseMagicResistance + 1);
                    }

                    // Set attackDamage of the spawned enemy
                    Attack attackComponent = spawnedEnemy.GetComponentInChildren<Attack>();
                    if (attackComponent != null)
                    {
                        attackComponent.attackDamage = Random.Range(minAttackDamage, maxAttackDamage + 1);
                    }
                    int enemyLevel = Random.Range(minEnemyLevel, maxEnemyLevel + 1);
                    EnemyLevel enemyLevelComponent = spawnedEnemy.GetComponent<EnemyLevel>();
                    if (enemyLevelComponent != null)
                    {
                        enemyLevelComponent.FixedLevel = enemyLevel;
                    }
                }


            }

            float spawnDelay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(spawnDelay);

            // Remove null or inactive enemies from the spawnedEnemies list
            spawnedEnemies.RemoveAll(enemy => enemy == null || !enemy.activeSelf);
        }
    }

    private bool IsSpawnPositionValid(Vector2 spawnPosition)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnPosition, 1f, LayerMask.GetMask("Ground"));

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Ground"))
            {
                return false;
            }
        }

        return true;
    }

    private Vector2 GetRandomSpawnPosition()
    {
        Vector2 spawnAreaMin = spawnArea.bounds.min;
        Vector2 spawnAreaMax = spawnArea.bounds.max;

        float randomX = Random.Range(spawnAreaMin.x, spawnAreaMax.x);
        float randomY = Random.Range(spawnAreaMin.y, spawnAreaMax.y);

        return new Vector2(randomX, randomY);
    }

    private GameObject GetRandomEnemyPrefab()
    {
        int randomIndex = Random.Range(0, enemyPrefabs.Count);
        GameObject enemyPrefab = enemyPrefabs[randomIndex];

        return enemyPrefab;
    }

    private bool IsPlayerNearby()
    {
        if (player == null || player.Length == 0) // Kiểm tra nếu không có player
        {
            return false;
        }

        foreach (GameObject p in player)
        {
            if (p != null && playerDetectionArea.bounds.Contains(p.transform.position))
            {
                return true; // Có ít nhất một player trong phạm vi
            }
        }
        return false; // Không có player nào trong phạm vi
    }


    public void SetSpawning(bool spawning)
    {
        isSpawning = spawning;
    }
}
