using Fusion;
using Inventory;
using Inventory.Model;
using Inventory.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
public class DamageAble : NetworkBehaviour
{
    [SerializeField]
    public GameObject diedMenu;
    [SerializeField]
    public LevelAndExperience playerLevel;
    public UnityEvent<int, Vector2> damageAbleHit;
    Animator animator;
    Attack attack;
    public int minDrop, maxDrop;
    public GameObject[] itemsToDrop;
    private Image content;
    [SerializeField]
    private Image healthBar;
    [SerializeField]
    private Image manaBar;
    // Thêm một biến public cho Slider
    public Slider enemyHealthSlider;
    [SerializeField]
    public int _maxHealth = 100;
    public int setHealthModifier = 0; // Giá trị từ SetArmor
    public int helmetHealthModifier = 0; // Giá trị từ SetHelmet
    public int bootsHealthModifier = 0; // Giá trị từ SetBoots

    public int setManaModifier = 0; // Giá trị từ SetArmor
    public int helmetManaModifier = 0; // Giá trị từ SetHelmet
    public int bootsManaModifier = 0; // Giá trị từ SetBoots
    [SerializeField]
    private int totalHealthModifier => setHealthModifier + helmetHealthModifier + bootsHealthModifier; // Tổng của ba giá trị
    [SerializeField]
    private int totalManaModifier => setManaModifier + helmetManaModifier + bootsManaModifier; // Tổng của ba giá trị
    [SerializeField]
    private int TotalHealthModifier;
    public AgentArmor agentArmor; // Add this line at the top of the class
    public AgentBoots agentBoots;
    public AgentHelmet agentHelmet;
    [SerializeField]
    private int TotalManaModifier;
    [SerializeField]
    public int healthFromPerkPoints = 0;
    [SerializeField]
    public int manaFromPerkPoints = 0;
    public int MaxHealth
    {
        get
        {
            return _maxHealth + totalHealthModifier + healthFromPerkPoints;
        }
        set
        {
            _maxHealth = value;
        }
    }
    [SerializeField]
    private float _mana = 100;
    [SerializeField]
    private float _manaRecoveryRate = 5f;
    [SerializeField]
    public float _maxMana = 100;
    public float Mana
    {
        get
        {
            return _mana;
        }
        set
        {
            _mana = Mathf.Min(value, _maxMana);
        }
    }
    public float MaxMana
    {
        get
        {
            return _maxMana + totalManaModifier + manaFromPerkPoints; // Cập nhật theo lượng điểm từ perkMana
        }
        set
        {
            _maxMana = value;
        }
    }
    [SerializeField]
    private int _health = 100;
    public bool IsAlive
    {
        get
        {
            return _isAlive;
        }
        set
        {
            _isAlive = value;
            animator.SetBool(AnimationStrings.isAlive, value);
            Debug.Log("IsAlive set" + value);
        }
    }

    [SerializeField]
    private bool _isAlive = true;

    [SerializeField]
    private bool isInvincible = false;


    public bool lockVelocity
    {
        get
        {
            return animator.GetBool(AnimationStrings.lockVelocity);
        }
        set
        {
            animator.SetBool(AnimationStrings.lockVelocity, value);
        }
    }




    private float timeSinceHit = 0;

    public float invincibilityTime = 0.25f;

