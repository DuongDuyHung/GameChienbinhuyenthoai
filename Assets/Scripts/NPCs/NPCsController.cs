using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class NPCsController : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float walkStopRate = 0.05f;
    public float moveDurationThreshold = 2.0f; // Thời gian di chuyển tối thiểu trước khi đảo hướng
    private float moveDuration = 0.0f;
    public float stopDuration = 2f; // Khoảng thời gian dừng lại trước khi flip direction
    private float timeSinceLastStop = 0f; // Thời gian đã trôi qua kể từ lần cuối cùng dừng lại
    Rigidbody2D rb;
    Animator animator;
    TouchingDirections touchingDirections;
    public DetectionZone cliffDetectionZone;
    public DetectionZone stairsDetectionZone;
    public enum WalkableDirection { Left, Right };
    private WalkableDirection _walkDirection;
    private Vector2 walkDirectionVector = Vector2.right;
    //public UIShop uiShop;

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

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
    }
    private void FixedUpdate()
    {
        if (cliffDetectionZone.detectedColliders.Count == 0)
        {
            FlipDirection();
        }
        if (stairsDetectionZone.detectedColliders.Count > 0)
        {
            // Nếu có bậc thang phía trước, thực hiện di chuyển lên trên bậc thang
            rb.velocity = new Vector2(rb.velocity.x, 4f);
        }
        if (CanMove)
        {
            // Di chuyển enemy theo hướng bình thường
            rb.velocity = new Vector2(walkSpeed * walkDirectionVector.x, rb.velocity.y);
            moveDuration += Time.fixedDeltaTime;
            timeSinceLastStop += Time.fixedDeltaTime;
            if (moveDuration >= moveDurationThreshold)
            {
                // Đã di chuyển đủ thời gian, dừng lại và đảo hướng
                moveDuration = 0.0f; // Reset thời gian di chuyển
                if(timeSinceLastStop >= stopDuration)
                {
                    CanMove = false;
                    timeSinceLastStop = 0f; // Đặt lại thời gian đã trôi qua
                    StartCoroutine(DelayMovement());
                    FlipDirection();
                }
                else
                {
                    // Nếu chưa đủ thời gian dừng, tiếp tục tăng thời gian đã trôi qua
                    timeSinceLastStop += Time.fixedDeltaTime;
                }
            }
        }
        else
        {
            rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, walkStopRate), rb.velocity.y);
        }
    }
    // Coroutine để thiết lập CanMove = false và sau đó đợi 2 giây
    IEnumerator DelayMovement()
    {
        yield return new WaitForSeconds(stopDuration); // Đợi 2 giây

        CanMove = true; // Cho phép di chuyển lại
    }
    private void FlipDirection()
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
    private void Start()
    {
        //if (uiShop == null)
        //{
        //    Debug.LogError("UIShop not found in the scene. Please ensure there is a UIShop component attached to a Canvas.");
        //}
        //else
        //{
        //    uiShop.shopPanel.SetActive(false); // Ensure the panel is hidden by default
        //}
    }


    public bool CanMove
    {
        get { return animator.GetBool(AnimationStrings.canMove); }
        set { animator.SetBool(AnimationStrings.canMove, value); }
    }


    private void Update()
    {

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

    public void OnCliffDetected()
    {
        if (touchingDirections.IsGrounded)
        {
            FlipDirection();
        }
    }
}
