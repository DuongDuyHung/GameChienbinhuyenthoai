using Inventory.Model;
using Inventory.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentWeapon : MonoBehaviour
{
    private Image content;
    [SerializeField]
    private Image durabilityWeapon;
    [SerializeField]
    public EquipableItemSO weapon;
    [SerializeField]
    public UIEquippedItem uiEquippedItem;
    [SerializeField]
    public UIDurability itemDurability;
    [SerializeField]
    private InventorySO inventoryData;
    [SerializeField]
    private AttackPlayer attackPlayer;
    [SerializeField]
    public List<ItemParameter> parametersToModify, itemCurrentState;
    public void Start()
    {
        uiEquippedItem = GameObject.FindGameObjectWithTag("UIEquippedItem").GetComponent<UIEquippedItem>();
        itemDurability = GameObject.FindGameObjectWithTag("UIDurability").GetComponent<UIDurability>();
        durabilityWeapon = itemDurability.weaponCurrentDurability;
    }
    public void SetWeapon(EquipableItemSO weaponItemSO, List<ItemParameter> itemState)
    {
        if (weapon != null)
        {
            inventoryData.AddItem(weapon, 1, itemCurrentState);
        }
        this.weapon = weaponItemSO;
        this.itemCurrentState = new List<ItemParameter>(itemState);
        // Set image and activate weaponImage in UIEquippedItem
        if (uiEquippedItem != null)
        {
            uiEquippedItem.weaponImage.sprite = weaponItemSO.ItemImage;
            uiEquippedItem.weaponImage.gameObject.SetActive(true);
            itemDurability.weaponCurrentDurability.sprite = weaponItemSO.ItemImage;
            itemDurability.weaponCurrentDurability.gameObject.SetActive(true);
        }
        ModifyParameters();
    }
    public void UnequipWeaponAndAddToInventory()
    {
        if (weapon != null && inventoryData != null)
        {
            // Gỡ bỏ vũ khí và thêm vào InventorySO
            inventoryData.AddItem(weapon, 1, itemCurrentState);
            // Đặt vũ khí hiện tại thành null
            weapon = null;
            // Ẩn hình ảnh vũ khí trong giao diện
            uiEquippedItem.weaponImage.gameObject.SetActive(false);
            // Đặt attackDamageModifier và magicDamageModifier về 0
            if (attackPlayer != null)
            {
                attackPlayer.ApplyItemParameters(0, 0);
            }
            // Thiết lập các tham số liên quan
            // ...
        }
    }
    public void DecreaseParameter()
    {
        foreach (var parameter in parametersToModify)
        {
            if (itemCurrentState.Contains(parameter) && weapon != null)
            {
                int index = itemCurrentState.IndexOf(parameter);
                float newValue = itemCurrentState[index].value - 1; // Giảm 50 điểm từ giá trị hiện tại
                itemCurrentState[index] = new ItemParameter
                {
                    itemParameter = parameter.itemParameter,
                    value = newValue
                };
            }
        }

        // Cập nhật lại thanh độ bền
        UpdateDurabilityBar();

        // Cập nhật attackPlayer dựa trên giá trị hiện tại của currentDurability
        float currentDurability = GetCurrentDurability();
        if (attackPlayer != null && weapon != null)
        {
            if (currentDurability > 0)
            {
                attackPlayer.ApplyItemParameters(weapon.attackDamageModifier, weapon.magicDamageModifier);
            }
            else
            {
                // Nếu độ bền là 0, đặt attackDamageModifier và magicDamageModifier về 0
                attackPlayer.ApplyItemParameters(0, 0);
            }
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

        // Kiểm tra độ bền hiện tại
        float currentDurability = GetCurrentDurability();

        if (attackPlayer != null && weapon != null)
        {
            if (currentDurability > 0)
            {
                attackPlayer.ApplyItemParameters(weapon.attackDamageModifier, weapon.magicDamageModifier);
            }
            else
            {
                // Nếu độ bền là 0, đặt attackDamageModifier và magicDamageModifier về 0
                attackPlayer.ApplyItemParameters(0, 0);
            }
        }

        UpdateDurabilityBar();
    }
    private float GetCurrentDurability()
    {
        // Tìm ItemParameter có itemParameter là độ bền trong itemCurrentState
        var currentDurabilityParameter = itemCurrentState.Find(param => param.itemParameter is ItemParameterSO);
        return currentDurabilityParameter.itemParameter != null ? currentDurabilityParameter.value : 0f;
    }

    private void UpdateDurabilityBar()
    {
        if (durabilityWeapon != null && weapon != null)
        {
            // Tìm ItemParameter có itemParameter là độ bền trong itemCurrentState
            var currentDurabilityParameter = itemCurrentState.Find(param => param.itemParameter is ItemParameterSO);
            float currentDurability = currentDurabilityParameter.itemParameter != null ? currentDurabilityParameter.value : 0f;

            // Tìm ItemParameter có itemParameter là độ bền trong DefaultParameterList
            var maxDurabilityParameter = weapon.DefaultParameterList.Find(param => param.itemParameter is ItemParameterSO);
            float maxDurability = maxDurabilityParameter.itemParameter != null ? maxDurabilityParameter.value : 1f; // Sửa dòng này để lấy giá trị từ DefaultParameterList

            durabilityWeapon.fillAmount = currentDurability / maxDurability;
        }
        else
        {
            // Nếu cả weapon và durabilityWeapon đều bằng null, tắt hình ảnh của weaponCurrentDurability
            if (itemDurability != null && itemDurability.weaponCurrentDurability != null)
            {
                itemDurability.weaponCurrentDurability.gameObject.SetActive(false);
            }
        }
    }
}