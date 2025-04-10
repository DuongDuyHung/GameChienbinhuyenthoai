using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMap : MonoBehaviour
{
    public GameObject mapPanel;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CheckForPlayerAndDeactivate());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        
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
        if (mapPanel != null)
        {
            mapPanel.SetActive(true); // Hiển thị UICrafting
            yield return new WaitForSeconds(1); // Chờ 0.1 giây
            mapPanel.SetActive(false); // Tắt UICrafting
        }
    }
    // B?t UI Map
    public void ShowMap()
    {
        if (mapPanel != null)
        {
            mapPanel.SetActive(true);
        }
    }

    // T?t UI Map
    public void HideMap()
    {
        if (mapPanel != null)
        {
            mapPanel.SetActive(false);
        }
    }
}
