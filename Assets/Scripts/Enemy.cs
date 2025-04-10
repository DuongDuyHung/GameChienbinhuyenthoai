using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(DamageAble))]
public class Enemy : NetworkBehaviour
{
    public float walkSpeed = 3f;
    public float walkStopRate = 0.05f;
    public float detectionRange = 10f; // Phạm vi phát hiện người chơi
    public Transform player; // Tham chiếu đến người chơi
    Rigidbody2D rb;
    Animator animator;
    TouchingDirections touchingDirections;
    public DetectionZone attackZone;
    public DetectionZone cliffDetectionZone;
    public DetectionZone stairsDetectionZone;
    DamageAble damageAble;
    private Attack attackScript;
    public GameObject swordAttackPrefab;
    public enum WalkableDirection { Left, Right };
    private WalkableDirection _walkDirection;
    private Vector2 walkDirectionVector = Vector2.right;
    public float disappearTime = 10f; // Time after which enemy will disappear if player is out of range
    private float outOfRangeTimer = 0f;
    private NetworkObject networkObject;
    [Networked] private WalkableDirection walkDirection { get; set; }

    [Networked] private Vector3 Position { get; set; }
    [Networked] private TickTimer UpdateTimer { get; set; }
    public WalkableDirection WalkDirection
    {
        get { return _walkDirection; }
        set
        {
            if (_walkDirection != value)
            {
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y);
                if (value == WalkableDirection.Right)
                {
                    walkDirectionVector = Vector2.right;
                }
                else if (value == WalkableDirection.Left)
                {
                    walkDirectionVector = Vector2.left;
                }
            }
            _walkDirection = value;
        }
    }
    public override void Spawned()
    {
        base.Spawned();
        player = GetClosestPlayer(); // Chỉ gọi khi kẻ địch spawn
        Debug.Log($"Enemy spawned on client {Runner.LocalPlayer.PlayerId}");
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageAble = GetComponent<DamageAble>();
        // Lấy tham chiếu đến GameObject có tag "Player"
        player = GameObject.FindGameObjectWithTag("Playerr")?.transform;
        if (player == null)
        {
            Debug.LogError("Player with tag 'Playerr' not found!");
        }
        networkObject = GetComponent<NetworkObject>();
        // Khởi tạo và thiết lập currentAttackType cho script Attack
        attackScript = GetComponent<Attack>();
        if (attackScript != null)
        {
            attackScript.currentAttackType = Attack.AttackType.Magic; // Thiết lập currentAttackType là Magic
        }
    }
    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)  // Chỉ host mới thay đổi hướng quay
        {
            // Kiểm tra xem kẻ địch có cần thay đổi hướng quay không
            if (walkDirection == WalkableDirection.Right && transform.localScale.x < 0)
            {
                FlipDirection();
            }
            else if (walkDirection == WalkableDirection.Left && transform.localScale.x > 0)
            {
                FlipDirection();
            }
            // Check if target is detected
            bool newHasTarget = attackZone.detectedColliders.Count > 0;
            if (newHasTarget != HasTarget)
            {
                RpcUpdateCanMove(false);
                RpcUpdateTargetStatus(newHasTarget);  // Update target status across all clients
            }

            // Check attack cooldown
            if (AttackCooldown > 0)
            {
                RpcUpdateCanMove(true);
                AttackCooldown -= Time.deltaTime;
                RpcUpdateAttackCooldown(AttackCooldown);  // Update cooldown across all clients
            }
        }
        // Cập nhật hướng quay qua RPC cho client
        RpcUpdateWalkDirection(walkDirection);
    }


    private void FixedUpdate()
    {
        bool playerInAttackRange = attackZone.detectedColliders.Count > 0;
        bool playerInDetectionRange = Vector2.Distance(transform.position, player.position) <= detectionRange;
        if (touchingDirections.IsOnWall && stairsDetectionZone.detectedColliders.Count > 0)
        {
            // Nếu có bậc thang phía trước, thực hiện nhảy lên bậc thang
            rb.velocity = new Vector2(rb.velocity.x, 4f); // Giả sử 4f là giá trị nhảy
            Debug.Log("Enemy is jumping over stairs.");
        }

        if (playerInDetectionRange)
        {
            outOfRangeTimer = 0f; // Reset timer if player is in range
            ChasePlayer(); // Gọi hàm đuổi theo người chơi
        }
        else
        {
            outOfRangeTimer += Time.deltaTime;

            // Nếu kẻ địch không phát hiện player trong khoảng thời gian nhất định, biến mất
            if (outOfRangeTimer >= disappearTime)
            {
                RpcHideEnemy(); // Gọi RPC để ẩn kẻ địch trên tất cả các client
            }
        }

        if (player != null && CanMove)
        {
            if (Vector2.Distance(transform.position, player.position) <= detectionRange)
            {
                ChasePlayer();
            }
        }

        // Kiểm tra nếu player có trong vùng attackZone không
        if (playerInAttackRange)
        {
            CanMove = false; // Dừng di chuyển khi vào tầm đánh
        }
        else
        {
            CanMove = true; // Di chuyển khi không ở trong tầm đánh
        }

        // Kiểm tra thêm điều kiện để dừng di chuyển nếu không còn ở trong phạm vi tầm đánh
        if (Vector2.Distance(transform.position, player.position) > detectionRange)
        {
            // Nếu kẻ địch ra ngoài tầm phát hiện, có thể dừng lại hoặc quay lại
            IsMoving = false;
            RpcUpdateEnemyAnimation(IsMoving, IsRunning); // Gọi RPC để dừng animation di chuyển
        }

        RpcUpdateTargetStatus(HasTarget);
    }

    private void ChasePlayer()
    {
        // Chỉ di chuyển khi người chơi trong tầm phát hiện
        if (Vector2.Distance(transform.position, player.position) <= detectionRange)
        {
            Vector2 moveDirection = (player.position - transform.position).normalized;
            if (Mathf.Sign(moveDirection.x) == 1 && walkDirection != WalkableDirection.Right)
            {
                walkDirection = WalkableDirection.Right;
                RpcUpdateWalkDirection(walkDirection); // Cập nhật hướng quay
            }
            else if (Mathf.Sign(moveDirection.x) == -1 && walkDirection != WalkableDirection.Left)
            {
                walkDirection = WalkableDirection.Left;
                RpcUpdateWalkDirection(walkDirection); // Cập nhật hướng quay
            }

            // Di chuyển kẻ địch
            rb.velocity = new Vector2(moveDirection.x * walkSpeed, rb.velocity.y);
        }
    }
    private bool IsInGroundLayer()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f, LayerMask.GetMask("Ground"));

        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Ground"))
            {
                return true;
            }
        }

        return false;
    }
    private void MoveOutOfGroundLayer()
    {
        Vector2 desiredPosition = transform.position;
        Vector2 direction = walkDirectionVector;

        // Tìm vị trí bên ngoài layer "Ground"
        while (IsInGroundLayer())
        {
            desiredPosition += direction * 0.1f;
            transform.position = desiredPosition;
        }

        // Di chuyển enemy theo hướng bình thường
        rb.velocity = new Vector2(walkSpeed * walkDirectionVector.x, rb.velocity.y);
    }
    private void FlipDirection()
    {
        if (!HasTarget)
        {
            if (WalkDirection == WalkableDirection.Right || transform.localScale.x > 0)
            {
                WalkDirection = WalkableDirection.Left;
                transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                walkDirectionVector = new Vector2(-Mathf.Abs(walkDirectionVector.x), walkDirectionVector.y);
            }
            else if (WalkDirection == WalkableDirection.Left || transform.localScale.x < 0)
            {
                WalkDirection = WalkableDirection.Right;
                transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                walkDirectionVector = new Vector2(Mathf.Abs(walkDirectionVector.x), walkDirectionVector.y);
            }
            else
            {
                Debug.Log("Current walkable direction is not set to legal values of left or right");
            }
        }
    }
    private void Start()
    {
    }

    public bool _hasTarget = false;
    public bool HasTarget
    {
        get { return _hasTarget; }
        private set
        {
            _hasTarget = value;
            animator.SetBool(AnimationStrings.hasTarget, value);
        }
    }

    public bool CanMove
    {
        get { return animator.GetBool(AnimationStrings.canMove); }
        set
        {
            // Check if the value has changed before calling the RPC
            if (animator.GetBool(AnimationStrings.canMove) != value)
            {
                animator.SetBool(AnimationStrings.canMove, value);  // Update the animator locally
                RpcUpdateCanMove(value);  // Synchronize the change across the network
            }
        }
    }


    public float AttackCooldown
    {
        get { return animator.GetFloat(AnimationStrings.attackCooldown); }
        private set { animator.SetFloat(AnimationStrings.attackCooldown, Mathf.Max(value, 0)); }
    }
    // Đồng bộ di chuyển qua RPC
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcMoveEnemy(Vector2 position)
    {
        // Di chuyển nhân vật trên các client khác
        transform.position = position;
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcUpdateCanMove(bool canMove)
    {
        animator.SetBool(AnimationStrings.canMove, canMove);  // Update the animator on all clients
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcUpdateEnemyAnimation(bool isMoving, bool isRunning)
    {
        animator.SetBool(AnimationStrings.isMoving, isMoving);
        animator.SetBool(AnimationStrings.isRunning, isRunning);
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcUpdateTargetStatus(bool hasTarget)
    {
        animator.SetBool(AnimationStrings.hasTarget, hasTarget);  // Cập nhật animation hoặc các hành động khác
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcUpdateAttackCooldown(float cooldown)
    {
        animator.SetFloat(AnimationStrings.attackCooldown, cooldown);  // Update the cooldown value in the animator
    }

    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcUpdateWalkDirection(WalkableDirection direction)
    {
        // Cập nhật hướng quay cho client
        if (direction == WalkableDirection.Left)
        {
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
        else if (direction == WalkableDirection.Right)
        {
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        }
    }
    [Rpc(RpcSources.All, RpcTargets.All)]
    public void RpcHideEnemy()
    {
        gameObject.SetActive(false);  // Tắt kẻ địch trên tất cả các client
    }

    private void Update()
    {
        if (HasStateAuthority)  // Chỉ host mới gọi RPC
        {
            RpcMoveEnemy(transform.position);
        }

        HasTarget = attackZone.detectedColliders.Count > 0;

        if (AttackCooldown > 0)
        {
            AttackCooldown -= Time.deltaTime;
        }
    }

    private Transform GetClosestPlayer()
    {
        Transform closestPlayer = null;
        float closestDistance = Mathf.Infinity;

        foreach (var player in Runner.ActivePlayers)
        {
            var playerObject = Runner.GetPlayerObject(player);
            if (playerObject != null)
            {
                var playerController = playerObject.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    float distance = Vector2.Distance(transform.position, playerController.currentPosition);
                    if (distance < closestDistance)
                    {
                        closestDistance = distance;
                        closestPlayer = playerObject.transform;
                    }
                }
            }
        }

        return closestPlayer;
    }
    [SerializeField]
    private bool _isMoving = false;
    public bool IsMoving
    {
        get { return _isMoving; }
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
        get { return _isRunning; }
        set
        {
            _isRunning = value;
            animator.SetBool(AnimationStrings.isRunning, value);
        }
    }
    public void OnDamaged(int damage, Vector2 knockBack)
    {
        rb.velocity = new Vector2(knockBack.x, rb.velocity.y + knockBack.y);
    }

    //public void OnCliffDetected()
    //{
    //    if (touchingDirections.IsGrounded)
    //    {
    //        FlipDirection();
    //    }
    //}
}
