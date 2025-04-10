using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

namespace Inventory.UI
{
    public class UIIventoryCharacterInfo : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text playerStats;

        // Cập nhật thông tin nhân vật
        public void UpdateCharacterInfo(int level, int experience,int experienceToNextLevel, int totalAttackDamage, int totalMagicDamage, int currentArmor, int currentMagicResistance, int currentHealth, int MaxHealth, float currentMana, int MaxMana)
        {
            playerStats.text = $"Cấp: {level}\n" +
                               $"Kinh nghiệm: {experience}/{experienceToNextLevel}\n" +
                               $"Máu: {currentHealth}/{MaxHealth}\n" +
                               $"Năng lượng: {currentMana}/{MaxMana}\n" +
                               $"T.công v.lý: {totalAttackDamage}\n" +
                               $"T.công phép: {totalMagicDamage}\n" +
                               $"Giáp: {currentArmor}\n" +
                               $"Kháng phép: {currentMagicResistance}\n";
        }
    }
}
