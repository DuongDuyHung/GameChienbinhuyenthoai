using Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Materials : MonoBehaviour
{
    public int materialQuantity;
    private InventoryController inventoryController;
    private bool isPickedUp = false;
    private float yVelocity = 4f;
    public MaterialType materialType;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AutoDestroyMaterial());

    }
    private void Awake()
    {
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
                if (materialType == MaterialType.Woods)
                {
                    inventoryController.AddMaterials(0, materialQuantity,0,0,0,0); // Add to woods
                }
                else if (materialType == MaterialType.Stones)
                {
                    inventoryController.AddMaterials(materialQuantity, 0,0,0,0,0); // Add to stones
                }
                else if (materialType == MaterialType.Copper)
                {
                    inventoryController.AddMaterials(0,0,materialQuantity, 0,0,0); // Add to stones
                }
                else if (materialType == MaterialType.Silver)
                {
                    inventoryController.AddMaterials(0,0,0,materialQuantity,0, 0); // Add to stones
                }
                else if (materialType == MaterialType.Gold)
                {
                    inventoryController.AddMaterials(0,0,0,0,materialQuantity, 0); // Add to stones
                }
                else if (materialType == MaterialType.Saphire)
                {
                    inventoryController.AddMaterials(0,0,0,0,0,materialQuantity); // Add to stones
                }

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
    private IEnumerator AutoDestroyMaterial()
    {
        yield return new WaitForSeconds(30f); // Wait for 30 seconds

        if (!isPickedUp) // Destroy if the coin has not been picked up
        {
            Destroy(gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public enum MaterialType
    {
        Stones,
        Woods,
        Copper,
        Silver,
        Gold,
        Saphire
    }
}
