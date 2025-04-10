using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestDialog : MonoBehaviour
{
    public TMP_Text npcName;
    public TMP_Text npcDialog;
    public Button acceptButton;
    public Button rejectButton;
    public GameObject questDialog;
    // Start is called before the first frame update
    void Start()
    {
        npcName = GameObject.FindGameObjectWithTag("NPCName").GetComponent<TMP_Text>();
        npcDialog = GameObject.FindGameObjectWithTag("NPCDialog").GetComponent<TMP_Text>();
        questDialog = GameObject.FindGameObjectWithTag("QuestDialog");
        // Set the NPC name to the name of the GameObject this script is attached to
        npcName.text = gameObject.name;
        StartCoroutine(CheckForPlayerAndDeactivate());
    }
    IEnumerator CheckForPlayerAndDeactivate()
    {
        GameObject player = null;

        // Chờ đến khi đối tượng Player được spawn
        while (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Playerr");
            yield return null; // Chờ frame tiếp theo
        }

        // Khi Player được tìm thấy, bật UICrafting và bắt đầu đếm ngược
        if (questDialog != null)
        {
            questDialog.SetActive(true); // Hiển thị UICrafting
            yield return new WaitForSeconds(1); // Chờ 0.1 giây
            questDialog.SetActive(false); // Tắt UICrafting
        }
    }
    // Update is called once per frame
    void Update()
    {

    }
    private Quest currentQuest; // Store the currently displayed quest

    private void Awake()
    {

    }

    public void SetQuest(Quest quest)
    {
        currentQuest = quest;
        npcDialog.text = quest.description; // Hiển thị mô tả nhiệm vụ

        // Xóa sự kiện cũ trước khi thêm sự kiện mới
        acceptButton.onClick.RemoveAllListeners();
        rejectButton.onClick.RemoveAllListeners();

        acceptButton.onClick.AddListener(AcceptQuest);
        rejectButton.onClick.AddListener(RejectQuest);

        Debug.Log($"SetQuest called: {quest.title}, Description: {quest.description}");
    }


    public void AcceptQuest()
    {
        // Notify QuestManager about quest acceptance
        FindObjectOfType<QuestManager>().AcceptQuest(currentQuest);
        questDialog.SetActive(false); // Hide the dialog after accepting
    }

    public void RejectQuest()
    {
        // Close the dialog or perform rejection logic
        questDialog.SetActive(false);
    }
}