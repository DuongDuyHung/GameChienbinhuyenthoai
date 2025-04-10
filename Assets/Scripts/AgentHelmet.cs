using Inventory.Model;
using Inventory.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentHelmet : MonoBehaviour
{
    private Image content;
    [SerializeField]
    private Image durabilityHelmet;
    [SerializeField]
    public EquipableItemSO helmetItem;
    [SerializeField]
    public UIEquippedItem uiEquippedItem;
    [SerializeField]
    public UIDurability itemDurability;
    [SerializeField]
    private InventorySO inventoryData;
    [SerializeField]
    private Defense defense;
    [SerializeField]
    private DamageAble damageAble; // Thêm một tham chiếu đến DamageAble
    [SerializeField]
    public List<ItemParameter> parametersToModify, itemCurrentState;
    public void Start()
    {
        uiEquippedItem = GameObject.FindGameObjectWithTag("UIEquippedItem").GetComponent<UIEquippedItem>();
        itemDurability = GameObject.FindGameObjectWithTag("UIDurability").GetComponent<UIDurability>();
        durabilityHelmet = itemDurability.helmetCurrentDurability;
    }
    public void SetHelmet(EquipableItemSO helmetItemSO, List<ItemParameter> itemState)
    {
        if (helmetItem != null)
        {
            inventoryData.AddItem(helmetItem, 1, itemCurrentState);
        }

        this.helmetItem = helmetItemSO;
        this.itemCurrentState = new List<ItemParameter>(itemState);
        // Set image and activate weaponImage in UIEquippedItem
        if (uiEquippedItem != null)
        {
            uiEquippedItem.helmetImage.sprite = helmetItemSO.ItemImage;
            uiEquippedItem.helmetImage.gameObject.SetActive(true);
            itemDurability.helmetCurrentDurability.sprite = helmetItemSO.ItemImage;
            itemDurability.helmetCurrentDurability.gameObject.SetActive(true);
        }
        ModifyParameters();
    }
    public void UnequipHelmetAndAddToInventory()
    {
        if (helmetItem != null && inventoryData != null)
        {
            // Gỡ bỏ vũ khí và thêm vào InventorySO
            inventoryData.AddItem(helmetItem, 1, itemCurrentState);
            // Đặt vũ khí hiện tại thành null
            helmetItem = null;
            // Ẩn hình ảnh vũ khí trong giao diện
            uiEquippedItem.helmetImage.gameObject.SetActive(false);
            if(damageAble!= null && defense!=null)
            {
                defense.SetHelmet(null, null);
                damageAble.SetHealthManaHelmet(null, null);
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

        if (defense != null && helmetItem != null && damageAble != null)
        {
            defense.SetHelmet(helmetItem, itemCurrentState);
            damageAble.SetHealthManaHelmet(helmetItem,itemCurrentState); // Áp dụng các giá trị từ armorItem
        }
        UpdateDurabilityBar();
    }
    private void UpdateDurabilityBar()
    {
        if (durabilityHelmet != null && helmetItem != null)
        {
            var currentDurabilityParameter = itemCurrentState.Find(param => param.itemParameter is ItemParameterSO);
            float currentDurability = currentDurabilityParameter.itemParameter != null ? currentDurabilityParameter.value : 0f;

            var maxDurabilityParameter = helmetItem.DefaultParameterList.Find(param => param.itemParameter is ItemParameterSO);
            float maxDurability = maxDurabilityParameter.itemParameter != null ? maxDurabilityParameter.value : 1f;

            durabilityHelmet.fillAmount = currentDurability / maxDurability;
        }
        else
        {
            // Nếu cả weapon và durabilityWeapon đều bằng null, tắt hình ảnh của weaponCurrentDurability
            if (itemDurability != null && itemDurability.helmetCurrentDurability != null)
            {
                itemDurability.helmetCurrentDurability.gameObject.SetActive(false);
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
