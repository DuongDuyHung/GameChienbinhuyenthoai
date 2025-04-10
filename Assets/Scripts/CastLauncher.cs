using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastLauncher : MonoBehaviour
{
    public Transform lauchPoint;
    public GameObject projectilePrefab;
    public SkillLearnButton skillLearnButton;
    public Cast cast; // Tham chiếu đến thành phần Cast

    void Start()
    {
        skillLearnButton = GameObject.FindGameObjectWithTag("Skill1").GetComponent<SkillLearnButton>();
        // Lấy tham chiếu đến thành phần Cast từ projectilePrefab
        cast = projectilePrefab.GetComponent<Cast>();
    }
    public void CastProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, lauchPoint.position, projectilePrefab.transform.rotation);

        // Gán tham chiếu đến thành phần Cast của projectile
        Cast projectileCast = projectile.GetComponent<Cast>();
        if (projectileCast != null)
        {
            projectileCast.attackPlayer = GetComponent<AttackPlayer>();
            projectileCast.skillLearnButton = skillLearnButton;
        }

        Vector3 originalScale = projectile.transform.localScale;
        projectile.transform.localScale = new Vector3(
            originalScale.x *= transform.localScale.x > 0 ? 1 : -1,
            originalScale.y,
            originalScale.z
        );
        Debug.Log("Projectile scale after setting: " + projectile.transform.localScale);
    }
}
