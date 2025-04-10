using Fusion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(DamageAble))]
public class PlayerController : NetworkBehaviour
{
    public float walkSpeed = 5f;
    public float airWalkSpeed = 3f;
    public float runSpeed = 9f;
    public float jumpImpulse = 10f;
    public int spellManaCost = 10;
    public Vector2 currentPosition;
    TouchingDirections touchingDirections;
    DamageAble damageAble;
    Vector2 moveInput;
    Rigidbody2D rb;
    public int perkSkillPoints;
    public bool canCastAttack = false;
    public bool canCastComboAttack = false;
    public bool canDoubleJump = false;
    [Networked]public Vector3 SpawnPosition { get; set; } // Thuộc tính để lưu vị trí spawn
    private NetworkObject networkObject;
    public bool IsOwner => networkObject.HasInputAuthority;
    public float CurrentMoveSpeed
    {
        get
        {
            if (CanMove)
            {
                if (IsMoving && !touchingDirections.IsOnWall)
                {
                    if (touchingDirections.IsGrounded)
                    {
                        if (IsRunning)
                        {
                            return runSpeed;
                        }
                        else
                        {
                            return walkSpeed;
                        }

                    }
                    else
                    {
                        return airWalkSpeed;
                    }

                }
                else
                {
                    //dung im
                    return 0;
                }
            }
            else
            {
                return 0;
            }



        }
    }
    [SerializeField]
    private bool _isMoving = false;
    public bool IsMoving
    {
        get
        {
            return _isMoving;
        }
        private set
        {
            _isMoving = value;
            animator.SetBool(AnimationStrings.isMoving, value);
        }
    }
    [SerializeField]
    private bool _isRunning = false;

    public bool IsRunning
    {
        get
        {
            return _isRunning;
        }
        private set
        {
            _isRunning = value;
            animator.SetBool(AnimationStrings.isRunning, value);
        }
    }

    public bool IsFacingRight
    {
        get { return _isFacingRight; }
        private set
        {
            if (_isFacingRight != value)
            {
                transform.localScale *= new Vector2(-1, 1);
            }

            _isFacingRight = value;
        }
    }

    public bool _isFacingRight = true;
    // Start is called before the first frame update
    void Start()
    {
        // Đăng ký sự kiện khi kỹ năng được học
        SkillLearnButton.OnSkillLearned += RpcLearnSkill;
        isJumping = true; // Đặt isJumping thành true khi vừa bắt đầu chạy scene
    }

    Animator animator;

    // Đồng bộ di chuyển qua RPC
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcMovePlayer(Vector2 position)
    {
        // Di chuyển nhân vật trên các client khác
        transform.position = position;
    }

