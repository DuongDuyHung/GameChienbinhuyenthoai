using Fusion;
using Inventory;
using Inventory.Model;
using Inventory.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIShop : MonoBehaviour
{
    public GameObject shopPanel;
    public ShopSO shopData;
    public InventorySO inventoryData;
    public RectTransform content;
    public UIInventoryItem itemPrefab;
    public UIInventoryDescription inventoryDescription;
    public Button buyButton;
    public TMP_Text price;
    public InventoryController inventoryController; // Thêm tham chiếu đến InventoryController
    private List<UIInventoryItem> shopItems = new List<UIInventoryItem>();
    [SerializeField] private float hideDelay = 1; // Thời gian chờ trước khi ẩn UIShop (giây)
    public NetworkObject networkObject;
    public bool IsOwner => networkObject.HasInputAuthority;
    // Start is called before the first frame update
    void Start()
    {
        if (shopData != null)
        {
            shopData.OnShopUpdated += RefreshUI;
            shopData.Initialize();
            CreateItemPrefabs();
            RefreshUI(shopData.GetCurrentShopState());
        }
        buyButton.onClick.AddListener(BuyButtonClicked);
    }
    private void OnDestroy()
    {
        if (shopData != null)
        {
            shopData.OnShopUpdated -= RefreshUI;
        }
        buyButton.onClick.RemoveListener(BuyButtonClicked);
    }
    private void BuyButtonClicked()
    {
        int selectedIndex = GetSelectedIndex();
        if (selectedIndex != -1)
        {
            var shopItemData = shopData.GetItemAt(selectedIndex);
            int itemPrice = shopItemData.item.coinValue;

            if (inventoryController.coinsValue >= itemPrice)
            {
                inventoryController.coinsValue -= itemPrice;
                inventoryController.UpdateCoinText();

                // Kiểm tra nếu item là Material
                if (shopItemData.item is ItemSO materialItem)
                {
                    if (materialItem.Name == "Gỗ")
                    {
                        inventoryController.AddMaterials(0, 1,0,0,0,0); // Add Woods
                        inventoryData.AddShopItemToInventory(shopItemData, 1);
                    }
                    else if (materialItem.Name == "Đá")
                    {
                        inventoryController.AddMaterials(1, 0,0,0,0,0); // Add Stones
                        inventoryData.AddShopItemToInventory(shopItemData, 1);
                    }
                    else if (materialItem.Name == "Đồng")
                    {
                        inventoryController.AddMaterials(0, 0,1,0,0,0); // Add Stones
                        inventoryData.AddShopItemToInventory(shopItemData, 1);
                    }
                    else if (materialItem.Name == "Bạc")
                    {
                        inventoryController.AddMaterials(0, 0,0,1,0,0); // Add Stones
                        inventoryData.AddShopItemToInventory(shopItemData, 1);
                    }
                    else if (materialItem.Name == "Vàng")
                    {
                        inventoryController.AddMaterials(0, 0,0,0,1,0); // Add Stones
                        inventoryData.AddShopItemToInventory(shopItemData, 1);
                    }
                    else if (materialItem.Name == "Đá Ngọc Bích")
                    {
                        inventoryController.AddMaterials(0, 0,0,0,0,1); // Add Stones
                        inventoryData.AddShopItemToInventory(shopItemData, 1);
                    }
                    else
                    {
                        inventoryData.AddShopItemToInventory(shopItemData, 1);
                    }

                }
                else
                {
                    // Nếu không phải Material, thêm vào Inventory thông thường
                    inventoryData.AddShopItemToInventory(shopItemData, 1);
                }
            }
            else
            {
                Debug.Log("Not enough coins!");
            }
        }
    }
    private int GetSelectedIndex()
    {
        for (int i = 0; i < shopItems.Count; i++)
        {
            if (shopItems[i].IsSelected)
            {
                return i;
            }
        }
        return -1; // Trả về -1 nếu không có mục nào được chọn
    }
    private void CreateItemPrefabs()
    {
        // Xóa các itemPrefab đã có trước đó
        foreach (UIInventoryItem item in shopItems)
        {
            Destroy(item.gameObject);
        }
        shopItems.Clear();

        // Tạo các itemPrefab mới dựa trên số lượng shopItems trong ShopSO
        int itemCount = shopData.Size;
        for (int i = 0; i < itemCount; i++)
        {
            UIInventoryItem newItem = Instantiate(itemPrefab, content);
            newItem.OnItemClicked += HandleItemClicked;
            shopItems.Add(newItem);
        }
    }
    private void HandleItemClicked(UIInventoryItem item)
    {
        foreach (UIInventoryItem inventoryItem in shopItems)
        {
            if (inventoryItem == item)
            {
                inventoryItem.SelectItem();
            }
            else
            {
                inventoryItem.DeselectItem();
            }
        }

        var shopItemData = shopData.GetItemAt(shopItems.IndexOf(item));
        string description = PrepareDescription(shopItemData);
        inventoryDescription.SetDescription(shopItemData.item.ItemImage, shopItemData.item.Name, description);
        // Set the coinValue to the price text
        price.text = shopItemData.item.coinValue.ToString();
    }
    private string PrepareDescription(ShopItem shopItem)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(shopItem.item.Description);
        sb.AppendLine();

        if (shopItem.item is EquipableItemSO equipableItem)
        {
            if (equipableItem.equipmentType == EquipableItemSO.EquipmentType.Weapon)
            {
                sb.Append($"Sát thương vật lý: {equipableItem.attackDamageModifier}");
                sb.AppendLine();
                sb.Append($"Sát thương phép: {equipableItem.magicDamageModifier}");
                sb.AppendLine();
                sb.Append($"Loại trang bị: Vũ khí");
                sb.AppendLine();
            }
            else if (equipableItem.equipmentType == EquipableItemSO.EquipmentType.Armor)
            {
                sb.Append($"Máu: {equipableItem.healthModifier}");
                sb.AppendLine();
                sb.Append($"Năng lượng: {equipableItem.manaModifier}");
                sb.AppendLine();
                sb.Append($"Giáp: {equipableItem.armorModifier}");
                sb.AppendLine();
                sb.Append($"Kháng phép: {equipableItem.magicResistanceModifier}");
                sb.AppendLine();
                sb.Append($"Loại trang bị: Giáp");
                sb.AppendLine();
            }
            else if (equipableItem.equipmentType == EquipableItemSO.EquipmentType.Boots)
            {
                sb.Append($"Máu: {equipableItem.healthModifier}");
                sb.AppendLine();
                sb.Append($"Năng lượng: {equipableItem.manaModifier}");
                sb.AppendLine();
                sb.Append($"Giáp: {equipableItem.armorModifier}");
                sb.AppendLine();
                sb.Append($"Kháng phép: {equipableItem.magicResistanceModifier}");
                sb.AppendLine();
                sb.Append($"Loại trang bị: Giày");
                sb.AppendLine();
            }
            else if (equipableItem.equipmentType == EquipableItemSO.EquipmentType.Helmet)
            {
                sb.Append($"Máu: {equipableItem.healthModifier}");
                sb.AppendLine();
                sb.Append($"Năng lượng: {equipableItem.manaModifier}");
                sb.AppendLine();
                sb.Append($"Giáp: {equipableItem.armorModifier}");
                sb.AppendLine();
                sb.Append($"Kháng phép: {equipableItem.magicResistanceModifier}");
                sb.AppendLine();
                sb.Append($"Loại trang bị: Nón");
                sb.AppendLine();
            }
            else if (equipableItem.equipmentType == EquipableItemSO.EquipmentType.Shield)
            {
                sb.Append($"Giáp: {equipableItem.armorModifier}");
                sb.AppendLine();
                sb.Append($"Kháng phép: {equipableItem.magicResistanceModifier}");
                sb.AppendLine();
                sb.Append($"Loại trang bị: Khiên");
                sb.AppendLine();
            }
            else if (equipableItem.equipmentType == EquipableItemSO.EquipmentType.Gloves)
            {
                sb.Append($"Sát thương vật lý: {equipableItem.attackDamageModifier}");
                sb.AppendLine();
                sb.Append($"Sát thương phép: {equipableItem.magicDamageModifier}");
                sb.AppendLine();
                sb.Append($"Loại trang bị: Găng tay");
                sb.AppendLine();
            }
            else if (equipableItem.equipmentType == EquipableItemSO.EquipmentType.Necklace)
            {
                sb.Append($"Sát thương vật lý: {equipableItem.attackDamageModifier}");
                sb.AppendLine();
                sb.Append($"Sát thương phép: {equipableItem.magicDamageModifier}");
                sb.AppendLine();
                sb.Append($"Loại trang bị: Dây chuyền");
                sb.AppendLine();
            }
            else if (equipableItem.equipmentType == EquipableItemSO.EquipmentType.Ring)
            {
                sb.Append($"Sát thương vật lý: {equipableItem.attackDamageModifier}");
                sb.AppendLine();
                sb.Append($"Sát thương phép: {equipableItem.magicDamageModifier}");
                sb.AppendLine();
                sb.Append($"Loại trang bị: Nhẫn");
                sb.AppendLine();
            }
        }

        for (int i = 0; i < shopItem.itemState.Count; i++)
        {
            sb.Append($"{shopItem.itemState[i].itemParameter.ParameterName}" + $":{shopItem.itemState[i].value}/" + $"{shopItem.item.DefaultParameterList[i].value}");
            sb.AppendLine();
        }
        return sb.ToString();
    }

    private void RefreshUI(Dictionary<int, ShopItem> shopItemsData)
    {
        for (int i = 0; i < shopItems.Count; i++)
        {
            if (shopItemsData.ContainsKey(i))
            {
                var shopItemData = shopItemsData[i];
                shopItems[i].SetData(shopItemData.item.ItemImage, shopItemData.quantity);
                shopItems[i].gameObject.SetActive(true);
            }
            else
            {
                shopItems[i].ResetData();
                shopItems[i].gameObject.SetActive(false);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        networkObject = GameObject.FindGameObjectWithTag("Playerr").GetComponent<NetworkObject>();
        if (IsOwner)
        {
            inventoryController = GameObject.FindGameObjectWithTag("Playerr").GetComponent<InventoryController>();

        }
        StartCoroutine(HideShopPanelAfterDelay());
    }
    private IEnumerator HideShopPanelAfterDelay()
    {
        // Chờ trong khoảng thời gian `hideDelay`
        yield return new WaitForSeconds(hideDelay);

        // Ẩn shopPanel sau thời gian chờ
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
            Debug.Log("UIShop has been hidden after " + hideDelay + " seconds.");
        }
    }
}
