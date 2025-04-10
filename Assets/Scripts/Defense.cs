using Inventory.Model;
using Inventory.UI;
using System.Collections.Generic;
using UnityEngine;

public class Defense : MonoBehaviour
{
    public int baseArmor = 10; // Giáp cơ bản
    public int baseMagicResistance = 10; // Kháng phép cơ bản

    private int setArmorModifier = 0; // Giá trị từ SetArmor
    private int helmetArmorModifier = 0; // Giá trị từ SetHelmet
    private int bootsArmorModifier = 0; // Giá trị từ SetBoots

    private int setMagicResistanceModifier = 0; // Giá trị từ SetArmor
    private int helmetMagicResistanceModifier = 0; // Giá trị từ SetHelmet
    private int bootsMagicResistanceModifier = 0; // Giá trị từ SetBoots
    public int armorFromPerk = 0;
    public int magicResistanceFromPerk = 0;
    [SerializeField]
    public int totalArmorModifier => setArmorModifier + helmetArmorModifier + bootsArmorModifier; // Tổng của ba giá trị
    [SerializeField]
    public int totalMagicResistanceModifier => setMagicResistanceModifier + helmetMagicResistanceModifier + bootsMagicResistanceModifier; // Tổng của ba giá trị
    [SerializeField]
    public int currentArmor
    {
        get
        {
            return baseArmor + totalArmorModifier + armorFromPerk;
        }
        set
        {
            baseArmor = value;
        }
    }
    [SerializeField]
    public int currentMagicResistance
    {
        get
        {
            return baseMagicResistance + totalMagicResistanceModifier + magicResistanceFromPerk;
        }
        set
        {
            baseMagicResistance = value;
        }
    }

    [SerializeField]
    private int TotalArmorModifier;

    [SerializeField]
    private int TotalMagicResistanceModifier;

    public int Armor => currentArmor;
    public int MagicResistance => currentMagicResistance;
    public void Awake()
    {
        UpdateCurrentValues();
    }
    public void SetArmor(EquipableItemSO itemSO, List<ItemParameter> itemState = null)
    {
        if (itemSO == null)
        {
            setArmorModifier = 0;
            setMagicResistanceModifier = 0;
        }
        else
        {
            setArmorModifier = itemSO.armorModifier;
            setMagicResistanceModifier = itemSO.magicResistanceModifier;
            var currentDurabilityParameter = itemState.Find(param => param.itemParameter is ItemParameterSO);
            float currentDurability = currentDurabilityParameter.itemParameter != null ? currentDurabilityParameter.value : 0f;

            if (currentDurability <= 0)
            {
                setArmorModifier = 0;
                setMagicResistanceModifier = 0;
            }
        }
        
        UpdateCurrentValues();
    }

    public void SetHelmet(EquipableItemSO itemSO, List<ItemParameter> itemState = null)
    {
        if (itemSO == null)
        {
            helmetArmorModifier = 0;
            helmetMagicResistanceModifier= 0;
        }
        else
        {
            helmetArmorModifier = itemSO.armorModifier;
            helmetMagicResistanceModifier = itemSO.magicResistanceModifier;
            var currentDurabilityParameter = itemState.Find(param => param.itemParameter is ItemParameterSO);
            float currentDurability = currentDurabilityParameter.itemParameter != null ? currentDurabilityParameter.value : 0f;

            if (currentDurability <= 0)
            {
                helmetArmorModifier = 0;
                helmetMagicResistanceModifier = 0;
            }
        }
        UpdateCurrentValues();
    }

    public void SetBoots(EquipableItemSO itemSO, List<ItemParameter> itemState = null)
    {
        if (itemSO == null)
        {
            bootsArmorModifier = 0;
            bootsMagicResistanceModifier = 0;
        }
        else
        {
            bootsArmorModifier = itemSO.armorModifier;
            bootsMagicResistanceModifier = itemSO.magicResistanceModifier;
            var currentDurabilityParameter = itemState.Find(param => param.itemParameter is ItemParameterSO);
            float currentDurability = currentDurabilityParameter.itemParameter != null ? currentDurabilityParameter.value : 0f;

            if (currentDurability <= 0)
            {
                bootsArmorModifier = 0;
                bootsMagicResistanceModifier = 0;
            }
        }
        
        UpdateCurrentValues();
    }

    private void UpdateCurrentValues()
    {
        //currentArmor = baseArmor + totalArmorModifier;
        //currentMagicResistance = baseMagicResistance + totalMagicResistanceModifier;

        if (currentArmor < 0) currentArmor = 0;
        if (currentMagicResistance < 0) currentMagicResistance = 0;

        // Update debug values
        TotalArmorModifier = totalArmorModifier;
        TotalMagicResistanceModifier = totalMagicResistanceModifier;
    }
    public void TakeDamage(int damage)
    {
        baseArmor -= damage;
        if (baseArmor < 0) baseArmor = 0;
        UpdateCurrentValues();
    }

    public void TakeMagicDamage(int magicDamage)
    {
        baseMagicResistance -= magicDamage;
        if (baseMagicResistance < 0) baseMagicResistance = 0;
        UpdateCurrentValues();
    }

    //public void HealArmor(int amount)
    //{
    //    baseArmor += amount;
    //    UpdateCurrentValues();
    //}

    //public void HealMagicResistance(int amount)
    //{
    //    baseMagicResistance += amount;
    //    UpdateCurrentValues();
    //}
}


