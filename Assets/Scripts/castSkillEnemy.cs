using Fusion;
using UnityEngine;

public class castSkillEnemy : NetworkBehaviour
{
    public GameObject darkThunderPrefab; // Prefab của DarkThunder
    public Animator animator;            // Animator của quái vật
    public float skillCooldown = 10f;    // Thời gian giữa các lần cast skill
    public int thunderCount = 5;         // Số lượng DarkThunder được tạo
    public float spawnRadius = 3f;       // Bán kính khu vực tạo DarkThunder

    private float nextCastTime;

    void Update()
    {
        animator = GetComponent<Animator>();
        // Kiểm tra nếu đủ thời gian để cast skill
        if (Time.time >= nextCastTime)
        {
            CastSkill();
            nextCastTime = Time.time + skillCooldown;
        }
    }

    // Sử dụng RPC để thực hiện đồng bộ hóa giữa các máy khách
    [Rpc(RpcSources.All, RpcTargets.All)]
    void RpcCastSkill()
    {
        // Kích hoạt animation castSkill trên tất cả các máy khách
        if (animator != null)
        {
            animator.SetTrigger("CastThunder");
        }

        // Gọi hàm tạo "mưa sấm sét"
        SpawnDarkThunderStorm();
    }

    // Gọi phương thức RPC từ máy chủ
    void CastSkill()
    {
        // Gọi RpcCastSkill từ máy chủ để đồng bộ trên tất cả các client
        RpcCastSkill();
    }

    void SpawnDarkThunderStorm()
    {
        // Tạo nhiều DarkThunder trong khu vực spawnRadius
        for (int i = 0; i < thunderCount; i++)
        {
            // Sử dụng transform.position để lấy vị trí hiện tại của đối tượng chứa script
            Vector3 randomPosition = transform.position + new Vector3(
                Random.Range(-spawnRadius, spawnRadius),
                0,
                Random.Range(-spawnRadius, spawnRadius)
            );

            Instantiate(darkThunderPrefab, randomPosition, Quaternion.identity);
        }
    }
}
