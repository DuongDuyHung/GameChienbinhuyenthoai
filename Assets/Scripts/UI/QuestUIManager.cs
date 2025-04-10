using Inventory.Model;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestUIManager : MonoBehaviour
{
    public TMP_Text questTitle;
    public TMP_Text questDescription;
    // Reference to the QuestManager
    public QuestManager questManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCurrentQuestUI();
    }
    private void UpdateCurrentQuestUI()
    {
        if (questManager.currentQuestIndex < questManager.quests.Count)
        {
            Quest currentQuest = questManager.quests[questManager.currentQuestIndex];
            UpdateQuestUI(currentQuest);
        }
    }
    public void UpdateQuestUI(Quest quest)
    {
        questTitle.text = quest.title;

        // Initialize reward information
        string rewardInfo = "Phần thưởng: ";

        // Check if there are any rewards and append them accordingly
        bool hasRewards = false;

        // Check for equipable item reward
        if (quest.equipableItemRewards != null && quest.equipableItemRewards.Count > 0)
        {
            foreach (EquipableItemSO equipableItem in quest.equipableItemRewards)
            {
                if (hasRewards)
                {
                    rewardInfo += "\n"; // Add a new line if there are already rewards listed
                }
                rewardInfo += equipableItem.name;
                hasRewards = true;
            }
        }

        // Check for consumable item reward
        if (quest.consumeItemRewards != null && quest.consumeItemRewards.Count > 0)
        {
            foreach (ConsumeItemSO consumeItem in quest.consumeItemRewards)
            {
                if (hasRewards)
                {
                    rewardInfo += "\n"; // Add a new line if there are already rewards listed
                }
                rewardInfo += consumeItem.name;
                hasRewards = true;
            }
        }

        // Check for coin reward
        if (quest.coinReward > 0)
        {
            if (hasRewards)
            {
                rewardInfo += "\n"; // Add a comma if there are already rewards listed
            }
            rewardInfo += quest.coinReward + " Vàng";
        }

        // Append enemy defeat status
        string enemyStatus = $"Kẻ địch bị đánh bại: {quest.enemiesDefeated}/{quest.enemiesToDefeat}";
        questDescription.text = $"{quest.description}\n{rewardInfo}\n{enemyStatus}";

    }

}
