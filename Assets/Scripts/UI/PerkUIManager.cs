using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PerkUIManager : MonoBehaviour
{
    public Perks perks;

    public TMP_Text perkPointsText;
    public TMP_InputField healthInputField;
    public TMP_InputField manaInputField;
    public TMP_InputField attackInputField;
    public TMP_InputField magicInputField;
    public TMP_InputField armorInputField;
    public TMP_InputField magicResistanceInputField;
    public Button applyButton;
    public DamageAble damageAble;
    public AttackPlayer attackPlayer;
    public Defense defense;
    private int tempHealthPerkPoints = 0;
    private int tempManaPerkPoints = 0;
    private int tempAttackPerkPoints = 0;
    private int tempMagicPerkPoints = 0;
    private int tempArmorPerkPoints = 0;
    private int tempMagicResistancePerkPoints = 0;
    private void Start()
    {
        applyButton.onClick.AddListener(ApplyPerks);
        healthInputField.onValueChanged.AddListener(UpdateHealthPerkPoints);
        manaInputField.onValueChanged.AddListener(UpdateManaPerkPoints);
        attackInputField.onValueChanged.AddListener(UpdateAttackPerkPoints);
        magicInputField.onValueChanged.AddListener(UpdateMagicPerkPoints);
        armorInputField.onValueChanged.AddListener(UpdateArmorPerkPoints);
        magicResistanceInputField.onValueChanged.AddListener(UpdateMagicResistancePerkPoints);
    }

    private void Update()
    {
        perkPointsText.text = "Điểm T.Năng: " + perks.perkPoints.ToString();
    }

    public void ApplyPerks()
    {
        int totalPerkPoints = tempHealthPerkPoints + tempManaPerkPoints + tempAttackPerkPoints + tempMagicPerkPoints + tempArmorPerkPoints + tempMagicResistancePerkPoints;

        if (perks.perkPoints >= totalPerkPoints)
        {
            // Cập nhật perkPoints
            perks.perkPoints -= totalPerkPoints;

            // Cập nhật perkHealth và perkMana trong Perks
            perks.UpdatePerkHealth(perks.perkHealth + tempHealthPerkPoints);
            perks.UpdatePerkMana(perks.perkMana + tempManaPerkPoints);
            perks.UpdatePerkAttack(perks.perkAttack + tempAttackPerkPoints);
            perks.UpdatePerkMagic(perks.perkMagic + tempMagicPerkPoints);
            perks.UpdatePerkArmor(perks.perkArmor + tempArmorPerkPoints);
            perks.UpdatePerkMagicResistance(perks.perkMagicResistance + tempMagicResistancePerkPoints);

            // Cập nhật MaxHealth và MaxMana trong DamageAble
            damageAble.healthFromPerkPoints += tempHealthPerkPoints * 10;
            damageAble.manaFromPerkPoints += tempManaPerkPoints * 10;
            attackPlayer.baseAttackDamage += tempAttackPerkPoints * 5;
            attackPlayer.baseMagicDamage += tempMagicPerkPoints * 5;
            defense.armorFromPerk += tempArmorPerkPoints * 5;
            defense.magicResistanceFromPerk += tempMagicResistancePerkPoints * 5;
            // Đặt lại giá trị của các TMP_InputField về 0
            healthInputField.text = "0";
            manaInputField.text = "0";
            attackInputField.text = "0";
            magicInputField.text = "0";
            armorInputField.text = "0";
            magicResistanceInputField.text = "0";
        }
        else
        {
            // Hiển thị thông báo hoặc cảnh báo khi không đủ điểm tiềm năng
            Debug.LogWarning("Không đủ điểm tiềm năng để áp dụng các perk này!");
        }
    }
    public void UpdateHealthPerkPoints(string value)
    {
        int.TryParse(value, out tempHealthPerkPoints);
    }

    public void UpdateManaPerkPoints(string value)
    {
        int.TryParse(value, out tempManaPerkPoints);
    }
    public void UpdateAttackPerkPoints(string value)
    {
        int.TryParse(value, out tempAttackPerkPoints);
    }
    public void UpdateMagicPerkPoints(string value)
    {
        int.TryParse(value, out tempMagicPerkPoints);
    }
    public void UpdateArmorPerkPoints(string value)
    {
        int.TryParse(value, out tempArmorPerkPoints);
    }
    public void UpdateMagicResistancePerkPoints(string value)
    {
        int.TryParse(value, out tempMagicResistancePerkPoints);
    }
}
