using System.Collections;
using UnityEngine;

public class ShopController : MonoBehaviour
{
    public UIShop uiShop; // Tham chiếu đến UIShop
    [SerializeField] private float hideDelay = 1; // Thời gian chờ trước khi ẩn UIShop (giây)

    void Start()
    {
        uiShop = GameObject.FindGameObjectWithTag("UIShop").GetComponent<UIShop>();

        if (uiShop != null && uiShop.shopPanel != null)
        {
            uiShop.shopPanel.SetActive(true); // Hiển thị shopPanel khi bắt đầu
            StartCoroutine(HideShopPanelAfterDelay()); // Bắt đầu coroutine để ẩn shopPanel
        }
        else
        {
            Debug.LogError("UIShop or shopPanel is not assigned!");
        }
    }

    private IEnumerator HideShopPanelAfterDelay()
    {
        // Chờ trong khoảng thời gian `hideDelay`
        yield return new WaitForSeconds(hideDelay);

        // Ẩn shopPanel sau thời gian chờ
        if (uiShop != null && uiShop.shopPanel != null)
        {
            uiShop.shopPanel.SetActive(false);
            Debug.Log("UIShop has been hidden after " + hideDelay + " seconds.");
        }
    }
}
