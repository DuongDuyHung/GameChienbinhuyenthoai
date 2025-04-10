using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;
using Inventory.Model;

namespace Inventory.UI
{
    public class UIInventoryItem : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDropHandler, IDragHandler
    {
        [SerializeField]
        public Image itemImage;
        [SerializeField]
        private TMP_Text quantityTxt;
        [SerializeField]
        private Image borderImage;
        public bool IsSelected { get; private set; }
        public event Action<UIInventoryItem> OnItemClicked, OnItemDroppedOn, OnItemBeginDrag, OnItemEndDrag, OnRightMouseBtnClick;

        public bool empty = true;
        public void SelectItem()
        {
            IsSelected = true;
            // Cập nhật giao diện để hiển thị rằng mục đã được chọn
        }

        public void DeselectItem()
        {
            IsSelected = false;
            // Cập nhật giao diện để hiển thị rằng mục không được chọn nữa
        }
        public void Awake()
        {
            ResetData();
            Deselect();
        }

        public void ResetData()
        {
            if (itemImage != null)
            {
                itemImage.gameObject.SetActive(false);
            }
            if (quantityTxt != null)
            {
                quantityTxt.text = "";
            }
            empty = true;
        }

        public void Deselect()
        {
            if (borderImage != null)
            {
                borderImage.enabled = false;
            }
        }
        public void UpdateItemImage(Sprite sprite)
        {
            if (itemImage != null)
            {
                itemImage.gameObject.SetActive(true);
                itemImage.sprite = sprite;
            }
        }
        private UIInventoryItem GetUIInventoryItemAt(int slotIndex)
        {
            // Assuming you have a parent GameObject that contains all the UIInventoryItem instances
            Transform parentTransform = transform.parent;

            // Check if the slotIndex is valid
            if (slotIndex >= 0 && slotIndex < parentTransform.childCount)
            {
                // Get the child at the specified slotIndex
                Transform childTransform = parentTransform.GetChild(slotIndex);
                if (childTransform != null)
                {
                    // Get the UIInventoryItem component from the child GameObject
                    UIInventoryItem uiItem = childTransform.GetComponent<UIInventoryItem>();
                    return uiItem;
                }
            }

            return null;
        }
        public void SetData(Sprite sprite, int quantity)
        {
            if (itemImage != null)
            {
                itemImage.gameObject.SetActive(true);
                itemImage.sprite = sprite;
            }

            if (quantityTxt != null)
            {
                quantityTxt.text = quantity + "";
            }

            empty = false;
        }

        public void Select()
        {
            if (borderImage != null)
            {
                borderImage.enabled = true;
            }
        }
        public void SetItem(Sprite sprite)
        {
            UpdateItemImage(sprite);
            empty = false;
        }

        //public void OnPointerClick(PointerEventData pointerData)
        //{
        //    if (pointerData.button == PointerEventData.InputButton.Right)
        //    {
        //        OnRightMouseBtnClick?.Invoke(this);
        //    }
        //    else
        //    {
        //        OnItemClicked?.Invoke(this);
        //    }
        //}
        public void OnPointerClick(PointerEventData pointerData)
        {
            if (pointerData.button == PointerEventData.InputButton.Right)
            {
                OnRightMouseBtnClick?.Invoke(this);
            }
            else if (pointerData.button == PointerEventData.InputButton.Left)
            {
                OnItemClicked?.Invoke(this);
            }
        }


        private void OnDestroy()
        {
            OnItemClicked = null;
            OnItemDroppedOn = null;
            OnItemBeginDrag = null;
            OnItemEndDrag = null;
            OnRightMouseBtnClick = null;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (empty) return;
            OnItemBeginDrag?.Invoke(this);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            OnItemEndDrag?.Invoke(this);
        }

        public void OnDrop(PointerEventData eventData)
        {
            OnItemDroppedOn?.Invoke(this);
        }

        public void OnDrag(PointerEventData eventData)
        {
        }

    }
}
