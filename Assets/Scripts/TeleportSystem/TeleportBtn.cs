using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportBtn : MonoBehaviour
{
    public GameObject[] players; // Mảng để chứa các GameObject của nhân vật
    public Vector3 targetPosition = new Vector3(0, 0, 0); // Vị trí đích XYZ

    // Start is called before the first frame update
    void Start()
    {
        // Tìm tất cả các đối tượng có tên "Player(Clone)" trong scene
        players = GameObject.FindGameObjectsWithTag("Playerr"); // Hoặc dùng Find nếu không có tag

        if (players.Length == 0)
        {
            Debug.LogWarning("No Player objects found with tag 'Playerr'.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Có thể kiểm tra thêm các điều kiện nếu cần
        // Tìm tất cả các đối tượng có tên "Player(Clone)" trong scene
        players = GameObject.FindGameObjectsWithTag("Playerr"); // Hoặc dùng Find nếu không có tag

        if (players.Length == 0)
        {
            Debug.LogWarning("No Player objects found with tag 'Playerr'.");
        }
    }

    // Phương thức dịch chuyển nhân vật
    public void TeleportPlayer()
    {
        if (players != null && players.Length > 0)
        {
            // Dịch chuyển chỉ một player (ví dụ: player[0])
            GameObject playerToTeleport = players[0]; // Bạn có thể thay đổi chỉ số này để chọn player khác

            if (playerToTeleport != null)
            {
                playerToTeleport.transform.position = targetPosition;
            }
            else
            {
                Debug.LogWarning("Selected player is null!");
            }
        }
        else
        {
            Debug.LogWarning("No Player reference is found!");
        }
    }
}
