using Fusion;
using Inventory.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.UI
{
    public class UIInventoryPage : MonoBehaviour
    {
        [SerializeField]
        private PerkUIManager perkUIManager;
        [SerializeField]
        private UIInventoryItem itemPrefab;
        [SerializeField]
        private RectTransform contentPanel;
        [SerializeField]
        private InventorySO inventoryData;
        [SerializeField]
        private UIInventoryDescription itemDescription;
        [SerializeField]
        private UIIventoryCharacterInfo characterInfo;
        [SerializeField]
        private MouseFollower mouseFollower;
        private List<UIInventoryItem> items = new List<UIInventoryItem>();
        private int currentlyDraggedItemIndex = -1;
        public event Action<EquipableItemSO> OnItemDropped;
        public event Action<int> OnDescriptionRequest, OnItemActionRequested, OnStartDragging;
        public event Action<int, int> OnSwapItems;
        [SerializeField]
        private ItemActionPanel actionPanel;
        public NetworkObject networkObject;
        public bool IsOwner => networkObject.HasInputAuthority;
        public void Awake()
        {
            Hide();
            mouseFollower.Toggle(false);
            itemDescription.ResetDescription();
        }
        private void Update()
        {
            networkObject = GameObject.FindGameObjectWithTag("Playerr").GetComponent<NetworkObject>();
            if (IsOwner)
            {
                
                perkUIManager.perks = GameObject.FindGameObjectWithTag("Playerr").GetComponent<Perks>();
                perkUIManager.damageAble = GameObject.FindGameObjectWithTag("Playerr").GetComponent<DamageAble>();
                perkUIManager.attackPlayer = GameObject.FindGameObjectWithTag("Playerr").GetComponent<AttackPlayer>();
                perkUIManager.defense = GameObject.FindGameObjectWithTag("Playerr").GetComponent<Defense>();
            }
            
        }
        public void InitializeInventoryUI(int inventorysize)
        {
            for (int i = 0; i < inventorysize; i++)
            {
                UIInventoryItem uiItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
                uiItem.transform.SetParent(contentPanel);
                items.Add(uiItem);
                uiItem.OnItemClicked += HandleItemSelection;
                uiItem.OnItemDroppedOn += HandleSwap;
                uiItem.OnItemBeginDrag += HandleBeginDrag;
                uiItem.OnItemEndDrag += HandleEndDrag;
                uiItem.OnRightMouseBtnClick += HandleShowItemActions;
            }
        }
        private void HandleItemDrop(EquipableItemSO item)
        {
            OnItemDropped?.Invoke(item);

            // Tái tạo prefab và đặt lại vật phẩm trong scene
            if (item != null)
            {
                // Tạo lại prefab của vật phẩm
                GameObject newItemPrefab = Instantiate(item.prefab);

                // Đặt lại vị trí của prefab trong scene
                newItemPrefab.transform.position = Vector3.zero; // Ví dụ: Đặt lại vị trí là Vector3.zero

                // Các xử lý khác (nếu cần)
                // ...

                // Cập nhật dữ liệu mới cho vật phẩm trong UI (nếu cần)
                // ...

                // Gọi các phương thức hoặc hàm xử lý khác (nếu cần)
                // ...
            }
        }
        // Thêm một phương thức để cập nhật thông tin nhân vật
        public void UpdateCharacterInfo(int level,int experience,int experienceToNextLevel,int totalAttackDamage, int totalMagicDamage, int currentArmor, int currentMagicResistance, int currentHealth, int MaxHealth, float currentMana, int MaxMana)
        {
            characterInfo.UpdateCharacterInfo(level,experience,experienceToNextLevel,totalAttackDamage, totalMagicDamage, currentArmor, currentMagicResistance, currentHealth, MaxHealth, currentMana, MaxMana);
        }

        public void UpdateData(int itemIndex, Sprite itemImage, int itemQuantity)
        {
            if (items.Count > itemIndex)
            {
                items[itemIndex].SetData(itemImage, itemQuantity);
            }
        }

        private void HandleShowItemActions(UIInventoryItem inventoryItemUI)
        {
            int index = items.IndexOf(inventoryItemUI);
            if (index == -1)
            {
                return;
            }
            OnItemActionRequested?.Invoke(index);
        }
        private void HandleEndDrag(UIInventoryItem inventoryItemUI)
        {
            ResetDraggedItem();
        }

        private void HandleBeginDrag(UIInventoryItem inventoryItemUI)
        {
            int index = items.IndexOf(inventoryItemUI);
            if (index == -1)
                return;
            currentlyDraggedItemIndex = index;
            HandleItemSelection(inventoryItemUI);
            OnStartDragging?.Invoke(index);
        }

        public void CreateDraggedItem(Sprite sprite, int quantity)
        {
            mouseFollower.Toggle(true);
            mouseFollower.SetData(sprite, quantity);
        }

        private void HandleSwap(UIInventoryItem inventoryItemUI)
        {
            int index = items.IndexOf(inventoryItemUI);
            if (index == -1)
            {
                return;
            }
            OnSwapItems?.Invoke(currentlyDraggedItemIndex, index);
            HandleItemSelection(inventoryItemUI);
        }

        private void ResetDraggedItem()
        {
            mouseFollower.Toggle(false);
            currentlyDraggedItemIndex = -1;
        }

        private void HandleItemSelection(UIInventoryItem inventoryItemUI)
        {
            int index = items.IndexOf(inventoryItemUI);
            if (index == -1)
                return;
            OnDescriptionRequest?.Invoke(index);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            itemDescription.ResetDescription();
            ResetSelection();
            // Khởi tạo UI cho InventorySO
            InitializeInventoryUI(inventoryData.Size, contentPanel);
        }

        public void ResetSelection()
        {
            itemDescription.ResetDescription();
            DeselectAllItems();
        }
        public void AddAction(string actionName, Action performAction)
        {
            actionPanel.AddButton(actionName, performAction);
        }
        public void ShowItemAction(int itemIndex)
        {
            actionPanel.Toggle(true);
            actionPanel.transform.position = items[itemIndex].transform.position;
        }

        private void DeselectAllItems()
        {
            foreach (UIInventoryItem item in items)
            {
                item.Deselect();
            }
            actionPanel.Toggle(false);
        }

        public void Hide()
        {
            actionPanel.Toggle(false);
            ResetDraggedItem();
        }

        internal void UpdateDescription(int itemIndex, Sprite itemImage, string name, string description)
        {
            itemDescription.SetDescription(itemImage, name, description);
            DeselectAllItems();
            items[itemIndex].Select();
        }

        internal void ResetAllItems()
        {
            foreach (var item in items)
            {
                item.ResetData();
                item.Deselect();
            }
        }
        public void InitializeInventoryUI(int inventorysize, RectTransform parentTransform)
        {
            for (int i = 0; i < inventorysize; i++)
            {
                UIInventoryItem uiItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity, parentTransform);
                items.Add(uiItem);
                uiItem.OnItemClicked += HandleItemSelection;
                uiItem.OnItemDroppedOn += HandleSwap;
                uiItem.OnItemBeginDrag += HandleBeginDrag;
                uiItem.OnItemEndDrag += HandleEndDrag;
                uiItem.OnRightMouseBtnClick += HandleShowItemActions;
            }
        }

    }
}
