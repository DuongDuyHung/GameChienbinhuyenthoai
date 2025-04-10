using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Inventory;
using Inventory.Model;
using Inventory.UI;

public class PlayerDataSaver : MonoBehaviour
{
    public static string filePath = @"C:\SaveLoadTest\player_save.json";
    public int comboAttackLevel;
    public int castAttackLevel;
    public int doubleJumpLevel;
    public SkillLearnButton[] skillLearnButtons;
    public InventoryController inventoryController;
    // Cập nhật phương thức này để thiết lập tham chiếu đến SkillLearnButton
    void Start()
    {
        // Tìm kiếm SkillLearnButton theo tag và gán vào mảng
        skillLearnButtons = new SkillLearnButton[3]; // Giả sử bạn có 3 SkillLearnButtons
        skillLearnButtons[0] = GameObject.FindGameObjectWithTag("Skill1")?.GetComponent<SkillLearnButton>();
        skillLearnButtons[1] = GameObject.FindGameObjectWithTag("Skill2")?.GetComponent<SkillLearnButton>();
        skillLearnButtons[2] = GameObject.FindGameObjectWithTag("Skill3")?.GetComponent<SkillLearnButton>();
        // Tìm đối tượng có tag "Playerr"
        GameObject playerObject = GameObject.FindWithTag("Playerr");

        if (playerObject != null)
        {
            // Lấy LevelAndExperience và PlayerDataSaver từ đối tượng có tag "Playerr"
            skillLearnButtons[0].playerLevel = playerObject.GetComponent<LevelAndExperience>();
            skillLearnButtons[0].playerDataSaver = playerObject.GetComponent<PlayerDataSaver>();
            skillLearnButtons[1].playerLevel = playerObject.GetComponent<LevelAndExperience>();
            skillLearnButtons[1].playerDataSaver = playerObject.GetComponent<PlayerDataSaver>();
            skillLearnButtons[2].playerLevel = playerObject.GetComponent<LevelAndExperience>();
            skillLearnButtons[2].playerDataSaver = playerObject.GetComponent<PlayerDataSaver>();

            if (skillLearnButtons[1].playerLevel == null && skillLearnButtons[1].playerLevel == null
                && skillLearnButtons[1].playerLevel == null)
            {
                Debug.LogError("Player object does not have LevelAndExperience component.");
            }

            if (skillLearnButtons[1].playerDataSaver == null && skillLearnButtons[1].playerDataSaver == null
                && skillLearnButtons[1].playerDataSaver == null)
            {
                Debug.LogError("Player object does not have PlayerDataSaver component.");
            }
        }
        else
        {
            Debug.LogError("Player object not found in scene.");
        }

        // Tìm đối tượng có tag "Skill1" và lấy AttackPlayer
        GameObject skill1Object = GameObject.FindWithTag("Skill1");
        if (skill1Object != null)
        {
            skillLearnButtons[0].attackPlayer = playerObject.GetComponent<AttackPlayer>();
            if (skillLearnButtons[0].attackPlayer == null)
            {
                Debug.LogError("Skill1 object does not have AttackPlayer component.");
            }
        }
        else
        {
            Debug.LogError("Skill1 object not found in scene.");
        }

        // Tìm đối tượng có tag "Skill2" và lấy AttackPlayer và Cast
        GameObject skill2Object = GameObject.FindWithTag("Skill2");
        if (skill2Object != null)
        {
            skillLearnButtons[1].attackPlayer = playerObject.GetComponent<AttackPlayer>();
            // Tìm object có tag "Combo" nằm trong Player
            Transform comboObject = playerObject.transform.Find("Combo");
            if (comboObject != null)
            {
                skillLearnButtons[1].attack = comboObject.GetComponent<Attack>();
                if (skillLearnButtons[1].attack == null)
                {
                    Debug.LogError("Combo object does not have Attack component.");
                }
            }
            else
            {
                Debug.LogError("Combo object not found in Player.");
            }
        }
        else
        {
            Debug.LogError("Skill2 object not found in scene.");
        }

        // Tìm đối tượng có tag "Skill3" (nếu cần thêm logic xử lý riêng)
        GameObject skill3Object = GameObject.FindWithTag("Skill3");
        if (skill3Object != null)
        {
            // Thêm xử lý cho Skill3 nếu cần
        }
        else
        {
            Debug.LogError("Skill3 object not found in scene.");
        }
        // Kiểm tra xem tất cả các nút kỹ năng có được gán đúng không
        for (int i = 0; i < skillLearnButtons.Length; i++)
        {
            if (skillLearnButtons[i] == null)
            {
                Debug.LogError($"SkillLearnButton với tag Skill{i + 1} không được tìm thấy hoặc không có thành phần SkillLearnButton.");
            }
        }
        // Load vị trí của nhân vật từ tệp JSON
        if (inventoryController.inventoryData != null && inventoryController != null)
        {
            inventoryController.inventoryData.SetInventorySize(0);
            inventoryController.inventoryData.SetInventorySize(60);
            inventoryController.inventoryData.Initialize();
        }
        else
        {
            Debug.Log("No inventoryController and inventoryData");
        }
        LoadPlayerData();
    }

