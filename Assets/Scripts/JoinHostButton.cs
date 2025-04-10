using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JoinHostButton : MonoBehaviour
{
    public GameObject lobbyPanel;
    [SerializeField] private Button hostButton; // Nút Host
    [SerializeField] private Button joinButton; // Nút Join
    [SerializeField] private Button startButton; // Nút Start
    [SerializeField] private Button backButton; // Nút Start
    [SerializeField] private Button copyButton;
    [SerializeField] private NetworkManager networkManager; // NetworkManager tham chiếu

    void Start()
    {
        if (hostButton != null)
        {
            hostButton.onClick.AddListener(() =>
            {
                Time.timeScale = 0;
                networkManager.GameStart(GameMode.Host); // Bắt đầu host
                lobbyPanel.SetActive(true); // Hiển thị lobby khi Host
            });
        }

        if (joinButton != null)
        {
            joinButton.onClick.AddListener(() =>
            {
                Time.timeScale = 0;
                networkManager.GameStart(GameMode.Client); // Kết nối client
                lobbyPanel.SetActive(true); // Hiển thị lobby (nếu cần)
            });
        }

        if (startButton != null)
        {
            startButton.onClick.AddListener(() =>
            {
                Time.timeScale = 1;
                lobbyPanel.SetActive(false); // Ẩn lobby panel
                Debug.Log("Game started!");
                // Thêm logic bắt đầu game ở đây nếu cần
            });
        }
        if (copyButton != null)
        {
            copyButton.onClick.AddListener(() =>
            {
                if (networkManager != null && networkManager.roomcode != null)
                {
                    string roomCodeText = networkManager.roomcode.text.Replace("Mã phòng:", "").Trim();
                    GUIUtility.systemCopyBuffer = roomCodeText; // Sao chép mã phòng vào clipboard
                    Debug.Log($"Room code '{roomCodeText}' has been copied to clipboard!");
                }
                else
                {
                    Debug.LogError("Room code is not available or NetworkManager is null.");
                }
            });
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(() =>
            {
                SceneManager.LoadSceneAsync("HomePage");
            });
        }
    }
}
