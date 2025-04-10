using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLauncher : MonoBehaviour
{
    public Transform lauchPoint;
    public GameObject projectilePrefab;
    public void FireProjectile()
    {
        GameObject projectile = Instantiate(projectilePrefab, lauchPoint.position, projectilePrefab.transform.rotation);
        Projectile arrow = projectile.GetComponent<Projectile>();
        arrow.attackPlayer = GetComponent<AttackPlayer>();
        Vector3 originalScale = projectile.transform.localScale;
        //doi huong mui ten theo huong cua nguoi choi
        projectile.transform.localScale = new Vector3(
            originalScale.x *= transform.localScale.x>0? 1 : -1,
            originalScale.y,
            originalScale.z
        );
    }
}