    //void OnDestroy()
    //{
    //    // Lưu vị trí của nhân vật vào tệp JSON trước khi đối tượng bị hủy
    //    SavePlayerData();
    //}

    public void SavePlayerData()
    {
        Vector3 playerPosition = gameObject.transform.position;
        LevelAndExperience levelAndExp = GetComponent<LevelAndExperience>();
        AttackPlayer attackPlayer = GetComponent<AttackPlayer>();
        Defense defense = GetComponent<Defense>();
        Perks perks = GetComponent<Perks>();
        PlayerController playerController = GetComponent<PlayerController>();
        DamageAble damageAble = GetComponent<DamageAble>();
        AgentWeapon weapon = GetComponent<AgentWeapon>();
        AgentArmor armor = GetComponent<AgentArmor>();
        AgentBoots boots = GetComponent<AgentBoots>();
        AgentHelmet helmet = GetComponent<AgentHelmet>();
        QuestManager questManager = FindAnyObjectByType<QuestManager>();
        if (levelAndExp != null && attackPlayer != null && perks != null 
            && playerController != null && defense != null && damageAble != null
            && inventoryController != null && inventoryController.inventoryData != null
            && questManager != null)
        {
            Dictionary<int, InventoryItem> inventoryState = inventoryController.inventoryData.GetCurrentInventoryState();
            List<InventoryItem> inventoryItems = new List<InventoryItem>(inventoryState.Values);
            PlayerData playerData = new PlayerData();
            playerData.position = playerPosition;
            playerData.level = levelAndExp.level;
            playerData.experience = levelAndExp.experience;
            playerData.experienceToNextLevel = levelAndExp.experienceToNextLevel;
            playerData.skillPoints = levelAndExp.skillPoints;
            playerData.canCastAttack = playerController.canCastAttack;
            playerData.canCastComboAttack = playerController.canCastComboAttack;
            playerData.canDoubleJump = playerController.canDoubleJump;
            playerData.baseAttackDamage = attackPlayer.baseAttackDamage;
            playerData.baseMagicDamage = attackPlayer.baseMagicDamage;
            playerData.totalAttackDamage = attackPlayer.totalAttackDamage;
            playerData.totalMagicDamage = attackPlayer.totalMagicDamage;
            playerData.baseArmor = defense.baseArmor;
            playerData.armorFromPerk = defense.armorFromPerk;
            playerData.baseMagicResistance = defense.baseMagicResistance;
            playerData.magicResistanceFromPerk = defense.magicResistanceFromPerk;
            playerData.Health = damageAble.Health;
            playerData.MaxHealth = damageAble._maxHealth;
            playerData.Mana = damageAble.Mana;
            playerData.MaxMana = damageAble._maxMana;
            playerData.perksData = new PerksData();
            playerData.perksData.perkPoints = perks.perkPoints;
            playerData.perksData.perkHealth = perks.perkHealth;
            playerData.perksData.perkMana = perks.perkMana;
            playerData.perksData.perkAttack = perks.perkAttack;
            playerData.perksData.perkMagic = perks.perkMagic;
            playerData.perksData.perkArmor = perks.perkArmor;
            playerData.perksData.perkMagicResistance = perks.perkMagicResistance;
            playerData.comboAttackLevel = comboAttackLevel;
            playerData.castAttackLevel = castAttackLevel;
            playerData.doubleJumpLevel = doubleJumpLevel;
            playerData.inventoryItems = inventoryItems;
            playerData.equipWeapon = weapon.weapon;
            playerData.itemWeaponState = weapon.itemCurrentState;
            playerData.equipHelmet = helmet.helmetItem;
            playerData.itemHelmetState = helmet.itemCurrentState;
            playerData.equipArmor = armor.armorItem;
            playerData.itemArmorState = armor.itemCurrentState;
            playerData.equipBoots = boots.bootsItem;
            playerData.itemBootsState = boots.itemCurrentState;
            playerData.coins = inventoryController.coinsValue;
            playerData.currentQuestIndex = questManager.currentQuestIndex;
            playerData.woods = inventoryController.woods;
            playerData.stones = inventoryController.stones;
            playerData.copper = inventoryController.copper;
            playerData.silver = inventoryController.silver;
            playerData.gold = inventoryController.gold;
            playerData.saphire = inventoryController.saphire;
            playerData.enemiesDefeated = new List<int>();
            foreach (Quest quest in questManager.quests)
            {
                playerData.enemiesDefeated.Add(quest.enemiesDefeated);
            }
            string json = JsonUtility.ToJson(playerData);

            // Sử dụng câu lệnh using để đảm bảo việc giải phóng tài nguyên đúng cách
            using (StreamWriter streamWriter = new StreamWriter(filePath))
            {
                streamWriter.Write(json);
            }

            Debug.Log("Player saved.");
        }
        else
        {
            Debug.LogError("One or more required components are is missing.");
        }
    }

