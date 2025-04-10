using Inventory.Model;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public string title;
    public string description;
    public int coinReward;
    public List<EquipableItemSO> equipableItemRewards;
    public List<ConsumeItemSO> consumeItemRewards;
    public int enemiesToDefeat;
    public int enemiesDefeated;
}
