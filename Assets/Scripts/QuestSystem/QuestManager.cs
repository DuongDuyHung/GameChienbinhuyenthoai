using Inventory;
using Inventory.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public QuestDialog questDialog;
    public QuestUIManager questUIManager;
    public InventoryController inventoryController;
    public List<Quest> quests;
    public int currentQuestIndex; // Track the current quest index
    // Start is called before the first frame update
    void Start()
    {
        inventoryController = GameObject.FindGameObjectWithTag("Playerr").GetComponent<InventoryController>();
        questUIManager = GameObject.FindGameObjectWithTag("QuestPanel").GetComponent<QuestUIManager>();
        questDialog = GetComponent<QuestDialog>();
        // Example to set a quest dialog for the first quest
        if (quests.Count > 0 && currentQuestIndex < quests.Count)
        {
            questDialog.SetQuest(quests[currentQuestIndex]); // Set the first quest for demonstration
        }
    }
    public void AcceptQuest(Quest quest)
    {
        // Reset enemiesDefeated when accepting the quest
        quest.enemiesDefeated = 0;
        // Update the UI Manager with quest details
        if (questUIManager != null)
        {
            questUIManager.UpdateQuestUI(quest);
        }
        Debug.Log($"Quest accepted: {quest.title}");
    }
    public void DefeatEnemy(Quest quest)
    {
        // Increment the defeated enemies count
        quest.enemiesDefeated++;

        // Update the UI if necessary
        if (questUIManager != null)
        {
            questUIManager.UpdateQuestUI(quest);
        }

        // Check if the quest is complete
        if (quest.enemiesDefeated >= quest.enemiesToDefeat)
        {
            CompleteQuest(quest);
        }
    }
    public void CompleteQuest(Quest quest)
    {
        // Update the UI Manager with quest details
        if (questUIManager != null)
        {
            questUIManager.UpdateQuestUI(quest);
        }
        // Move to the next quest
        currentQuestIndex++;
        if (currentQuestIndex < quests.Count)
        {
            // Set the next quest dialog
            questDialog.SetQuest(quests[currentQuestIndex]);
        }
        else
        {
            Debug.Log("All quests completed!");
        }
        // Add rewards to inventory
        AddRewardsToInventory(quest);
        // Add any additional logic for completing the quest here
        Debug.Log($"Quest completed: {quest.title}");
    }
    private void AddRewardsToInventory(Quest quest)
    {
        // Add coins to inventory
        if (quest.coinReward > 0)
        {
            inventoryController.AddCoins(quest.coinReward);
        }

        // Add consumable item reward
        if (quest.consumeItemRewards != null)
        {
            foreach (ConsumeItemSO consumeItem in quest.consumeItemRewards)
            {
                inventoryController.inventoryData.AddItem(consumeItem, 1);
            }
        }

        // Add equipable item reward
        if (quest.equipableItemRewards != null)
        {
            foreach (EquipableItemSO equipableItem in quest.equipableItemRewards)
            {
                inventoryController.inventoryData.AddItem(equipableItem, 1);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        inventoryController = GameObject.FindGameObjectWithTag("Playerr").GetComponent<InventoryController>();
        questUIManager = GameObject.FindGameObjectWithTag("QuestPanel").GetComponent<QuestUIManager>();
        questDialog = GetComponent<QuestDialog>();
        // Example to set a quest dialog for the first quest
        if (quests.Count > 0 && currentQuestIndex < quests.Count)
        {
            questDialog.SetQuest(quests[currentQuestIndex]); // Set the first quest for demonstration
        }
    }
}