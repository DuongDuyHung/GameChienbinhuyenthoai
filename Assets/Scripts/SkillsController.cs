using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillsController : MonoBehaviour
{
    [SerializeField]
    private GameObject skillsPanel;
    [SerializeField] private float hideDelay = 1; // Thời gian chờ trước khi ẩn UIShop (giây)
    private void Start()
    {
        skillsPanel = GameObject.FindGameObjectWithTag("SkillPanel");
        if (skillsPanel != null)
        {
            skillsPanel.SetActive(true); // Hiển thị shopPanel khi bắt đầu
            StartCoroutine(HideShopPanelAfterDelay()); // Bắt đầu coroutine để ẩn shopPanel
        }
        else
        {
            Debug.LogError("Skill Panel is not assigned!");
        }
    }
    private IEnumerator HideShopPanelAfterDelay()
    {
        // Chờ trong khoảng thời gian `hideDelay`
        yield return new WaitForSeconds(hideDelay);

        // Ẩn shopPanel sau thời gian chờ
        if (skillsPanel != null)
        {
            skillsPanel.SetActive(false);
            Debug.Log("Skill Panel has been hidden after " + hideDelay + " seconds.");
        }
    }
    void Update()
    {
        // Kiểm tra xem phím K đã được nhấn chưa
        if (Input.GetKeyDown(KeyCode.K))
        {
            // Nếu nhấn phím K, hiển thị skillsPanel (nếu chưa được hiển thị)
            if (!skillsPanel.activeSelf)
            {
                skillsPanel.SetActive(true);
            }
            else
            {
                // Ngược lại, nếu skillsPanel đã hiển thị, tắt nó đi
                skillsPanel.SetActive(false);
            }
        }
    }
}