    public int Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = value;
            if (_health <= 0)
            {
                DropItems();

                //Kiểm tra xem đối tượng là người chơi hay không để xác định cách xử lý kinh nghiệm
                if (gameObject.CompareTag("Playerr"))
                {
                    int experienceLost = CalculateExperienceLost(); // Tính toán kinh nghiệm bị mất khi nhân vật mất sức khỏe
                    playerLevel.RemoveExperience(experienceLost); // Giảm kinh nghiệm của người chơi
                }
                else
                {
                    int experienceGained = CalculateExperienceGained(); // Tính toán kinh nghiệm nhận được khi nhân vật mất sức khỏe
                    playerLevel.AddExperience(experienceGained); // Tăng kinh nghiệm của nhân vật
                    QuestManager questManager = FindObjectOfType<QuestManager>();
                    if (questManager != null)
                    {
                        // Assuming you have a reference to the active quest
                        Quest currentQuest = questManager.quests.Find(q => q.enemiesDefeated < q.enemiesToDefeat);
                        if (currentQuest != null)
                        {
                            questManager.DefeatEnemy(currentQuest);
                        }
                    }
                }

                IsAlive = false;
                if (gameObject.CompareTag("Playerr"))
                {
                    diedMenu.SetActive(true);
                }
            }
        }
    }
    private int CalculateExperienceGained()
    {
        int baseExperience = 50; // Kinh nghiệm cơ bản nhận được khi nhân vật chết
        int experiencePerLevel = 200000; // Kinh nghiệm nhận được cho mỗi cấp độ của quái vật

        int totalExperienceGained = baseExperience + (experiencePerLevel * GetComponent<EnemyLevel>().GetLevel());

        return totalExperienceGained;
    }

    private int CalculateExperienceLost()
    {
        // Bạn có thể thêm logic tính toán dựa trên mức độ sức khỏe mất và loại của nhân vật
        // Ví dụ: mất một phần trăm kinh nghiệm dựa trên mức độ sức khỏe mất

        int baseExperience = 50; // Kinh nghiệm cơ bản bị mất khi nhân vật chết
        int experiencePerHealthLost = 2; // Kinh nghiệm mất cho mỗi điểm sức khỏe mất

        int totalExperienceLost = baseExperience + ((_maxHealth - _health) * experiencePerHealthLost);

        return totalExperienceLost;
    }

    public void DropItems()
    {
        if (itemsToDrop.Length == 0)
        {
            return; // Không có vật phẩm trong danh sách, thoát phương thức
        }

        int dropCount = Random.Range(minDrop, maxDrop); // Số lượng vật phẩm sẽ được drop, từ 1 đến 2

        for (int i = 0; i < dropCount; i++)
        {
            int randomIndex = Random.Range(0, itemsToDrop.Length); // Chọn một chỉ số ngẫu nhiên

            GameObject item = itemsToDrop[randomIndex]; // Lấy món đồ tại chỉ số ngẫu nhiên

            if (item != null)
            {
                Vector3 randomOffset = new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 1f), 0);
                Vector3 dropPosition = transform.position + randomOffset;
                GameObject droppedItem = Instantiate(item, dropPosition, Quaternion.identity);
            }

        }
    }
    private void Update()
    {
        RpcUpdateHealth(_health);
        RpcUpdateMana(_mana);
        if (isInvincible)
        {
            if (timeSinceHit > invincibilityTime)
            {
                isInvincible = false;
                timeSinceHit = 0;
            }
            timeSinceHit += Time.deltaTime;
        }
        if (_mana < MaxMana)
        {
            _mana += (float)(_manaRecoveryRate * Time.deltaTime);
        }
        if (_mana > MaxMana)
        {
            _mana = MaxMana; // Đặt lại giá trị của Mana thành MaxMana nếu nó vượt quá
        }

    }
    private NetworkObject networkObject;
    public bool IsOwner => networkObject.HasInputAuthority;
    private void Awake()
    {
        diedMenu = GameObject.FindGameObjectWithTag("DiedMenu");
        animator = GetComponent<Animator>();
        networkObject = GetComponent<NetworkObject>();
        if (gameObject.CompareTag("Playerr"))
        {
            agentHelmet = GetComponent<AgentHelmet>();
            agentBoots = GetComponent<AgentBoots>();
            agentArmor = GetComponent<AgentArmor>();
            content = GetComponent<Image>();
            healthBar = GameObject.FindWithTag("HealthBar").GetComponent<Image>();
            manaBar = GameObject.FindWithTag("ManaBar").GetComponent<Image>();
            UpdateCurrentValues();
        }
    }

    // Hàm Hit với RPC
    public bool Hit(int damage, Vector2 knockback)
    {
        if (HasStateAuthority) // Chỉ máy chủ xử lý sát thương
        {
            RpcApplyDamage(damage, knockback, isMagic: false);
            return true;
        }
        return false;
    }

    // Hàm MagicHit với RPC
    public bool MagicHit(int magicDamage, Vector2 knockback)
    {
        if (HasStateAuthority) // Chỉ máy chủ xử lý sát thương
        {
            RpcApplyDamage(magicDamage, knockback, isMagic: true);
            return true;
        }
        return false;
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcUpdateHealth(int newHealth)
    {
        _health = newHealth; // Đồng bộ giá trị máu

        // Chỉ cập nhật thanh máu nếu đây là player của client
        if (gameObject.CompareTag("Playerr") && IsOwner)
        {
            UpdateHealthBar(); // Cập nhật UI cho chính mình
        }
        else if (gameObject.CompareTag("Enemy"))
        {
            UpdateHealthBar(); // Nếu là kẻ địch, luôn cập nhật thanh máu
        }
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcUpdateMana(float newMana)
    {
        _mana = newMana;
        // Chỉ cập nhật thanh máu nếu đây là player của client
        if (gameObject.CompareTag("Playerr") && IsOwner)
        {
            UpdateManaBar(); // Cập nhật UI cho chính mình
        }
    }

    // RPC áp dụng sát thương trên tất cả máy
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcApplyDamage(int damage, Vector2 knockback, bool isMagic)
    {
        if (IsAlive && !isInvincible) // Kiểm tra trạng thái chỉ trên máy khách
        {
            // Trừ máu
            Health -= damage;
            // Giảm độ bền trang bị nếu là Player
            if (gameObject.CompareTag("Playerr"))
            {
                if (agentArmor != null) agentArmor.DecreaseDurability(1);
                if (agentHelmet != null) agentHelmet.DecreaseDurability(1);
                if (agentBoots != null) agentBoots.DecreaseDurability(1);
            }

            // Trạng thái không thể bị đánh tiếp
            isInvincible = true;
            lockVelocity = true;

            // Kích hoạt animation
            animator.SetTrigger(AnimationStrings.hitTrigger);
            RpcUpdateHealth(_health);

            // Gọi sự kiện tương ứng
            if (isMagic)
            {
                CharacterEvents.characterMagicDamaged?.Invoke(gameObject, damage);
            }
            else
            {
                CharacterEvents.characterDamaged?.Invoke(gameObject, damage);
            }

            // Hiển thị thanh máu nếu cần
            if (enemyHealthSlider != null)
            {
                enemyHealthSlider.gameObject.SetActive(true);
            }
        }
    }


    public bool CastSpell(int manaCost)//, Vector2 knockback)
    {
        if (IsAlive && Mana >= manaCost)
        {
            Mana -= manaCost;
            return true;
        }
        return false;
    }

    public bool Heal(int healthRestore)
    {
        if (IsAlive && Health < MaxHealth)
        {
            int maxHeal = Mathf.Max(MaxHealth - Health, 0);
            int actualHeal = Mathf.Min(maxHeal, healthRestore);
            Health += actualHeal;
            CharacterEvents.characterHealed(gameObject, actualHeal);
            return true;
        }
        return false;
    }
    public bool ManaRecovery(int manaRecovery)
    {
        if (IsAlive && Mana < MaxMana)
        {
            int maxMana = Mathf.Max((int)(MaxMana - Mana), 0);
            int actualMana = Mathf.Min(maxMana, manaRecovery);
            Mana += actualMana;
            CharacterEvents.characterManaRecovery(gameObject, actualMana);
            return true;
        }
        return false;
    }
    private void UpdateHealthBar()
    {
        if (healthBar != null && gameObject.CompareTag("Playerr") && IsOwner)
        {
            healthBar.fillAmount = (float)_health / MaxHealth;
        }
        // Kiểm tra nếu Slider đã được thiết lập
        if (enemyHealthSlider != null && gameObject.CompareTag("Enemy"))
        {
            // Cập nhật giá trị fill amount của Slider dựa trên phần trăm thanh máu còn lại của kẻ địch
            enemyHealthSlider.value = (float)_health / MaxHealth;
        }
    }

    private void UpdateManaBar()
    {
        if (manaBar != null)
        {
            manaBar.fillAmount = _mana / MaxMana;
        }
    }
    private void UpdateCurrentValues()
    {
        //MaxHealth = _maxHealth + totalHealthModifier;
        //MaxMana = _maxMana + totalManaModifier; // Cập nhật MaxMana dựa trên totalManaModifier
        TotalHealthModifier = totalHealthModifier;
        TotalManaModifier = totalManaModifier;
    }
    public void SetHealthManaArmor(EquipableItemSO itemSO, List<ItemParameter> itemState = null)
    {
        if (itemSO == null)
        {
            setHealthModifier = 0;
            setManaModifier = 0;
        }
        else
        {
            setHealthModifier = itemSO.healthModifier;
            setManaModifier = itemSO.manaModifier;
            var currentDurabilityParameter = itemState.Find(param => param.itemParameter is ItemParameterSO);
            float currentDurability = currentDurabilityParameter.itemParameter != null ? currentDurabilityParameter.value : 0f;

            if (currentDurability <= 0)
            {
                setHealthModifier = 0;
                setManaModifier = 0;
            }
        }

        UpdateCurrentValues();
    }
    public void SetHealthManaHelmet(EquipableItemSO itemSO, List<ItemParameter> itemState = null)
    {
        if (itemSO == null)
        {
            helmetHealthModifier = 0;
            helmetManaModifier = 0;
        }
        else
        {
            helmetHealthModifier = itemSO.healthModifier;
            helmetManaModifier = itemSO.manaModifier;
            var currentDurabilityParameter = itemState.Find(param => param.itemParameter is ItemParameterSO);
            float currentDurability = currentDurabilityParameter.itemParameter != null ? currentDurabilityParameter.value : 0f;

            if (currentDurability <= 0 || itemSO == null)
            {
                helmetHealthModifier = 0;
                helmetManaModifier = 0;
            }
        }
        UpdateCurrentValues();
    }

    public void SetHealthManaBoots(EquipableItemSO itemSO, List<ItemParameter> itemState = null)
    {
        if (itemSO == null)
        {
            bootsHealthModifier = 0;
            bootsManaModifier = 0;
        }
        else
        {
            bootsHealthModifier = itemSO.healthModifier;
            bootsManaModifier = itemSO.manaModifier;
            var currentDurabilityParameter = itemState.Find(param => param.itemParameter is ItemParameterSO);
            float currentDurability = currentDurabilityParameter.itemParameter != null ? currentDurabilityParameter.value : 0f;

            if (currentDurability <= 0)
            {
                bootsHealthModifier = 0;
                bootsManaModifier = 0;
            }
        }

        UpdateCurrentValues();
    }
}