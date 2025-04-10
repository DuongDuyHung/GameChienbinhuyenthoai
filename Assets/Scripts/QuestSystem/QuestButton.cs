using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestButton : MonoBehaviour
{
    public QuestUIManager questUIManager;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AutoDisableQuestUI());
    }
    private IEnumerator AutoDisableQuestUI()
    {
        // Đợi cho đến khi "Player" xuất hiện trong scene
        GameObject player = null;
        while (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Playerr");
            yield return null; // Chờ frame tiếp theo
        }

        // Bắt đầu tính thời gian sau khi "Playerr" đã xuất hiện
        if (questUIManager.gameObject.activeSelf)
        {
            yield return new WaitForSeconds(1f); // Chờ 1 giây
            questUIManager.gameObject.SetActive(false);
            Time.timeScale = 1;
        }
    }

    public void OnButtonClick()
    {
        questUIManager.gameObject.SetActive(!questUIManager.gameObject.activeSelf);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
