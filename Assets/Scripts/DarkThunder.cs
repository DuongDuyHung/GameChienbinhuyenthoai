using UnityEngine;

public class DarkThunder : MonoBehaviour
{
    [SerializeField] private int damageAmount = 20; // Số sát thương gây ra
    [SerializeField] private float knockbackForce = 5f; // Lực đẩy lùi
    [SerializeField] private Vector2 knockbackDirection = Vector2.zero; // Hướng đẩy lùi

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Kiểm tra xem đối tượng va chạm có chứa lớp DamageAble hay không
        DamageAble damageAble = collision.GetComponent<DamageAble>();

        if (damageAble != null && damageAble.IsAlive)
        {
            // Gây sát thương cho đối tượng bằng DamageAble
            Vector2 knockback = knockbackDirection * knockbackForce;
            damageAble.Hit(damageAmount, knockback);

            Debug.Log($"Gây {damageAmount} sát thương lên {collision.gameObject.name}");
        }
    }
}
