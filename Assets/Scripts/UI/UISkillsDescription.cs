using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISkillsDescription : MonoBehaviour
{
    [SerializeField]
    public Image skillImage;
    [SerializeField]
    public TMP_Text skillDescription;
    [SerializeField]
    public TMP_Text skillName;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void SetSkillDescription(string description)
    {
        skillName.text = description;
    }
    public void UpdateSkillCastDescription(int totalCastDamage,int nextLevelCastDamage,string requiredLevelPlayer)
    {
        // Hiển thị mô tả của CastAttack bao gồm tổng sát thương
        skillDescription.text = "Người chơi sẽ thực hiện tạo ra 1 quả cầu lửa gây " +
            "<color=#FF00FF>" + totalCastDamage + "➜" + nextLevelCastDamage + "</color> phép lên kẻ địch"+
            "\nCấp độ yêu cầu: " + requiredLevelPlayer;
    }
    public void UpdateSkillComboDescription(int totalComboDamage, int nextLevelComboDamage, string requiredLevelPlayer)
    {
        // Hiển thị mô tả của CastAttack bao gồm tổng sát thương
        skillDescription.text = "Người chơi sẽ thực hiện tung ra Liêm hoàn" +
            " những đòn đánh gây <color=#FF0000>" + totalComboDamage + "➜" + nextLevelComboDamage + "</color> vật lý lên kẻ địch"
            + "\nCấp độ yêu cầu: " + requiredLevelPlayer;
    }
    public void UpdateSkillDoubleJumpDescription(string requiredPlayerLevel)
    {
        skillDescription.text = "Người chơi có thể thực hiện nhảy được 2 lần"+"\nCấp độ yêu cầu: " + requiredPlayerLevel;
    }
    public void SetSkillImage(Sprite sprite)
    {
        skillImage.sprite = sprite;
    }
    public void ToggleSkillImage(bool isActive)
    {
        skillImage.gameObject.SetActive(isActive);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
