using Inventory.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.UI
{
    public class UIEquippedItem : MonoBehaviour
    {
        public Image weaponImage;
        public Image helmetImage;
        public Image armorImage;
        public Image bootsImage;
        public Button btnUnequipWeapon;
        public Button btnUnequiphelmet;
        public Button btnUnequipArmor;
        public Button btnUnequipBoots;
        public AgentWeapon agentWeapon; // Thêm một thuộc tính agentWeapon
        public AgentHelmet agentHelmet; // Thêm một thuộc tính agentHelmet
        public AgentArmor agentArmor;
        public AgentBoots agentBoots;
        // Start is called before the first frame update
        void Start()
        {
            btnUnequipWeapon.onClick.AddListener(UnequipWeapon);
            btnUnequiphelmet.onClick.AddListener(UnequipHelmet); // Gắn sự kiện cho nút UnequipHelmet
            btnUnequipArmor.onClick.AddListener(UnequipArmor);
            btnUnequipBoots.onClick.AddListener(UnequipBoots);
        }
        void UnequipWeapon()
        {
            if (agentWeapon != null)
            {
                agentWeapon.UnequipWeaponAndAddToInventory();
            }
        }
        void UnequipHelmet()
        {
            if (agentHelmet != null)
            {
                agentHelmet.UnequipHelmetAndAddToInventory(); // Gọi phương thức UnequipHelmet tương tự như UnequipWeapon
            }
        }
        void UnequipArmor()
        {
            if (agentArmor != null)
            {
                agentArmor.UnequipArmorAndAddToInventory();
            }
        }
        void UnequipBoots()
        {
            if (agentBoots != null)
            {
                agentBoots.UnequipBootsAndAddToInventory();
            }
        }

        // Update is called once per frame
        void Update()
        {
            agentWeapon = GameObject.FindGameObjectWithTag("Playerr").GetComponent<AgentWeapon>();
            agentHelmet = GameObject.FindGameObjectWithTag("Playerr").GetComponent<AgentHelmet>();
            agentArmor = GameObject.FindGameObjectWithTag("Playerr").GetComponent<AgentArmor>();
            agentBoots = GameObject.FindGameObjectWithTag("Playerr").GetComponent<AgentBoots>();
        }
    }
}