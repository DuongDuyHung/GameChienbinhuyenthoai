using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelAndExperience : MonoBehaviour
{
    public int level = 1; // Cấp độ mặc định là 1
    public int experience = 0; // Kinh nghiệm mặc định là 0
    public int skillPoints = 0; // Số điểm kỹ năng mặc định là 0
    public int experienceToNextLevel = 100; // Kinh nghiệm cần để lên cấp tiếp theo
    private Perks perks; // Tham chiếu đến Perks
    private Image content;
    [SerializeField]
    private Image experienceBar;
    [SerializeField]
    private TMP_Text skillPointsText; // Để hiển thị số điểm kỹ năng
    // Hàm này để cập nhật kinh nghiệm và kiểm tra xem người chơi có lên cấp không
    public void AddExperience(int amount)
    {
        experience += amount;

        // Kiểm tra xem người chơi có đủ kinh nghiệm để lên cấp không
        while (experience >= experienceToNextLevel)
        {
            LevelUp();
        }
    }
    public void RemoveExperience(int amount)
    {
        experience -= amount;
        if (experience < 0)
        {
            experience = 0;
        }
    }

    // Hàm này để tăng cấp độ và cập nhật kinh nghiệm cần để lên cấp tiếp theo
    private void LevelUp()
    {
        level++;
        experience -= experienceToNextLevel; // Giảm kinh nghiệm hiện tại bằng số kinh nghiệm cần để lên cấp

        // Tăng kinh nghiệm cần để lên cấp tiếp theo (có thể là một công thức khác)
        experienceToNextLevel = (int)(experienceToNextLevel * 1.3f); // Ví dụ: Tăng 10% cho mỗi cấp độ
         // Kiểm tra nếu cấp độ hiện tại chia hết cho 10, tăng điểm kỹ năng
        if (level % 10 == 0)
        {
            skillPoints += 1;
        }
        // Cập nhật điểm tiềm năng
        perks.UpdatePerkPoints(5);
    }

    // Start is called before the first frame update
    void Start()
    {
        perks = GetComponent<Perks>(); // Lấy tham chiếu đến Perks từ cùng một GameObject
        content = GetComponent<Image>();
        experienceBar = GameObject.FindWithTag("ExperienceBar").GetComponent<Image>();
        skillPointsText = GameObject.FindGameObjectWithTag("SkillPoints").GetComponent<TMP_Text>();
        UpdateSkillPointsText(); // Cập nhật hiển thị số điểm kỹ năng khi bắt đầu
    }
    public void DecreaseSkillPoints(int amount)
    {
        skillPoints -= amount;
        UpdateSkillPointsText(); // Cập nhật số điểm kỹ năng trên giao diện người dùng
    }

    // Update is called once per frame
    void Update()
    {
        UpdateExperienceBar();
        UpdateSkillPointsText(); // Cập nhật hiển thị số điểm kỹ năng khi bắt đầu
    }
    private void UpdateExperienceBar()
    {
        float fillAmount = (float)experience / (float)experienceToNextLevel;
        experienceBar.fillAmount = fillAmount;
    }
    private void UpdateSkillPointsText()
    {
        skillPointsText.text = "Điểm K.Năng: " + skillPoints.ToString();
    }
}
