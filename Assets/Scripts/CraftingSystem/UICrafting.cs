using Fusion;
using Inventory;
using Inventory.Model;
using Inventory.UI;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class UICrafting : MonoBehaviour
{
    public GameObject craftingPanel;
    public InventorySO inventoryData;
    public InventoryController inventoryController;
    public UIInventoryItem itemPrefab;
    public RectTransform content;
    public Button craftButton;
    public UICraftingDescription craftingDescription;
    private List<UIInventoryItem> craftItems = new List<UIInventoryItem>();
    public List<CraftingRecipe> craftingRecipes = new List<CraftingRecipe>();
    public NetworkObject networkObject;
    public bool IsOwner => networkObject.HasInputAuthority;
    private bool CanCraftItem(CraftingRecipe recipe)
    {
        return inventoryController.woods >= recipe.requiredWoods
               &&
               inventoryController.stones >= recipe.requiredStones
               &&
               inventoryController.silver >= recipe.requiredSilver
               &&
               inventoryController.copper >= recipe.requiredCopper
               &&
               inventoryController.gold >= recipe.requiredGold
               &&
               inventoryController.saphire >= recipe.requiredSaphire;
    }
    private void DisplayCraftingItems()
    {
        foreach (CraftingRecipe recipe in craftingRecipes)
        {
            UIInventoryItem item = Instantiate(itemPrefab, content); // Instantiate UIInventoryItem in content
            item.SetItem(recipe.itemToCraft.ItemImage); // Assuming SetItem is a method that configures the item
            item.OnItemClicked += HandleItemClicked; // Add the click handler for the item
            craftItems.Add(item); // Add to list for further management
        }
    }
    private void CraftButtonClicked()
    {
        int selectedIndex = GetSelectedIndex();
        if (selectedIndex != -1)
        {
            CraftingRecipe selectedRecipe = craftingRecipes[selectedIndex];

            if (CanCraftItem(selectedRecipe))
            {
                inventoryController.woods -= selectedRecipe.requiredWoods;
                inventoryController.stones -= selectedRecipe.requiredStones;
                inventoryController.copper -= selectedRecipe.requiredCopper;
                inventoryController.silver -= selectedRecipe.requiredSilver;
                inventoryController.gold -= selectedRecipe.requiredGold;
                inventoryController.saphire -= selectedRecipe.requiredSaphire;
                DeductItemQuantity("Gỗ", selectedRecipe.requiredWoods);
                DeductItemQuantity("Đá", selectedRecipe.requiredStones);
                DeductItemQuantity("Đồng", selectedRecipe.requiredCopper);
                DeductItemQuantity("Bạc", selectedRecipe.requiredSilver);
                DeductItemQuantity("Vàng", selectedRecipe.requiredGold);
                DeductItemQuantity("Đá Ngọc Bích", selectedRecipe.requiredSaphire);

                // Add the crafted item to inventory
                inventoryData.AddShopItemToInventory(new ShopItem
                {
                    item = selectedRecipe.itemToCraft,
                    quantity = 1
                }, 1);

                Debug.Log("Item crafted successfully!");
            }
            else
            {
                Debug.Log("Not enough materials!");
            }
        }
    }
    private void DeductItemQuantity(string itemName, int amount)
    {
        for (int i = 0; i < inventoryData.inventoryItems.Count; i++)
        {
            var shopItem = inventoryData.inventoryItems[i];
            if (shopItem.item.Name.Trim() == itemName.Trim()) // Đảm bảo không có khoảng trắng
            {
                shopItem.quantity -= amount;
                if (shopItem.quantity <= 0)
                {
                    shopItem.quantity = 0;
                }
                inventoryData.inventoryItems[i] = shopItem;  // Cập nhật vật phẩm
                Debug.Log($"Đã giảm {amount} từ {itemName}. Số lượng còn lại: {shopItem.quantity}");
                break; // Thoát vòng lặp sau khi tìm thấy vật phẩm
            }
        }
    }

    private int GetSelectedIndex()
    {
        for (int i = 0; i < craftItems.Count; i++)
        {
            if (craftItems[i].IsSelected)
            {
                return i;
            }
        }
        return -1; // Return -1 if no item is selected
    }
    private void HandleItemClicked(UIInventoryItem item)
    {
        foreach (UIInventoryItem inventoryItem in craftItems)
        {
            if (inventoryItem == item)
            {
                inventoryItem.SelectItem(); // Mark as selected
            }
            else
            {
                inventoryItem.DeselectItem(); // Deselect others
            }
        }

        var recipe = craftingRecipes[craftItems.IndexOf(item)];

        // Define the material requirement string
        string materialRequirement = $"Yêu cầu: {inventoryController.woods}/{recipe.requiredWoods} gỗ, {inventoryController.stones}/{recipe.requiredStones} đá,{inventoryController.copper}/{recipe.requiredCopper} Đồng, {inventoryController.silver}/{recipe.requiredSilver} Bạc,{inventoryController.gold}/{recipe.requiredGold} Vàng, {inventoryController.saphire}/{recipe.requiredSaphire} Đá Ngọc Bích";

        // Set description with material requirement and item description
        string description = PrepareItemDescription(recipe.itemToCraft); // Assuming this is a function that provides the item details

        // Pass all details to the UICraftingDescription
        craftingDescription.SetDescription(recipe.itemToCraft.Name, description, materialRequirement, recipe.itemToCraft.ItemImage);
    }
    private string PrepareItemDescription(ItemSO item)
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(item.Description);
        sb.AppendLine();

        if (item is EquipableItemSO equipableItem)
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

        // Add any other item-specific details here

        return sb.ToString();
    }

    private void RefreshCraftingUI()
    {
        for (int i = 0; i < craftItems.Count; i++)
        {
            if (i < craftingRecipes.Count)
            {
                var recipe = craftingRecipes[i];
                craftItems[i].SetData(recipe.itemToCraft.ItemImage, 1);
                craftItems[i].gameObject.SetActive(true);
            }
            else
            {
                craftItems[i].ResetData();
                craftItems[i].gameObject.SetActive(false);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        craftButton.onClick.AddListener(CraftButtonClicked);
        DisplayCraftingItems();
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(CheckForPlayerAndDeactivate());
        networkObject = GameObject.FindGameObjectWithTag("Playerr").GetComponent<NetworkObject>();
        if (IsOwner)
        {
            inventoryController = GameObject.FindGameObjectWithTag("Playerr").GetComponent<InventoryController>();
        }
        
    }
    IEnumerator CheckForPlayerAndDeactivate()
    {
        GameObject player = null;

        // Chờ đến khi đối tượng Player được spawn
        while (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Playerr");
            yield return null; // Chờ frame tiếp theo
        }

        // Khi Player được tìm thấy, bật UICrafting và bắt đầu đếm ngược
        if (craftingPanel != null)
        {
            craftingPanel.SetActive(true); // Hiển thị UICrafting
            yield return new WaitForSeconds(1); // Chờ 0.1 giây
            craftingPanel.SetActive(false); // Tắt UICrafting
        }
    }
}
[System.Serializable]
public struct CraftingRecipe
{
    public ItemSO itemToCraft;
    public int requiredStones;
    public int requiredWoods;
    public int requiredCopper;
    public int requiredGold;
    public int requiredSilver;
    public int requiredSaphire;
}

