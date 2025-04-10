using Inventory.Model;
using Inventory.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentArmor : MonoBehaviour
{
    private Image content;
    [SerializeField]
    private Image durabilityArmor;
    [SerializeField]
    public EquipableItemSO armorItem;
    [SerializeField]
    public UIEquippedItem uiEquippedItem;
    [SerializeField]
    public UIDurability itemDurability;
    [SerializeField]
    private InventorySO inventoryData;
    [SerializeField]
    private DamageAble damageAble; // Thêm một tham chiếu đến DamageAble

    [SerializeField]
    private Defense defense;
    [SerializeField]
    public List<ItemParameter> parametersToModify, itemCurrentState;
    public void Start()
    {
        uiEquippedItem = GameObject.FindGameObjectWithTag("UIEquippedItem").GetComponent<UIEquippedItem>();
        itemDurability = GameObject.FindGameObjectWithTag("UIDurability").GetComponent<UIDurability>();
        durabilityArmor = itemDurability.armorCurrentDurability;
    }
    public void SetArmor(EquipableItemSO armorItemSO, List<ItemParameter> itemState)
    {
        if (armorItem != null)
        {
            inventoryData.AddItem(armorItem, 1, itemCurrentState);
        }

        this.armorItem = armorItemSO;
        this.itemCurrentState = new List<ItemParameter>(itemState);
        if (uiEquippedItem != null)
        {
            uiEquippedItem.armorImage.sprite = armorItemSO.ItemImage;
            uiEquippedItem.armorImage.gameObject.SetActive(true);
            itemDurability.armorCurrentDurability.sprite = armorItemSO.ItemImage;
            itemDurability.armorCurrentDurability.gameObject.SetActive(true);
        }
        ModifyParameters();
    }
    public void UnequipArmorAndAddToInventory()
    {
        if (armorItem != null && inventoryData != null)
        {
            // Gỡ bỏ vũ khí và thêm vào InventorySO
            inventoryData.AddItem(armorItem, 1, itemCurrentState);
            // Đặt vũ khí hiện tại thành null
            armorItem = null;
            // Ẩn hình ảnh vũ khí trong giao diện
            uiEquippedItem.armorImage.gameObject.SetActive(false);
            if (damageAble != null && defense != null)
            {
                defense.SetArmor(null, null);
                damageAble.SetHealthManaArmor(null, null);
            }
            // Thiết lập các tham số liên quan
            // ...
        }
    }
    private void ModifyParameters()
    {
        foreach (var parameter in parametersToModify)
        {
            if (itemCurrentState.Contains(parameter))
            {
                int index = itemCurrentState.IndexOf(parameter);
                float newValue = itemCurrentState[index].value + parameter.value;
                itemCurrentState[index] = new ItemParameter
                {
                    itemParameter = parameter.itemParameter,
                    value = newValue
                };
            }
        }
        if (defense != null && armorItem != null && damageAble != null)
        {
            defense.SetArmor(armorItem, itemCurrentState);
            damageAble.SetHealthManaArmor(armorItem,itemCurrentState); // Áp dụng các giá trị từ armorItem
        }
        UpdateDurabilityBar();
    }
    private void UpdateDurabilityBar()
    {
        if (durabilityArmor != null && armorItem != null)
        {
            var currentDurabilityParameter = itemCurrentState.Find(param => param.itemParameter is ItemParameterSO);
            float currentDurability = currentDurabilityParameter.itemParameter != null ? currentDurabilityParameter.value : 0f;

            var maxDurabilityParameter = armorItem.DefaultParameterList.Find(param => param.itemParameter is ItemParameterSO);
            float maxDurability = maxDurabilityParameter.itemParameter != null ? maxDurabilityParameter.value : 1f;

            durabilityArmor.fillAmount = currentDurability / maxDurability;
        }
        else
        {
            // Nếu cả weapon và durabilityWeapon đều bằng null, tắt hình ảnh của weaponCurrentDurability
            if (itemDurability != null && itemDurability.armorCurrentDurability != null)
            {
                itemDurability.armorCurrentDurability.gameObject.SetActive(false);
            }
        }
    }
    public void DecreaseDurability(float decreaseAmount)
    {
        var currentDurabilityParameter = itemCurrentState.Find(param => param.itemParameter is ItemParameterSO);
        float currentDurability = currentDurabilityParameter.itemParameter != null ? currentDurabilityParameter.value : 0f;
        if (currentDurability != 0)
        {
            currentDurabilityParameter.value -= decreaseAmount;
            int index = itemCurrentState.IndexOf(currentDurabilityParameter);
            itemCurrentState[index] = currentDurabilityParameter;
            UpdateDurabilityBar();
        }
    }
}