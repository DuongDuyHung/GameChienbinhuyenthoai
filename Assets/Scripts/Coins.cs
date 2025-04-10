using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coins : MonoBehaviour
{
    public int minCoinValue = 1; // Giá trị xu tối thiểu
    public int maxCoinValue = 10; // Giá trị xu tối đa
    public int coinValue;
    private float yVelocity = 4f;
    private InventoryController inventoryController;
    private bool isPickedUp = false; // Track if the coin has been picked up


    private void Awake()
    {
        // Đặt coinValue thành một giá trị ngẫu nhiên trong khoảng từ minCoinValue đến maxCoinValue
        coinValue = Random.Range(minCoinValue, maxCoinValue + 1); // +1 vì giá trị tối đa là không bao gồm
        inventoryController = FindObjectOfType<InventoryController>();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger entered with " + collision.gameObject.name);
        
        // Check if the collision is with the player
        if (collision.gameObject.CompareTag("Playerr"))
        {
            if (inventoryController != null)
            {
                inventoryController.AddCoins(coinValue);

                // Mark as picked up and destroy the coin
                isPickedUp = true;
                // Sau khi nhặt coin, phá hủy đối tượng coin
                Destroy(gameObject);
            }
        }
        
        // Lấy layer của Collider2D
        int collisionLayer = collision.gameObject.layer;
        int groundLayer = LayerMask.NameToLayer("Ground");

        // So sánh layer của collision với layer của Ground
        if (collisionLayer == groundLayer)
        {
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Decrease the y-velocity by 0.5
                yVelocity -= 0.5f;

                // If the y-velocity is less than or equal to 0, set it to 0
                if (yVelocity <= 0)
                {
                    yVelocity = 0;
                    rb.gravityScale = 0;
                }
                rb.velocity = new Vector2(rb.velocity.x, yVelocity);
            }
        }
    }
    private IEnumerator AutoDestroyCoin()
    {
        yield return new WaitForSeconds(30f); // Wait for 30 seconds

        if (!isPickedUp) // Destroy if the coin has not been picked up
        {
            Destroy(gameObject);
        }
    }
    void Start() 
    {
        StartCoroutine(AutoDestroyCoin()); // Start the auto-destroy timer
    }

    void Update() { }
}
