using Inventory.Model;
using Inventory.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace Inventory
{
    public class InventoryController : MonoBehaviour
    {
        [SerializeField]
        private UIInventoryPage inventoryUI;
        [SerializeField]
        public InventorySO inventoryData;
        public List<InventoryItem> initialItems = new List<InventoryItem>();
        [SerializeField]
        private AudioClip dropClip;
        [SerializeField]
        private AudioSource audioSource;
        [SerializeField]
        private TMP_Text coinText;
        public int coinsValue;
        public int stones;
        public int woods;
        public int copper;
        public int silver;
        public int gold;
        public int saphire;
        //public int inventorySize = 10;
        // Start is called before the first frame update
        void Start()
        {
            coinText = GameObject.FindGameObjectWithTag("Coin").GetComponent<TMP_Text>();
            inventoryUI = GameObject.FindGameObjectWithTag("UIInventoryPage").GetComponent<UIInventoryPage>();
            PrepareUI();
            PrepareInventoryData();
            UpdateCoinText();
        }
        public void AddCoins(int amount)
        {
            coinsValue += amount;
            UpdateCoinText();
        }
        public void AddMaterials(int stoneAmount, int woodAmount,
        int copperAmount, int silverAmount, int goldAmount, int saphireAmount)
        {
            stones += stoneAmount;
            woods += woodAmount;
            copper += copperAmount;
            silver += silverAmount;
            gold += goldAmount;
            saphire += saphireAmount;
        }
        public void UpdateCoinText()
        {
            coinText.text = coinsValue.ToString();
        }
        private void PrepareInventoryData()
        {
            inventoryData.Initialize();
            inventoryData.OnInventoryUpdated += UpdateInventoryUI;
            foreach (InventoryItem item in initialItems)
            {
                if (item.IsEmpty)
                    continue;
                inventoryData.AddItem(item);
            }
        }

        private void UpdateInventoryUI(Dictionary<int, InventoryItem> inventoryState)
        {
            inventoryUI.ResetAllItems();
            foreach (var item in inventoryState)
            {
                inventoryUI.UpdateData(item.Key, item.Value.item.ItemImage, item.Value.quantity);
            }
        }

        private void PrepareUI()
        {
            inventoryUI.InitializeInventoryUI(inventoryData.Size);
            inventoryUI.OnDescriptionRequest += HandleDescriptionRequest;
            inventoryUI.OnSwapItems += HandleSwapItems;
            inventoryUI.OnStartDragging += HandleDragging;
            inventoryUI.OnItemActionRequested += HandleItemActionRequest;
        }

        private void HandleItemActionRequest(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;
            IItemAction itemAction = inventoryItem.item as IItemAction;
            if (itemAction != null)
            {
                inventoryUI.ShowItemAction(itemIndex);
                inventoryUI.AddAction(itemAction.ActionName, () => PerformAction(itemIndex));
            }
            IDestroyableItem destroyableItem = inventoryItem.item as IDestroyableItem;
            if (destroyableItem != null)
            {
                inventoryUI.AddAction("Bỏ", () => DropItem(itemIndex, inventoryItem.quantity));
            }
        }
        public void DropItem(int itemIndex, int quantity)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;

            ItemSO itemSO = inventoryItem.item;
            if (itemSO != null)
            {
                // Lấy vị trí hiện tại của nhân vật
                Vector3 playerPosition = transform.position;

                // Thiết lập khoảng cách giữa nhân vật và vật phẩm theo trục x
                float dropDistanceX = 2.0f; // Khoảng cách muốn vật phẩm rơi xa nhân vật theo trục x

                // Tạo vị trí mới cho vật phẩm
                Vector3 itemPosition = playerPosition + new Vector3(dropDistanceX, 0f, 0f);

                // Kiểm tra nếu là ConsumeItemSO, tạo prefab và thiết lập vị trí
                if (itemSO is ConsumeItemSO consumeItemSO)
                {
                    GameObject itemPrefab = Instantiate(consumeItemSO.prefab, itemPosition, Quaternion.identity);
                    // Các xử lý khác (nếu cần)
                    // ...
                }
                // Kiểm tra nếu là EquipableItemSO, tạo prefab và thiết lập vị trí
                else if (itemSO is EquipableItemSO equipableItemSO)
                {
                    GameObject itemPrefab = Instantiate(equipableItemSO.prefab, itemPosition, Quaternion.identity);
                    // Các xử lý khác (nếu cần)
                    // ...
                }

                // Xóa vật phẩm khỏi InventorySO
                inventoryData.RemoveItem(itemIndex, 1);
            }

            inventoryUI.ResetSelection();
            audioSource.PlayOneShot(dropClip);
        }
        public void PerformAction(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;
            IDestroyableItem destroyableItem = inventoryItem.item as IDestroyableItem;
            if (destroyableItem != null)
            {
                inventoryData.RemoveItem(itemIndex, 1);
            }
            IItemAction itemAction = inventoryItem.item as IItemAction;
            if (itemAction != null)
            {
                itemAction.PerformAction(gameObject, inventoryItem.itemState);
                audioSource.PlayOneShot(itemAction.actionSFX);
                if (inventoryData.GetItemAt(itemIndex).IsEmpty)
                    inventoryUI.ResetSelection();
            }
        }

        private void HandleDragging(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
                return;
            inventoryUI.CreateDraggedItem(inventoryItem.item.ItemImage, inventoryItem.quantity);
        }

        private void HandleSwapItems(int itemIndex_1, int itemIndex_2)
        {
            inventoryData.SwapItems(itemIndex_1, itemIndex_2);
        }

        private void HandleDescriptionRequest(int itemIndex)
        {
            InventoryItem inventoryItem = inventoryData.GetItemAt(itemIndex);
            if (inventoryItem.IsEmpty)
            {
                inventoryUI.ResetSelection();
                return;
            }
            ItemSO item = inventoryItem.item;
            string description = PrepareDescription(inventoryItem);
            inventoryUI.UpdateDescription(itemIndex, item.ItemImage, item.name, description);
        }

        //private string PrepareDescription(InventoryItem inventoryItem)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    sb.Append(inventoryItem.item.Description);
        //    sb.AppendLine();
        //    for(int i = 0; i < inventoryItem.itemState.Count; i++)
        //    {
        //        sb.Append($"{inventoryItem.itemState[i].itemParameter.ParameterName}" + $":{inventoryItem.itemState[i].value}/" + $"{inventoryItem.item.DefaultParameterList[i].value}");
        //        sb.AppendLine();
        //    }
        //    return sb.ToString();   
        //}
        private string PrepareDescription(InventoryItem inventoryItem)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(inventoryItem.item.Description);
            sb.AppendLine();

            if (inventoryItem.item is EquipableItemSO equipableItem)
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

            for (int i = 0; i < inventoryItem.itemState.Count; i++)
            {
                sb.Append($"{inventoryItem.itemState[i].itemParameter.ParameterName}" + $":{inventoryItem.itemState[i].value}/" + $"{inventoryItem.item.DefaultParameterList[i].value}");
                sb.AppendLine();
            }
            return sb.ToString();
        }


        // Update is called once per frame
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.I))
            {
                if (inventoryUI.isActiveAndEnabled == false)
                {
                    inventoryUI.Show();
                    foreach (var item in inventoryData.GetCurrentInventoryState())
                    {
                        inventoryUI.UpdateData(item.Key, item.Value.item.ItemImage, item.Value.quantity);
                    }
                }
                else
                {
                    inventoryUI.Hide();
                }
            }
        }
    }
}