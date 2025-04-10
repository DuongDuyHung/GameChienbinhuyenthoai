using UnityEngine;

public class Perks : MonoBehaviour
{
    public int perkPoints = 0; // Điểm tiềm năng mặc định là 0
    public int perkHealth = 0;
    public int perkMana = 0;
    public int perkAttack = 0;
    public int perkMagic = 0;
    public int perkArmor = 0;
    public int perkMagicResistance = 0;
    // Hàm này được gọi khi cập nhật điểm tiềm năng mỗi khi lên cấp độ
    public void UpdatePerkPoints(int points)
    {
        perkPoints += points;
    }

    // Cập nhật giá trị perkHealth khi người dùng nhập giá trị mới
    public void UpdatePerkHealth(int healthPoints)
    {
        perkHealth = healthPoints;
    }

    // Cập nhật giá trị perkMana khi người dùng nhập giá trị mới
    public void UpdatePerkMana(int manaPoints)
    {
        perkMana = manaPoints;
    }
    // Cập nhật giá trị perkAttack khi người dùng nhập giá trị mới
    public void UpdatePerkAttack(int attackPoints)
    {
        perkAttack = attackPoints;
    }
    // Cập nhật giá trị perkMagic khi người dùng nhập giá trị mới
    public void UpdatePerkMagic(int magicPoints)
    {
        perkMagic = magicPoints;
    }
    // Cập nhật giá trị perkArmor khi người dùng nhập giá trị mới
    public void UpdatePerkArmor(int armorPoints)
    {
        perkArmor = armorPoints;
    }
    // Cập nhật giá trị perkMagicResistance khi người dùng nhập giá trị mới
    public void UpdatePerkMagicResistance(int magicResistancePoints)
    {
        perkMagicResistance = magicResistancePoints;
    }
}
