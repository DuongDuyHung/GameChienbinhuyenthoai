using Inventory.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    AttackPlayer attackPlayer;
    public int attackDamage;
    public Vector2 knockBack = Vector2.zero;
    [SerializeField]
    private AgentWeapon agentWeapon;
    [SerializeField]
    private SkillLearnButton skillLearnButton;
    public int comboAttackLevel;
    public enum AttackType
    {
        Normal,
        Magic
    }
    public AttackType currentAttackType = AttackType.Normal; // Mặc định là Normal

    private void Start()
    {
        // Kiểm tra nếu GameObject chứa script này có tên là "Combo"
        if (gameObject.name == "Combo")
        {
            // Tìm GameObject có tag "Skill2" và lấy SkillLearnButton từ đó
            GameObject skillObject = GameObject.FindGameObjectWithTag("Skill2");
            if (skillObject != null)
            {
                skillLearnButton = skillObject.GetComponent<SkillLearnButton>();
            }
            else
            {
                Debug.LogWarning("Skill2 object not found!");
            }
        }
        attackPlayer = GetComponentInParent<AttackPlayer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DamageAble damageAble = collision.GetComponent<DamageAble>();

        if (damageAble != null)
        {
            Defense defense = damageAble.GetComponent<Defense>();
            int actualDamage = 0;
            // Kiểm tra cấp độ của ComboAttack
            if (skillLearnButton != null && skillLearnButton.skillLevels.ContainsKey(SkillLearnButton.SkillType.ComboAttack))
            {
                comboAttackLevel = skillLearnButton.skillLevels[SkillLearnButton.SkillType.ComboAttack];

                // Nếu cấp độ của ComboAttack là 1, giữ nguyên actualDamage
                if (comboAttackLevel == 1)
                {
                    actualDamage = attackDamage + (attackPlayer != null ? attackPlayer.TotalAttackDamage : 0);
                }
                else
                {
                    // Tính toán tổng sát thương với mức tăng từ cấp độ ComboAttack
                    actualDamage = skillLearnButton.CalculateTotalComboDamage();
                }
            }
            else
            {
                // Nếu không có cấp độ ComboAttack, giữ nguyên actualDamage
                actualDamage = attackDamage + (attackPlayer != null ? attackPlayer.TotalAttackDamage : 0);
            }
            Vector2 deliverKnockBack = transform.parent.localScale.x > 0 ? knockBack : new Vector2(-knockBack.x, knockBack.y);

            if (currentAttackType == AttackType.Magic)
            {
                if (defense != null)
                {
                    // Giảm sát thương bằng giá trị giáp

                    if (damageAble.CompareTag("Enemy") && agentWeapon != null)
                    {
                        actualDamage -= defense.baseMagicResistance;
                        // Giảm parametersToModify, giảm 1 điểm mỗi lần va chạm
                        agentWeapon.DecreaseParameter();
                    }
                    else
                    {
                        actualDamage -= defense.currentMagicResistance;
                    }
                    // Đảm bảo sát thương không nhỏ hơn 0
                    if (actualDamage < 0)
                    {
                        actualDamage = 0;
                    }
                }

                bool gotMagicHit = damageAble.MagicHit(actualDamage, deliverKnockBack);
                if (gotMagicHit)
                {
                    Debug.Log(collision.name + " MagicHit for " + actualDamage);
                }
            }
            else if (currentAttackType == AttackType.Normal)
            {
                if (defense != null)
                {
                    // Giảm sát thương bằng giá trị giáp

                    if (damageAble.CompareTag("Enemy") && agentWeapon != null)
                    {
                        actualDamage -= defense.baseArmor;
                        // Giảm parametersToModify, giảm 1 điểm mỗi lần va chạm
                        agentWeapon.DecreaseParameter();
                    }
                    else
                    {
                        actualDamage -= defense.currentArmor;
                    }
                    // Đảm bảo sát thương không nhỏ hơn 0
                    if (actualDamage < 0)
                    {
                        actualDamage = 0;
                    }
                }

                bool gotNormalHit = damageAble.Hit(actualDamage, deliverKnockBack);
                if (gotNormalHit)
                {
                    Debug.Log(collision.name + " Hit for " + actualDamage);
                }
            }
        }
    }
}
