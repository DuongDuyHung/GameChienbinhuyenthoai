using UnityEngine;
using UnityEngine.Events;

public class CharacterEvents
{
    public static UnityAction<GameObject, int> characterDamaged;
    public static UnityAction<GameObject, int> characterHealed;
    public static UnityAction<GameObject, int> characterManaRecovery;
    public static UnityAction<GameObject, int> characterMagicDamaged; // Thêm sự kiện MagicDamage
}
