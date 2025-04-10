using Inventory.Model;
using UnityEngine;

public class DroppedItemScript : MonoBehaviour
{
    public ItemSO item;  // Thông tin của item

    private bool isPlayerInRange; // Biến để kiểm tra xem người chơi có gần item không

    private void Update()
    {
        // Kiểm tra xem người chơi có gần item không và nếu người chơi nhấn một phím (ví dụ: 'E') để nhặt item
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            // Gọi hàm để nhặt item
            PickUpItem();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra nếu người chơi vào vùng tiếp xúc với item
        if (collision.CompareTag("Playerr"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Kiểm tra nếu người chơi ra khỏi vùng tiếp xúc với item
        if (collision.CompareTag("Playerr"))
        {
            isPlayerInRange = false;
        }
    }

    private void PickUpItem()
    {
        // Thêm item vào inventory của người chơi
        // Tùy thuộc vào cấu trúc của inventory của bạn, bạn có thể cần thêm một phương thức hoặc sự kiện để thêm item
        // ví dụ: PlayerInventory.Instance.AddItem(item);

        // Sau khi nhặt item, hủy GameObject đại diện cho item
        Destroy(gameObject);
    }
}
