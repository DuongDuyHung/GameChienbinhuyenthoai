using System.Collections;
using UnityEngine;

public class CraftingController : MonoBehaviour
{
    public UICrafting uiCrafting;

    void Start()
    {
        uiCrafting = GameObject.FindGameObjectWithTag("CraftingPanel").GetComponent<UICrafting>();
        StartCoroutine(CheckForPlayerAndDeactivate());
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
        if (uiCrafting != null)
        {
            uiCrafting.gameObject.SetActive(true); // Hiển thị UICrafting
            yield return new WaitForSeconds(1); // Chờ 0.1 giây
            uiCrafting.gameObject.SetActive(false); // Tắt UICrafting
        }
    }
}
