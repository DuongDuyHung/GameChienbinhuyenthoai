using UnityEngine;
using UnityEditor;
using Inventory.UI;

public class AttackPlayer : MonoBehaviour
{
    [SerializeField]
    private UIInventoryPage uiInventoryPage;
    private LevelAndExperience levelAndExperience;
    Defense defense;
    [SerializeField]
    public int baseAttackDamage = 10;

    [SerializeField]
    public int attackDamageModifier = 0;

    [SerializeField]
    public int baseMagicDamage = 10;

    [SerializeField]
    public int magicDamageModifier = 0;

    [SerializeField]
    public int totalAttackDamage;

    [SerializeField]
    public int totalMagicDamage;

    public int TotalAttackDamage => baseAttackDamage + attackDamageModifier;
    public int TotalMagicDamage => baseMagicDamage + magicDamageModifier;
    private void Start()
    {
        uiInventoryPage = GameObject.FindGameObjectWithTag("UIInventoryPage").GetComponent<UIInventoryPage>();
        levelAndExperience = GetComponent<LevelAndExperience>();
    }
    private void Update()
    {
        totalAttackDamage = TotalAttackDamage;
        totalMagicDamage = TotalMagicDamage;
        UpdateUI();
    }
    private void UpdateUI()
    {
        Defense defense = GetComponent<Defense>();
        DamageAble healthAndMana = GetComponent<DamageAble>();

        if (defense != null && healthAndMana != null && uiInventoryPage != null && levelAndExperience != null)
        {
            uiInventoryPage.UpdateCharacterInfo(
                levelAndExperience.level,
                levelAndExperience.experience,
                levelAndExperience.experienceToNextLevel,
                TotalAttackDamage,
                TotalMagicDamage,
                defense.currentArmor,
                defense.currentMagicResistance,
                healthAndMana.Health,
                healthAndMana.MaxHealth,
                (int)healthAndMana.Mana,  // Làm tròn _mana đến một chữ số thập phân
                (int)healthAndMana.MaxMana
            );
        }

        else
        {
            Debug.LogWarning("Defense, AttackPlayer, or UIInventoryPage is null.");
        }
    }
    public void ApplyItemParameters(int attackModifier, int magicModifier)
    {
        attackDamageModifier = attackModifier;
        magicDamageModifier = magicModifier;
    }
}
