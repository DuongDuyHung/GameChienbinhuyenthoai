using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UICraftingDescription : MonoBehaviour
{
    [SerializeField]
    private Image itemImage;
    [SerializeField]
    private TMP_Text title;
    [SerializeField]
    private TMP_Text description;
    [SerializeField]
    private TMP_Text materialRequirement;

    public void SetDescription(string newTitle, string newDescription, string newMaterialRequirement, Sprite newItemImage)
    {
        if (title != null)
        {
            title.text = newTitle;
        }
        if (description != null)
        {
            description.text = newDescription;
        }
        if (materialRequirement != null)
        {
            materialRequirement.text = newMaterialRequirement; // Display the material requirements
        }
        if (itemImage != null)
        {
            itemImage.sprite = newItemImage;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
