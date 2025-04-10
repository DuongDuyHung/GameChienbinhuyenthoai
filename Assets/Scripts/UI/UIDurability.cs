using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.UI
{
    public class UIDurability : MonoBehaviour
    {
        public Image weaponCurrentDurability;
        public Image helmetCurrentDurability;
        public Image armorCurrentDurability;
        public Image bootsCurrentDurability;
        public AgentWeapon weapon;
        public AgentArmor armor;
        public AgentHelmet helmet;
        public AgentBoots boots;
        private bool hasSpawnedCharacter = false;

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(AutoDisableImagesAfterSpawn());
        }

        private IEnumerator AutoDisableImagesAfterSpawn()
        {
            // Chờ nhân vật được spawn qua NetworkManager
            yield return new WaitUntil(() => HasCharacterSpawned());

            // Đợi thêm 1 giây sau khi spawn
            yield return new WaitForSeconds(1f);

            // Tắt các hình ảnh
            DisableDurabilityImages();
        }

        private bool HasCharacterSpawned()
        {
            // Kiểm tra logic spawn của nhân vật
            // Ví dụ: kiểm tra xem object Player(Clone) đã xuất hiện trong scene hay chưa
            GameObject player = GameObject.FindGameObjectWithTag("Playerr");
            if (player != null && !hasSpawnedCharacter)
            {
                hasSpawnedCharacter = true;
                return true;
            }
            return false;
        }

        private void DisableDurabilityImages()
        {
            if (weaponCurrentDurability != null)
                weaponCurrentDurability.enabled = false;

            if (helmetCurrentDurability != null)
                helmetCurrentDurability.enabled = false;

            if (armorCurrentDurability != null)
                armorCurrentDurability.enabled = false;

            if (bootsCurrentDurability != null)
                bootsCurrentDurability.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            weapon = GameObject.FindGameObjectWithTag("Playerr").GetComponent<AgentWeapon>();
            helmet = GameObject.FindGameObjectWithTag("Playerr").GetComponent<AgentHelmet>();
            armor = GameObject.FindGameObjectWithTag("Playerr").GetComponent<AgentArmor>();
            boots = GameObject.FindGameObjectWithTag("Playerr").GetComponent<AgentBoots>();

        }
    }
}