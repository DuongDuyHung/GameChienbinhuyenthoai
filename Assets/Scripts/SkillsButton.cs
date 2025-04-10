using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillsButton : MonoBehaviour
{
    public GameObject skillsPanel; // Kéo và thả Panel của skills vào đây từ inspector
    private bool isSkillsOpen = false;
    // Start is called before the first frame update
    void Start()
    {

    }
    public void ToggleSkills()
    {
        isSkillsOpen = !isSkillsOpen;
        skillsPanel.SetActive(isSkillsOpen);
        // Dừng hoặc tiếp tục hoạt động của scene
        if (isSkillsOpen)
        {
            Time.timeScale = 0f; // Dừng toàn bộ hoạt động
        }
        else
        {
            Time.timeScale = 1f; // Tiếp tục hoạt động
        }
    }

    void Update()
    {
        // Kiểm tra xem phím K đã được nhấn chưa
        if (Input.GetKeyDown(KeyCode.K))
        {
            ToggleSkills(); // Gọi phương thức ToggleSkills khi nhấn phím K
        }
    }
}
