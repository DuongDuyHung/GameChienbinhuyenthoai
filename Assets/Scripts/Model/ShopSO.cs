using Inventory.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopSO : ScriptableObject
{
    [SerializeField]
    private List<ShopItem> shopItems = new List<ShopItem>();
    [field: SerializeField]
    public int Size { get; private set; } = 10;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public event Action<Dictionary<int, ShopItem>> OnShopUpdated;
    //public void Initialize()
    //{
    //    inventoryItems = new List<InventoryItem>();
    //    for (int i = 0; i < Size; i++)
    //    {
    //        inventoryItems.Add(InventoryItem.GetEmptyItem());
    //    }
    //}
    public void SetShopSize(int newSize)
    {
        shopItems.Clear();
        for (int i = 0; i < newSize; i++)
        {
            shopItems.Add(ShopItem.GetEmptyItem());
        }
    }
    //public void Initialize()
    //{
    //    // Chỉ khởi tạo danh sách mục nếu chưa có
    //    if (shopItems == null)
    //    {
    //        shopItems = new List<ShopItem>();
    //        for (int i = 0; i < Size; i++)
    //        {
    //            shopItems.Add(ShopItem.GetEmptyItem());
    //        }
    //    }
    //}
    public void Initialize()
    {
        if (shopItems == null || shopItems.Count == 0)
        {
            shopItems = new List<ShopItem>();
            for (int i = 0; i < Size; i++)
            {
                shopItems.Add(ShopItem.GetEmptyItem());
            }
        }
        InformAboutChange();
    }
    public int AddItem(ItemSO item, int quantity, List<ItemParameter> itemState = null)
    {
        if (item.IsStackable == false)
        {
            for (int i = 0; i < shopItems.Count; i++)
            {
                while (quantity > 0 && IsShopFull() == false)
                {
                    quantity -= AddItemToFirstFreeSlot(item, 1, itemState);
                }
                //InformAboutChange();
                //return quantity;
            }
        }
        quantity = AddStackableItem(item, quantity);
        InformAboutChange();
        return quantity;
    }

    private int AddItemToFirstFreeSlot(ItemSO item, int quantity, List<ItemParameter> itemState = null)
    {
        ShopItem newItem = new ShopItem
        {
            item = item,
            quantity = quantity,
            itemState = new List<ItemParameter>(itemState == null ? item.DefaultParameterList : itemState)
        };
        for (int i = 0; i < shopItems.Count; i++)
        {
            if (shopItems[i].IsEmpty)
            {
                shopItems[i] = newItem;
                return quantity;
            }
        }
        return 0;
    }

    private bool IsShopFull()
    => shopItems.Where(item => item.IsEmpty).Any() == false;
    private int AddStackableItem(ItemSO item, int quantity)
    {
        for (int i = 0; i < shopItems.Count; i++)
        {
            if (shopItems[i].IsEmpty)
                continue;
            if (shopItems[i].item.ID == item.ID)
            {
                int amountPossibleToTake = shopItems[i].item.MaxStackSize - shopItems[i].quantity;
                if (quantity > amountPossibleToTake)
                {
                    shopItems[i] = shopItems[i].ChangeQuantity(shopItems[i].item.MaxStackSize);
                    quantity -= amountPossibleToTake;
                }
                else
                {
                    shopItems[i] = shopItems[i].ChangeQuantity(shopItems[i].quantity + quantity);
                    InformAboutChange();
                    return 0;
                }
            }
        }
        while (quantity > 0 && IsShopFull() == false)
        {
            int newQuantity = Mathf.Clamp(quantity, 0, item.MaxStackSize);
            quantity -= newQuantity;
            AddItemToFirstFreeSlot(item, newQuantity);
        }
        return quantity;
    }

    public Dictionary<int, ShopItem> GetCurrentShopState()
    {
        Dictionary<int, ShopItem> returnValue = new Dictionary<int, ShopItem>();
        for (int i = 0; i < shopItems.Count; i++)
        {
            if (shopItems[i].IsEmpty)
                continue;
            returnValue[i] = shopItems[i];
        }
        return returnValue;
    }

    internal ShopItem GetItemAt(int itemIndex)
    {
        return shopItems[itemIndex];
    }

    public void AddItem(ShopItem item)
    {
        AddItem(item.item, item.quantity);
    }

    public void SwapItems(int itemIndex_1, int itemIndex_2)
    {
        ShopItem item1 = shopItems[itemIndex_1];
        shopItems[itemIndex_1] = shopItems[itemIndex_2];
        shopItems[itemIndex_2] = item1;
        InformAboutChange();
    }

    private void InformAboutChange()
    {
        OnShopUpdated?.Invoke(GetCurrentShopState());
    }

    public void RemoveItem(int itemIndex, int amount)
    {
        if (shopItems.Count > itemIndex)
        {
            if (shopItems[itemIndex].IsEmpty)
                return;
            int reminder = shopItems[itemIndex].quantity - amount;
            if (reminder <= 0)
                shopItems[itemIndex] = ShopItem.GetEmptyItem();
            else
                shopItems[itemIndex] = shopItems[itemIndex].ChangeQuantity(reminder);
            InformAboutChange();
        }
    }
}
[Serializable]
public struct ShopItem
{
    public int quantity;
    public ItemSO item;
    public List<ItemParameter> itemState;
    public float cooldown; // thời gian cooldown tính bằng giây
    public bool IsEmpty => item == null;
    public ShopItem ChangeQuantity(int newQuantity)
    {
        return new ShopItem
        {
            item = this.item,
            quantity = newQuantity,
            itemState = new List<ItemParameter>(this.itemState)
        };
    }
    public static ShopItem GetEmptyItem() => new ShopItem
    {
        item = null,
        quantity = 0,
        itemState = new List<ItemParameter>()
    };
}