    public override void Spawned()
    {
        rb = GetComponent<Rigidbody2D>();
        base.Spawned();
        IsFacingRight = true; // Hoặc false tùy trạng thái mặc định
                              // Gán lại các thành phần khi spawn
        if (Object.HasStateAuthority)
        {
            SpawnPosition = transform.position; // Host đặt vị trí spawn
        }
        else
        {
            transform.position = SpawnPosition; // Client nhận vị trí spawn
        }

        Debug.Log($"Spawned at position: {SpawnPosition}");
        networkObject = GetComponent<NetworkObject>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageAble = GetComponent<DamageAble>();
        craftingPanel = GameObject.FindGameObjectWithTag("CraftingPanel");
        uiMap = GameObject.FindGameObjectWithTag("UIMap").GetComponent<UIMap>();

        // Gán lại các tham chiếu UI nếu cần
        questDialog = GameObject.FindGameObjectWithTag("QuestDialog");

    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcSetFacingDirection(bool facingRight)
    {
        IsFacingRight = facingRight;

        // Đảo chiều nhân vật
        transform.localScale = new Vector3(
            facingRight ? Mathf.Abs(transform.localScale.x) : -Mathf.Abs(transform.localScale.x),
            transform.localScale.y,
            transform.localScale.z
        );
    }

    private void Awake()
    {

        networkObject = GetComponent<NetworkObject>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageAble = GetComponent<DamageAble>();
        questDialog = GameObject.FindGameObjectWithTag("QuestDialog");


    }
    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            questDialog = GameObject.Find("QuestNPC").GetComponent<QuestDialog>().questDialog;
            moveInput = new Vector2(Input.GetAxis("Horizontal"), 0f); // Lưu input
            RpcMovePlayer(transform.position); // Chỉ gửi vị trí đồng bộ, không thay đổi rb.velocity ở đây
            // Kiểm tra nếu người chơi đang gần NPC và bấm phím T
            if (currentNPC != null && currentNPC.name == "BusinessManNPC" && Input.GetKeyDown(KeyCode.E))
            {
                ShopController shopController = GetComponent<ShopController>();
                if (shopController != null && shopController.uiShop != null)
                {
                    // Nếu UI Shop đang active, thì tắt nó
                    if (shopController.uiShop.gameObject.activeSelf)
                    {
                        shopController.uiShop.gameObject.SetActive(false);
                        Time.timeScale = 1f; // Đặt lại Time.timeScale về giá trị mặc định
                    }
                    // Nếu UI Shop đang inactive, thì kích hoạt nó
                    else
                    {
                        shopController.uiShop.gameObject.SetActive(true);
                        Time.timeScale = 0f; // Dừng thời gian
                    }
                }
            }
            if (currentNPC != null && currentNPC.name == "CraftingNPC" && Input.GetKeyDown(KeyCode.E))
            {
                CraftingController craftController = FindObjectOfType<CraftingController>();
                if (craftController != null && craftController.uiCrafting != null)
                {
                    // Nếu UI Shop đang active, thì tắt nó
                    if (craftController.uiCrafting.gameObject.activeSelf)
                    {
                        craftController.uiCrafting.gameObject.SetActive(false);
                        Time.timeScale = 1f; // Đặt lại Time.timeScale về giá trị mặc định
                    }
                    // Nếu UI Shop đang inactive, thì kích hoạt nó
                    else
                    {
                        craftController.uiCrafting.gameObject.SetActive(true);
                        Time.timeScale = 0f; // Dừng thời gian
                    }
                }
            }
            if (currentNPC != null && currentNPC.name == "QuestNPC" && Input.GetKeyDown(KeyCode.E))
            {
                questDialog.gameObject.SetActive(true);
            }
            if (isNearPortal && Input.GetKeyDown(KeyCode.E))
            {
                if (uiMap != null)
                {
                    isMapVisible = !isMapVisible; // Chuyển trạng thái hiển thị UI Map
                    if (isMapVisible)
                    {
                        uiMap.ShowMap(); // Hiển thị UI Map
                        Time.timeScale = 0f;
                    }
                    else
                    {
                        uiMap.HideMap(); // Ẩn UI Map
                        Time.timeScale = 1f;
                    }
                }
            }
        }
    }
    void OnDestroy()
    {
        // Hủy đăng ký sự kiện khi đối tượng bị hủy
        SkillLearnButton.OnSkillLearned -= RpcLearnSkill;
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    void RpcLearnSkill(SkillLearnButton.SkillType skillType)
    {
        if (IsOwner)
        {
            switch (skillType)
            {
                case SkillLearnButton.SkillType.CastAttack:
                    canCastAttack = true;
                    break;
                case SkillLearnButton.SkillType.ComboAttack:
                    canCastComboAttack = true;
                    break;
                case SkillLearnButton.SkillType.DoubleJump:
                    canDoubleJump = true;
                    break;
            }
        }
    }

    void EnableCastAttack()
    {
        canCastAttack = true;
    }
    void EnaleComboAttack()
    {
        canCastComboAttack = true;
    }
    private void FixedUpdate()
    {
        if (!damageAble.lockVelocity)
        {
            rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);
        }

        animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);
        currentPosition = transform.position;
    }
    public void OnMove(InputAction.CallbackContext context)
    {
        if (IsOwner) // Chỉ chủ sở hữu nhân vật mới có thể gửi lệnh
        {
            moveInput = context.ReadValue<Vector2>(); // Đọc giá trị đầu vào từ người chơi

            if (IsAlive)
            {
                IsMoving = moveInput != Vector2.zero;

                // Chỉ cập nhật hướng nếu nhân vật đang di chuyển
                if (IsMoving)
                {
                    SetFacingDirection(moveInput); // Cập nhật hướng sử dụng phương pháp mới
                }
            }
            else
            {
                IsMoving = false;
            }
        }
    }


    private void SetFacingDirection(Vector2 moveInput)
    {
        if (Mathf.Abs(moveInput.x) > 0.1f) // Chỉ xử lý nếu đầu vào có ý nghĩa
        {
            bool newFacingRight = moveInput.x > 0;
            if (newFacingRight != IsFacingRight) // Chỉ gửi RPC nếu hướng thay đổi
            {
                RpcSetFacingDirection(newFacingRight);
            }
        }
    }



    public bool CanMove
    {
        get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }

    public bool IsAlive
    {
        get
        {
            return animator.GetBool(AnimationStrings.isAlive);
        }
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    void RpcPlayAnimation(string animationTrigger)
    {
        animator.SetTrigger(animationTrigger);
    }

    // RPC để đồng bộ IsRunning
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcSetRunning(bool isRunning, bool isMoving)
    {
        IsMoving = isMoving;
        IsRunning = isRunning;
        animator.SetBool(AnimationStrings.isRunning, isRunning); // Đồng bộ với Animator
    }
    public void OnRun(InputAction.CallbackContext context)
    {
        if (IsOwner)
        {
            if (context.started)
            {
                RpcSetRunning(true,true); // Gọi RPC để đồng bộ trạng thái chạy
            }
            else if (context.canceled)
            {
                RpcSetRunning(false, false); // Gọi RPC để đồng bộ trạng thái chạy

            }
        }

    }
    [SerializeField]
    private bool hasDoubleJumped = false;
    [SerializeField]
    private bool isJumping = false;

    public void OnJump(InputAction.CallbackContext context)
    {
        if (IsOwner)
        {
            if (context.started && CanMove)
            {
                isJumping = true; // Đang nhảy
                if (touchingDirections.IsGrounded || touchingDirections.IsOnWall)
                {
                    // Nhảy lần đầu tiên
                    RpcPlayAnimation(AnimationStrings.jump_trigger);
                    rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
                    hasDoubleJumped = false; // Đặt lại double jump
                }
                else if (!hasDoubleJumped && !touchingDirections.IsGrounded && canDoubleJump)
                {
                    // Double jump
                    RpcPlayAnimation(AnimationStrings.jump_trigger);
                    rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
                    hasDoubleJumped = true; // Đánh dấu đã sử dụng double jump
                }
            }
        }
    }


    public void OnAttack(InputAction.CallbackContext context)
    {
        if (IsOwner)
        {
            if (context.started)
            {
                RpcPlayAnimation(AnimationStrings.attack_trigger);
                if (!isJumping)
                {
                    GetComponent<Attack>();
                }
                Debug.Log("Attack");
            }
        }
    }
    public void OnRangedAttack(InputAction.CallbackContext context)
    {
        if (IsOwner)
        {
            if (context.started)
            {
                RpcPlayAnimation(AnimationStrings.rangedAttackTrigger);
                if (!isJumping)
                {
                    GetComponent<ProjectileLauncher>().FireProjectile();
                }
                Debug.Log("Range Attack");
            }
        }

    }
    public void OnCastAttack(InputAction.CallbackContext context)
    {
        if (IsOwner)
        {
            if (context.started && canCastAttack)
            {
                // Kiểm tra xem Mana có đủ để thực hiện spell không
                if (damageAble.Mana >= spellManaCost)
                {
                    RpcPlayAnimation(AnimationStrings.castAttackTrigger);
                    damageAble.CastSpell(spellManaCost);

                    // Thực hiện CastLauncher nếu không phải đang nhảy
                    if (!isJumping)
                    {
                        GetComponent<CastLauncher>().CastProjectile();
                    }
                    Debug.Log("Cast Attack");
                }
                else
                {
                    // Xuất thông báo hoặc hiển thị một hình ảnh để cho người chơi biết rằng họ không có đủ Mana
                    Debug.Log("Not enough Mana to cast spell!");
                }
            }
        }

    }
    public GameObject currentNPC;
    public GameObject questDialog;
    public GameObject craftingPanel;
    public GameObject portal;
    public UIMap uiMap;
    private bool isNearPortal = false;
    private bool isMapVisible = false;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (IsOwner)
        {
            if (collision.CompareTag("NPC"))
            {
                // Lưu trữ GameObject của NPC vào currentNPC.
                currentNPC = collision.gameObject;
            }
            else if (collision.CompareTag("Portal"))
            {
                /*portal = collision.gameObject;
                // Hiển thị UI Map khi chạm vào Portal
                if (uiMap != null)
                {
                    uiMap.ShowMap();
                }*/
                isNearPortal = true;
            }
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (IsOwner)
        {
            if (collision.CompareTag("NPC"))
            {
                // Khi ra khỏi va chạm với NPC, đặt currentNPC thành null.
                currentNPC = null;
            }
            else if (collision.CompareTag("Portal"))
            {
                /*portal = null;
                if (uiMap != null)
                {
                    uiMap.HideMap();
                }*/
                isNearPortal = false;
            }
        }

    }

    public void OnComboAttack(InputAction.CallbackContext context)
    {
        if (IsOwner)
        {
            if (context.started && canCastComboAttack)
            {
                RpcPlayAnimation(AnimationStrings.comboAttackTrigger);
                Debug.Log("Combo Attack");
            }
        }

    }
    public void OnHit(int damage, Vector2 knockBack)
    {
        if (IsOwner)
        {
            rb.velocity = new Vector2(knockBack.x, rb.velocity.y + knockBack.y);
        }
    }

}
