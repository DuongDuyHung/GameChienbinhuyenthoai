using System.Collections.Generic;
using UnityEngine;
using static Inventory.Model.EquipableItemSO;

namespace Inventory.Model
{
    [CreateAssetMenu]
    public class EquipableItemSO : ItemSO, IDestroyableItem, IItemAction
    {
        public string ActionName => "Trang bị";
        [field: SerializeField]
        public AudioClip actionSFX { get; private set; }
        [SerializeField]
        public int attackDamageModifier = 0;
        [SerializeField]
        public int magicDamageModifier = 0;
        [SerializeField]
        public int armorModifier = 0;
        [SerializeField]
        public int magicResistanceModifier = 0;
        [SerializeField]
        public int healthModifier = 0;
        [SerializeField]
        public int manaModifier = 0;
        [SerializeField]
        public EquipmentType equipmentType;
        [SerializeField]
        public GameObject prefab; // Thêm trường prefab
        public enum EquipmentType
        {
            Armor,
            Weapon,
            Helmet,
            Boots,
            Shield,
            Necklace,
            Ring,
            Gloves
        }
        public bool PerformAction(GameObject character, List<ItemParameter> itemState = null)
        {
            if (equipmentType == EquipmentType.Weapon)
            {
                AgentWeapon agentWeapon = character.GetComponent<AgentWeapon>();
                if (agentWeapon != null)
                {
                    agentWeapon.SetWeapon(this, itemState);
                    return true;
                }
            }
            else if (equipmentType == EquipmentType.Armor)
            {
                AgentArmor agentArmor = character.GetComponent<AgentArmor>();
                if (agentArmor != null)
                {
                    agentArmor.SetArmor(this, itemState);
                    return true;
                }
            }
            else if(equipmentType == EquipmentType.Helmet)
            {
                AgentHelmet agentHelmet = character.GetComponent<AgentHelmet>();
                if(agentHelmet != null)
                {
                    agentHelmet.SetHelmet(this, itemState);
                    return true;
                }
            }
            else if (equipmentType == EquipmentType.Boots)
            {
                AgentBoots agentBoots = character.GetComponent<AgentBoots>();
                if (agentBoots != null)
                {
                    agentBoots.SetBoots(this, itemState);
                    return true;
                }
            }

            return false;
        }
    }
}
