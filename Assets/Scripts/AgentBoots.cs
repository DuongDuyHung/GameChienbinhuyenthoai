using Inventory.Model;
using Inventory.UI;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AgentBoots : MonoBehaviour
{
    private Image content;
    [SerializeField]
    private Image durabilityBoots;
    [SerializeField]
    public EquipableItemSO bootsItem;
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
        durabilityBoots = itemDurability.bootsCurrentDurability;
    }
    // Hàm này được gọi khi thiết lập giày bảo hộ mới
    public void SetBoots(EquipableItemSO bootsItemSO, List<ItemParameter> itemState)
    {
        // Nếu đã có giày bảo hộ, thêm nó vào kho
        if (bootsItem != null)
        {
            inventoryData.AddItem(bootsItem, 1, itemCurrentState);
        }
        
        // Cập nhật giày bảo hộ mới và trạng thái mục
        this.bootsItem = bootsItemSO;
        this.itemCurrentState = new List<ItemParameter>(itemState);
        if (uiEquippedItem != null)
        {
            uiEquippedItem.bootsImage.sprite = bootsItemSO.ItemImage;
            uiEquippedItem.bootsImage.gameObject.SetActive(true);
            itemDurability.bootsCurrentDurability.sprite = bootsItemSO.ItemImage;
            itemDurability.bootsCurrentDurability.gameObject.SetActive(true);
        }
        ModifyParameters();
    }
    public void UnequipBootsAndAddToInventory()
    {
        if (bootsItem != null && inventoryData != null)
        {
            // Gỡ bỏ vũ khí và thêm vào InventorySO
            inventoryData.AddItem(bootsItem, 1, itemCurrentState);
            // Đặt vũ khí hiện tại thành null
            bootsItem = null;
            // Ẩn hình ảnh vũ khí trong giao diện
            uiEquippedItem.bootsImage.gameObject.SetActive(false);
            if (damageAble != null && defense != null)
            {
                defense.SetBoots(null, null);
                damageAble.SetHealthManaBoots(null, null);
            }
            // Thiết lập các tham số liên quan
            // ...
        }
    }
    // Hàm này sẽ điều chỉnh các thông số cho giày bảo hộ
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

        // Nếu có đối tượng Defense và giày bảo hộ, cập nhật giá trị
        if (defense != null && bootsItem != null && damageAble != null)
        {
            defense.SetBoots(bootsItem, itemCurrentState);
            damageAble.SetHealthManaBoots(bootsItem,itemCurrentState);
        }
        UpdateDurabilityBar();
    }
    private void UpdateDurabilityBar()
    {
        if (durabilityBoots != null && bootsItem != null)
        {
            var currentDurabilityParameter = itemCurrentState.Find(param => param.itemParameter is ItemParameterSO);
            float currentDurability = currentDurabilityParameter.itemParameter != null ? currentDurabilityParameter.value : 0f;

            var maxDurabilityParameter = bootsItem.DefaultParameterList.Find(param => param.itemParameter is ItemParameterSO);
            float maxDurability = maxDurabilityParameter.itemParameter != null ? maxDurabilityParameter.value : 1f;

            durabilityBoots.fillAmount = currentDurability / maxDurability;
        }
        else
        {
            // Nếu cả weapon và durabilityWeapon đều bằng null, tắt hình ảnh của weaponCurrentDurability
            if (itemDurability != null && itemDurability.bootsCurrentDurability != null)
            {
                itemDurability.bootsCurrentDurability.gameObject.SetActive(false);
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
