using Fusion;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillLearnButton : MonoBehaviour , IPointerClickHandler
{
    public delegate void SkillLearned(SkillType skillType);
    public static event SkillLearned OnSkillLearned;
    public Image skillImage;
    public UISkillsDescription skillDescription;
    public string skillName;
    public AttackPlayer attackPlayer; // Thêm tham chiếu đến AttackPlayer
    public Cast cast; // Thêm tham chiếu đến Cast
    public Attack attack;
    public LevelAndExperience playerLevel;
    public PlayerDataSaver playerDataSaver;
    public Dictionary<SkillType, int> skillLevels = new Dictionary<SkillType, int>(); // Lưu trữ cấp độ của từng kỹ năng
    public NetworkObject networkObject;
    public bool IsOwner => networkObject.HasInputAuthority;
    public enum SkillType
    {
        CastAttack,
        ComboAttack,
        DoubleJump
    }
    private bool hasLearnedDoubleJump = false; // Biến để kiểm tra xem DoubleJump đã được học hay chưa
    public SkillType skillToLearn;
    // Xử lý khi click chuột trái
    public void CheckSkillLeftClick()
    {
        Debug.Log("Skill left-clicked");
        // Gọi phương thức xử lý khi click chuột trái
        skillDescription.SetSkillImage(skillImage.sprite);
        // Cập nhật mô tả kỹ năng
        skillDescription.SetSkillDescription(skillName);
        // Kích hoạt hình ảnh trong skillDescription
        skillDescription.ToggleSkillImage(true);
        //Debug.Log("Left");

        //if (skillToLearn == SkillType.CastAttack)
        //{
        //    // Tính toán tổng sát thương
        //    int totalCastDamage = cast.damage + attackPlayer.TotalMagicDamage;
        //    // Cập nhật mô tả kỹ năng
        //    skillDescription.UpdateSkillCastDescription(totalCastDamage);
        //}
        if (skillToLearn == SkillType.CastAttack && IsOwner)
        {
            int totalCastDamage = CalculateTotalCastDamage();
            int nextLevelCastDamage = (int)(totalCastDamage * 1.2f);
            string requiredLevelPlayer;
            if (skillLevels[SkillType.CastAttack] < 6)
            {
                requiredLevelPlayer = (skillLevels[SkillType.CastAttack] * 10).ToString();
            }
            else
            {
                requiredLevelPlayer = "Tối đa";
            }
            skillDescription.UpdateSkillCastDescription(totalCastDamage, nextLevelCastDamage, requiredLevelPlayer); ;
        }
        if (skillToLearn == SkillType.ComboAttack && IsOwner)
        {
            int totalComboDamage = CalculateTotalComboDamage();
            int nextLevelComboDamage = (int)(totalComboDamage * 1.2f);
            string requiredLevelPlayer;
            if (skillLevels[SkillType.ComboAttack] < 6)
            {
                requiredLevelPlayer = (skillLevels[SkillType.ComboAttack] * 20).ToString();
            }
            else
            {
                requiredLevelPlayer = "Tối đa";
            }
            skillDescription.UpdateSkillComboDescription(totalComboDamage, nextLevelComboDamage, requiredLevelPlayer);
        }
        if (skillToLearn == SkillType.DoubleJump && IsOwner)
        {
            string requiredLevelPlayer;
            if (skillLevels[SkillType.DoubleJump] < 2)
            {
                requiredLevelPlayer = (skillLevels[SkillType.DoubleJump] * 5).ToString();
            }
            else
            {
                requiredLevelPlayer = "Tối đa";
            }
            skillDescription.UpdateSkillDoubleJumpDescription(requiredLevelPlayer);
        }
        UpdateSkillDescription();
        
    }

    // Xử lý khi click chuột phải
    public void LearnSkillRightClick()
    {

        Debug.Log("Skill right-clicked");
        if (CanLearnSkill() == true && IsOwner)
        {
            // Tính toán cấp độ yêu cầu mới dựa trên cấp độ hiện tại của người chơi và cấp độ hiện tại của kỹ năng
            int requiredLevel = (skillLevels[skillToLearn] * 5);

            if (playerLevel.level >= (requiredLevel * 2) && skillToLearn == SkillType.CastAttack)
            {
                skillLevels[skillToLearn]++; // Tăng cấp độ của kỹ năng
                LearnSkill();
                UpdateSkillDescription();
                playerDataSaver.castAttackLevel = skillLevels[skillToLearn];
            }
            else if (playerLevel.level >= (requiredLevel * 4) && skillToLearn == SkillType.ComboAttack)
            {
                skillLevels[skillToLearn]++; // Tăng cấp độ của kỹ năng
                LearnSkill();
                UpdateSkillDescription();
                // Cập nhật cấp độ ComboAttack trong PlayerDataSaver
                playerDataSaver.comboAttackLevel = skillLevels[skillToLearn];
            }
            else if (playerLevel.level >= requiredLevel && skillToLearn == SkillType.DoubleJump)
            {
                skillLevels[skillToLearn]++; // Tăng cấp độ của kỹ năng
                LearnSkill();
                UpdateSkillDescription();
            }
            else
            {
                Debug.Log("Player level is not high enough to learn this skill.");
            }
        }
        
    }
    // Kiểm tra xem có thể học kỹ năng này không
    bool CanLearnSkill()
    {
        
        if (skillLevels.ContainsKey(skillToLearn))
        {
            if (skillToLearn == SkillType.DoubleJump)
            {
                if (!hasLearnedDoubleJump && playerLevel.level >= 5)
                {
                    Debug.Log("Can learn Double Jump");  // Kiểm tra điều kiện DoubleJump
                    return true;
                }
            }
            else if (skillToLearn == SkillType.CastAttack)
            {
                if (playerLevel.level >= 10 && skillLevels[SkillType.CastAttack] < 6)
                {
                    Debug.Log("Can learn Cast Attack");  // Kiểm tra điều kiện CastAttack
                    return true;
                }
            }
            else if (skillToLearn == SkillType.ComboAttack)
            {
                if (playerLevel.level >= 20 && skillLevels[SkillType.ComboAttack] < 6)
                {
                    Debug.Log("Can learn Combo Attack");  // Kiểm tra điều kiện ComboAttack
                    return true;
                }
            }
            else
            {
                Debug.Log("Can learn other skill");
                return skillLevels[skillToLearn] < 6;
            }
        }
        

        Debug.Log("Cannot learn skill. Check conditions.");
        return false;
    }

    // Tính toán tổng sát thương của CastAttack dựa trên cấp độ kỹ năng
    public int CalculateTotalCastDamage()
    {
        float totalCastDamage = cast.damage + attackPlayer.TotalMagicDamage;
        for (int i = 0; i <= skillLevels[SkillType.CastAttack]; i++)
        {
            totalCastDamage *= 1.2f; // Mỗi cấp độ tăng 20% sát thương
        }
        return (int)totalCastDamage;
    }
    public int CalculateTotalComboDamage()
    {
        float totalComboDamage = attack.attackDamage + attackPlayer.TotalAttackDamage;
        for (int i = 0; i <= skillLevels[SkillType.ComboAttack]; i++)
        {
            totalComboDamage *= 1.2f; // Mỗi cấp độ tăng 20% sát thương
        }
        return (int)totalComboDamage;
    }
    public void LearnSkill()
    {
        if (IsOwner)
        {
            if (playerLevel.skillPoints > 0) // Kiểm tra xem người chơi có điểm kỹ năng không
            {
                Debug.Log("Skill points before: " + playerLevel.skillPoints);

                if (skillToLearn == SkillType.DoubleJump)
                {
                    hasLearnedDoubleJump = true; // Đánh dấu rằng DoubleJump đã được học
                }
                OnSkillLearned?.Invoke(skillToLearn);
                Debug.Log("Skill learned!");
                UpdateSkillDescription(); // Cập nhật màu sau khi học kỹ năng
                playerLevel.DecreaseSkillPoints(1); // Giảm một điểm kỹ năng khi học kỹ năng mới

                Debug.Log("Skill points after: " + playerLevel.skillPoints);
            }
            else
            {
                Debug.Log("Not enough skill points to learn this skill.");
            }
        }
    }

    public void UpdateSkillLevel(SkillType skillType, int level)
    {
        if (skillLevels.ContainsKey(skillType))
        {
            skillLevels[skillType] = level;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsOwner)
        {

            if (eventData.button == PointerEventData.InputButton.Left)
            {
                CheckSkillLeftClick();
            }
            else if (eventData.button == PointerEventData.InputButton.Right)
            {
                LearnSkillRightClick();
            }
        }
    }
    void Start()
    {
        // Tìm player trong scene dựa trên tag "Player"
        GameObject playerObject = GameObject.FindWithTag("Playerr");
        
        // Kiểm tra xem playerObject có tồn tại không
        if (playerObject != null && IsOwner)
        {
            // Lấy tham chiếu đến component LevelAndExperience từ playerObject
            playerLevel = playerObject.GetComponent<LevelAndExperience>();
            playerDataSaver = playerObject.GetComponent<PlayerDataSaver>();
            // Kiểm tra xem playerLevel có tồn tại không
            if (playerLevel == null)
            {
                Debug.LogError("Player object does not have LevelAndExperience component.");
            }
            else
            {
                InitializeSkillLevels(); // Khởi tạo cấp độ cho từng loại kỹ năng
                UpdateSkillDescription();
            }
        }
        else
        {
            Debug.LogError("Player object not found in scene.");
        }
        
        InitializeSkillLevels(); // Khởi tạo cấp độ cho từng loại kỹ năng
        UpdateSkillDescription();
    }
    void Update()
    {
        networkObject = GameObject.FindGameObjectWithTag("Playerr").GetComponent<NetworkObject>();
    }
    // Khởi tạo cấp độ cho từng loại kỹ năng
    void InitializeSkillLevels()
    {
        skillLevels[SkillType.CastAttack] = 1;  // Hoặc một giá trị mặc định
        skillLevels[SkillType.ComboAttack] = 1; // Cấp độ mặc định là 0
        skillLevels[SkillType.DoubleJump] = 1;  // Cấp độ mặc định là 0

        if (playerDataSaver != null)
        {
            // Load dữ liệu của người chơi từ tệp JSON
            playerDataSaver.LoadPlayerData();

            // Khởi tạo cấp độ cho từng loại kỹ năng trước
            skillLevels[SkillType.CastAttack] = playerDataSaver.castAttackLevel;
            skillLevels[SkillType.ComboAttack] = playerDataSaver.comboAttackLevel;
            skillLevels[SkillType.DoubleJump] = playerDataSaver.doubleJumpLevel;
        }
        else
        {
            Debug.LogError("PlayerDataSaver is not assigned.");
        }

    }
    void UpdateSkillDescription()
    {
        bool isSkillLevelRequirementMet = false;
        bool isMaxSkillLevel = false;
        if (IsOwner)
        {
            // Ensure playerLevel is not null before accessing its properties
            if (playerLevel != null)
            {
                // Kiểm tra xem cấp độ người chơi có đủ yêu cầu cho kỹ năng không
                if (playerLevel.level >= (skillLevels[SkillType.CastAttack] * 10) && skillToLearn == SkillType.CastAttack)
                {
                    isSkillLevelRequirementMet = true;
                }
                else if (playerLevel.level >= (skillLevels[SkillType.ComboAttack] * 20) && skillToLearn == SkillType.ComboAttack)
                {
                    isSkillLevelRequirementMet = true;
                }
                else if (playerLevel.level >= 5 && skillToLearn == SkillType.DoubleJump)
                {
                    isSkillLevelRequirementMet = true;
                }
            }
            else
            {
                Debug.LogError("PlayerLevel is not assigned.");
                return; // Exit the method early to prevent further errors
            }

            // Ensure skillLevels contains the required keys before accessing them
            if (skillLevels.ContainsKey(skillToLearn))
            {
                if (skillLevels[skillToLearn] == 6 && skillToLearn != SkillType.DoubleJump) // Kiểm tra nếu không phải là DoubleJump và đạt cấp độ tối đa là 6
                {
                    isMaxSkillLevel = true;
                }
                else if (skillLevels[skillToLearn] == 2 && skillToLearn == SkillType.DoubleJump) // Kiểm tra nếu là DoubleJump và đạt cấp độ tối đa là 2
                {
                    isMaxSkillLevel = true;
                }
            }
            else
            {
                Debug.LogError("SkillToLearn key not found in skillLevels.");
                return; // Exit the method early to prevent further errors
            }

            // Now update the skill description based on the conditions
            if (isMaxSkillLevel)
            {
                skillDescription.skillName.color = Color.blue; // Màu xanh nếu đạt tới cấp độ tối đa của kỹ năng
            }
            // Cập nhật màu cho mô tả kỹ năng
            else if (isSkillLevelRequirementMet)
            {
                skillDescription.skillName.color = Color.green; // Màu xanh nếu đạt yêu cầu
            }
            else
            {
                skillDescription.skillName.color = Color.red; // Màu đỏ nếu không đạt yêu cầu
            }
        }
        
    }
}