    public void LoadPlayerData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            PlayerData playerData = JsonUtility.FromJson<PlayerData>(json);
            gameObject.transform.position = playerData.position;
            LevelAndExperience levelAndExp = GetComponent<LevelAndExperience>();
            AttackPlayer attackPlayer = GetComponent<AttackPlayer>();
            Perks perks = GetComponent<Perks>();
            PlayerController playerController = GetComponent<PlayerController>();
            Defense defense = GetComponent<Defense>();
            DamageAble damageAble = GetComponent<DamageAble>();
            AgentWeapon weapon = GetComponent<AgentWeapon>();
            AgentArmor armor = GetComponent<AgentArmor>();
            AgentBoots boots = GetComponent<AgentBoots>();
            AgentHelmet helmet = GetComponent<AgentHelmet>();
            QuestManager questManager = FindAnyObjectByType<QuestManager>();
            QuestDialog questDialog = FindAnyObjectByType<QuestDialog>();
            if (levelAndExp != null && attackPlayer != null && perks != null 
                && playerController != null && defense != null && damageAble != null
                && questManager != null && questDialog != null
                && playerData.enemiesDefeated.Count == questManager.quests.Count)
            {
                levelAndExp.level = playerData.level;
                levelAndExp.experience = playerData.experience;
                levelAndExp.experienceToNextLevel = playerData.experienceToNextLevel;
                levelAndExp.skillPoints = playerData.skillPoints;
                playerController.canCastAttack = playerData.canCastAttack;
                playerController.canCastComboAttack = playerData.canCastComboAttack;
                playerController.canDoubleJump = playerData.canDoubleJump;
                attackPlayer.baseAttackDamage = playerData.baseAttackDamage;
                attackPlayer.baseMagicDamage = playerData.baseMagicDamage;
                attackPlayer.totalAttackDamage = playerData.totalAttackDamage;
                attackPlayer.totalMagicDamage = playerData.totalMagicDamage;
                defense.baseArmor = playerData.baseArmor;
                defense.armorFromPerk = playerData.armorFromPerk;
                defense.baseMagicResistance = playerData.baseMagicResistance;
                defense.magicResistanceFromPerk = playerData.magicResistanceFromPerk;
                damageAble.Health = playerData.Health;
                damageAble._maxHealth = playerData.MaxHealth;
                damageAble.Mana = playerData.Mana;
                damageAble._maxMana = playerData.MaxMana;
                perks.perkPoints = playerData.perksData.perkPoints;
                perks.perkHealth = playerData.perksData.perkHealth;
                perks.perkMana = playerData.perksData.perkMana;
                perks.perkAttack = playerData.perksData.perkAttack;
                perks.perkMagic = playerData.perksData.perkMagic;
                perks.perkArmor = playerData.perksData.perkArmor;
                perks.perkMagicResistance = playerData.perksData.perkMagicResistance;
                comboAttackLevel = playerData.comboAttackLevel;
                castAttackLevel = playerData.castAttackLevel;
                doubleJumpLevel = playerData.doubleJumpLevel;
                inventoryController.coinsValue = playerData.coins;
                inventoryController.UpdateCoinText();
                inventoryController.woods = playerData.woods;
                inventoryController.stones = playerData.stones;
                inventoryController.copper = playerData.copper;
                inventoryController.silver = playerData.silver;
                inventoryController.gold = playerData.gold;
                inventoryController.saphire = playerData.saphire;
                questManager.currentQuestIndex = playerData.currentQuestIndex;
                if (questManager.currentQuestIndex < questManager.quests.Count)
                {
                    questDialog.SetQuest(questManager.quests[questManager.currentQuestIndex]);
                }
                for (int i = 0; i < playerData.enemiesDefeated.Count; i++)
                {
                    questManager.quests[i].enemiesDefeated = playerData.enemiesDefeated[i];
                }
                // Xóa tất cả các mục hiện tại trong InventorySO
                if (inventoryController.inventoryData != null && inventoryController != null)
                {
                    inventoryController.inventoryData.SetInventorySize(0);
                    inventoryController.inventoryData.SetInventorySize(60);
                    inventoryController.inventoryData.Initialize();

                    // Thêm các EquipableItemSO từ playerData.inventoryItems vào InventorySO
                    foreach (InventoryItem item in playerData.inventoryItems)
                    {
                        inventoryController.inventoryData.AddItem(item);
                    }
                }
                else
                {
                    Debug.Log("No inventoryController and inventoryData");
                }
                //weapon.SetWeapon(playerData.equipWeapon, playerData.itemWeaponState);
                //helmet.SetHelmet(playerData.equipHelmet, playerData.itemHelmetState);
                //armor.SetArmor(playerData.equipArmor, playerData.itemArmorState);
                //boots.SetBoots(playerData.equipBoots, playerData.itemBootsState);
                if (playerData.equipWeapon != null)
                {
                    weapon.SetWeapon(playerData.equipWeapon, playerData.itemWeaponState);
                }
                if (playerData.equipHelmet != null)
                {
                    helmet.SetHelmet(playerData.equipHelmet, playerData.itemHelmetState);
                }
                if (playerData.equipArmor != null)
                {
                    armor.SetArmor(playerData.equipArmor, playerData.itemArmorState);
                }
                if (playerData.equipBoots != null)
                {
                    boots.SetBoots(playerData.equipBoots, playerData.itemBootsState);
                }

                Debug.Log("Player loaded.");
                // Kiểm tra xem mảng skillLearnButtons có phần tử không
                if (skillLearnButtons.Length > 0)
                {
                    // Thiết lập skillLevels của SkillLearnButton đầu tiên
                    skillLearnButtons[0].UpdateSkillLevel(SkillLearnButton.SkillType.CastAttack, castAttackLevel);
                    skillLearnButtons[1].UpdateSkillLevel(SkillLearnButton.SkillType.ComboAttack, comboAttackLevel);
                }
                else
                {
                    Debug.LogError("skillLearnButtons array is empty.");
                }
            }
            else
            {
                Debug.LogError("One or more required components are missing.");
            }
        }
        else
        {
            Debug.Log("Player save file not found.");
        }
    }
}

