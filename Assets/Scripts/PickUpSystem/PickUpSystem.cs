using Inventory.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpSystem : MonoBehaviour
{
    [SerializeField]
    private InventorySO invetoryData;
    [SerializeField]
    private LevelAndExperience playerLevel; // Tham chiếu đến LevelAndExperience

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Item item = collision.GetComponent<Item>();
        if (item != null)
        {
            int reminder = invetoryData.AddItem(item.InventoryItem, item.Quantity);
            if (reminder == 0)
            {
                item.DestroyItem();

                // Tăng kinh nghiệm cho người chơi khi nhặt vật phẩm
                if (playerLevel != null)
                {
                    int experienceGained = CalculateExperienceGained(item); // Tính toán kinh nghiệm nhận được từ vật phẩm
                    playerLevel.AddExperience(experienceGained); // Thêm kinh nghiệm cho người chơi
                }
            }
            else
            {
                item.Quantity = reminder;
            }
        }
    }

    // Hàm tính toán kinh nghiệm nhận được từ vật phẩm
    private int CalculateExperienceGained(Item item)
    {
        // Bạn có thể thêm logic tính toán dựa trên loại và số lượng của vật phẩm
        // Ví dụ: một vật phẩm có giá trị kinh nghiệm cố định hoặc dựa trên mức độ hiếm có của nó

        int baseExperience = 10; // Kinh nghiệm cơ bản từ mỗi vật phẩm
        int experiencePerQuantity = 30; // Kinh nghiệm từng đơn vị số lượng

        int totalExperience = baseExperience + (item.Quantity * experiencePerQuantity);

        return totalExperience;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
