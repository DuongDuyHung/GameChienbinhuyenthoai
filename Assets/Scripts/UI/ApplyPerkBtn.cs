using UnityEngine;
using UnityEngine.UI;

public class ApplyPerkBtn : MonoBehaviour
{
    public PerkUIManager perkUIManager;

    private Button applyButton;

    private void Start()
    {
        applyButton = GetComponent<Button>();
        applyButton.onClick.AddListener(ApplyPerks);
    }

    public void ApplyPerks()
    {
        perkUIManager.ApplyPerks();
    }
}