[System.Serializable]
public class PlayerData
{
    public Vector3 position; // Vị trí của nhân vật
    public int level; // Cấp độ của nhân vật
    public int experience; // Kinh nghiệm của nhân vật
    public int experienceToNextLevel; //kinh nghiệm cho cấp độ tiếp theo của nhân vật
    public int skillPoints; // Số điểm kỹ năng
    public PerksData perksData; // Thông tin về Perks
    public int totalAttackDamage;
    public int totalMagicDamage;
    public int baseAttackDamage;
    public int baseMagicDamage;
    public int baseArmor;
    public int baseMagicResistance;
    public int armorFromPerk;
    public int magicResistanceFromPerk;
    public int Health;
    public int MaxHealth;
    public float Mana;
    public float MaxMana;
    public bool canCastAttack; // Trạng thái của kỹ năng Cast Attack
    public bool canCastComboAttack; // Trạng thái của kỹ năng Combo Attack
    public bool canDoubleJump; // Trạng thái của kỹ năng Double Jump
    public int comboAttackLevel; // Cấp độ của kỹ năng ComboAttack
    public int castAttackLevel;
    public int doubleJumpLevel;
    // Thêm thuộc tính để lưu trữ danh sách các mục trong inventory
    public List<InventoryItem> inventoryItems;
    public EquipableItemSO equipWeapon; // Add this field to store the EquipableItemSO
    public EquipableItemSO equipHelmet;
    public EquipableItemSO equipArmor;
    public EquipableItemSO equipBoots;
    public List<ItemParameter> itemWeaponState;
    public List<ItemParameter> itemHelmetState;
    public List<ItemParameter> itemArmorState;
    public List<ItemParameter> itemBootsState;
    public int coins;
    public int currentQuestIndex;
    public List<int> enemiesDefeated;
    public int woods;
    public int stones;
    public int copper;
    public int silver;
    public int gold;
    public int saphire;
    // Hàm tạo mặc định
    public PlayerData()
    {
        // Khởi tạo các trường với giá trị mặc định
        canCastAttack = false;
        canCastComboAttack = false;
        canDoubleJump = false;
        inventoryItems = new List<InventoryItem>();
        enemiesDefeated = new List<int>();
    }
}
[System.Serializable]
public class PerksData
{
    public int perkPoints;
    public int perkHealth;
    public int perkMana;
    public int perkAttack;
    public int perkMagic;
    public int perkArmor;
    public int perkMagicResistance;
}
