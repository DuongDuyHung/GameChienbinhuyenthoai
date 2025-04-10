using Inventory.Model;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [field : SerializeField]
    public ItemSO InventoryItem {  get; private set; }
    [field: SerializeField]
    public int Quantity { get; set; } = 1;
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    private float duration = .3f;
    private float yVelocity = 4f;
    private bool isPickedUp = false; // Track if item was picked up
    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = InventoryItem.ItemImage;
        StartCoroutine(AutoDestroyItem()); // Start the auto-destroy timer
    }
    internal void DestroyItem()
    {
        isPickedUp = true; // Mark as picked up
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(AnimateItemPickup());
    }
    private IEnumerator AnimateItemPickup()
    {
        audioSource.Play();
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.zero;
        float currentTime = 0;
        while(currentTime < duration)
        {
            currentTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, endScale, currentTime/duration);
            yield return null;
        }
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger entered with " + collision.gameObject.name);

        // Lấy layer của Collider2D
        int collisionLayer = collision.gameObject.layer;

        // Lấy layer của "Ground" bằng cách sử dụng LayerMask.NameToLayer
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
    private IEnumerator AutoDestroyItem()
    {
        yield return new WaitForSeconds(30f); // Wait for 30 seconds

        if (!isPickedUp) // Destroy if item has not been picked up
        {
            Destroy(gameObject);
        }
    }

}