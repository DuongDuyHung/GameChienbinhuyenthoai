using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HomePage : MonoBehaviour
{
    public CanvasGroup choiTiep;
    public void Choi()
    {
        // Kiểm tra nếu tệp seed.json tồn tại, thì xóa nó
        if (File.Exists(MapDataSaver.filePath))
        {
            File.Delete(MapDataSaver.filePath);
            Debug.Log("Deleted existing seed file.");
        }
        if (File.Exists(MapDataSaver.hostSeedFilePath))
        {
            File.Delete(MapDataSaver.hostSeedFilePath);
            Debug.Log("Deleted existing seed file.");
        }
        /*if (File.Exists(MapDataSaver.npcFilePath))
        {
            File.Delete(MapDataSaver.npcFilePath);
            Debug.Log("Deleted existing seed file.");
        }*/
        // Kiểm tra nếu tệp seed.json tồn tại, thì xóa nó
        if (File.Exists(PlayerDataSaver.filePath))
        {
            File.Delete(PlayerDataSaver.filePath);
            Debug.Log("Deleted existing player save file.");
        }
        // Tiến hành tải cảnh mới
        SceneManager.LoadSceneAsync(1);
    }
    public void Start()
    {
        // Kiểm tra xem tệp JSON có tồn tại hay không
        if (/*!File.Exists(MapDataSaver.filePath) ||*/ !File.Exists(MapDataSaver.hostSeedFilePath))
        {
            // Nếu không tồn tại, đặt alpha của CanvasGroup thành 0.3 và vô hiệu hóa khả năng tương tác
            choiTiep.alpha = 0.6f;
            choiTiep.interactable = false;
        }
        else
        {
            choiTiep.alpha = 1;
            choiTiep.interactable = true;
        }
    }
    public void ChoiTiep()
    {
        // Tiến hành tải cảnh mới
        SceneManager.LoadSceneAsync(1);
    }
    /*public void Multiplayer()
    {
        // Tiến hành tải cảnh phòng chờ (WaitingRoom)
        SceneManager.LoadSceneAsync("WaitingRoom");
    }*/
    public void Thoat()
    {
        // Tắt game
        Application.Quit();
    }
}
