using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InventoryButton : MonoBehaviour
{
    public GameObject inventoryPanel; // Kéo và thả Panel của inventory vào đây từ inspector
    private bool isInventoryOpen = false;

    void Start()
    {
        StartCoroutine(AutoDisableUIInventoryPage());
    }
    private IEnumerator AutoDisableUIInventoryPage()
    {
        // Đợi cho đến khi "Player" xuất hiện trong scene
        GameObject player = null;
        while (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Playerr");
            yield return null; // Chờ frame tiếp theo
        }

        // Bắt đầu tính thời gian sau khi "Playerr" đã xuất hiện
        if (inventoryPanel.activeSelf)
        {
            yield return new WaitForSeconds(1f); // Chờ 1 giây
            inventoryPanel.SetActive(false);
            Time.timeScale = 1;
        }
    }
    public void ToggleInventory()
    {
        if (inventoryPanel != null)
        {
            isInventoryOpen = !isInventoryOpen;
            inventoryPanel.SetActive(isInventoryOpen);
            if (isInventoryOpen)
            {
                Time.timeScale = 1f;
            }
            else
            {
                Time.timeScale = 1f;
            }
        }
        else
        {
            Debug.LogError("Inventory panel is not assigned in InventoryButton script.");
        }
    }
    private void Update()
    {

    }
}
