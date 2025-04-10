using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisconnectedScreen : MonoBehaviour
{
    [SerializeField] Image disconnectedPanel;
    [SerializeField] TMP_Text disconnectedText;
    [SerializeField] GameObject backButton;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Kiểm tra xem có đối tượng nào với tag "Player" trong scene hay không
        GameObject playerObject = GameObject.FindGameObjectWithTag("Playerr");
        if (playerObject == null)
        {
            // Không còn đối tượng nào với tag "Player", kích hoạt disconnectedPanel
            backButton.SetActive(true);
            disconnectedText.enabled = true;
            disconnectedPanel.enabled = true;
        }
        else
        {
            // Có đối tượng với tag "Player", ẩn disconnectedPanel
            backButton.SetActive(false);
            disconnectedText.enabled=false;
            disconnectedPanel.enabled = false;
        }
    }
}
