using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cast : MonoBehaviour
{
    public AttackPlayer attackPlayer;
    public int damage;
    public float moveSpeed = 4f;
    public Vector2 knockBack = new Vector2(0, 0);
    public float lifeTime = 2.0f; // Thời gian tồn tại của mũi tên
    Rigidbody2D rb;
    public SkillLearnButton skillLearnButton;
    public int castAttackLevel;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        UpdateVelocity();
        StartCoroutine(DestroyAfterTime());
    }
    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(lifeTime);
        Destroy(gameObject);
    }

    private void Update()
    {
        if (transform.hasChanged)
        {
            FlipDirection();
            transform.hasChanged = false;
        }
    }

    private void FlipDirection()
    {
        float direction = Mathf.Sign(rb.velocity.x);
        float rotationZ = direction > 0 ? 180 : 180;
        transform.rotation = Quaternion.Euler(0, 0, rotationZ);
        UpdateVelocity();
    }

    private void UpdateVelocity()
    {
        rb.velocity = new Vector2(moveSpeed * transform.localScale.x, 0);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DamageAble damageAble = collision.GetComponent<DamageAble>();
        if (damageAble != null)
        {
            Defense defense = damageAble.GetComponent<Defense>();
            int actualDamage = 0;
            Vector2 deliveredKnockBack = transform.localScale.x > 0 ? knockBack : new Vector2(-knockBack.x, knockBack.y);
            if (skillLearnButton != null && skillLearnButton.skillLevels.ContainsKey(SkillLearnButton.SkillType.CastAttack))
            {
                castAttackLevel = skillLearnButton.skillLevels[SkillLearnButton.SkillType.CastAttack];

                // Nếu cấp độ của ComboAttack là 1, giữ nguyên actualDamage
                if (castAttackLevel == 1)
                {
                    actualDamage = damage + (attackPlayer != null ? attackPlayer.TotalMagicDamage : 0);
                }
                else
                {
                    // Tính toán tổng sát thương với mức tăng từ cấp độ ComboAttack
                    actualDamage = skillLearnButton.CalculateTotalCastDamage();
                }
            }
            else
            {
                // Nếu không có cấp độ ComboAttack, giữ nguyên actualDamage
                actualDamage = damage + (attackPlayer != null ? attackPlayer.TotalAttackDamage : 0);
            }

            if (defense != null)
            {
                // Giảm sát thương bằng giá trị giáp
                actualDamage -= defense.baseMagicResistance;

                // Đảm bảo sát thương không nhỏ hơn 0
                if (actualDamage < 0)
                {
                    actualDamage = 0;
                }
            }

            bool gotHit = damageAble.MagicHit(actualDamage, deliveredKnockBack);
            if (gotHit)
            {
                Debug.Log(collision.name + " MagicHit for " + actualDamage);
                Destroy(gameObject);
            }
        }
    }
}
