using System.Collections;
using UnityEngine;
using UnityEngine.UI;  // Để sử dụng Button
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public GameObject Tutorial; // Gán panel Tutorial từ Inspector
    public Button startButton;
    public GameObject loadingPanel; // Màn hình loading
    public GameObject[] highlightAreas;  // Mảng các vùng sáng
    public TextMeshProUGUI[] areaNames;  // Mảng các tên của vùng sáng
    private int currentHighlightIndex = 0;  // Vị trí hiện tại của vùng sáng
    public Button skipButton;  // Nút bỏ qua
    public GameObject darkOverlay; // Lớp phủ tối (DarkOverlay)
    public GameObject highlightMask; // Mask container cho vùng sáng

    private void Start()
    {
        skipButton.gameObject.SetActive(false);
        // Ẩn tất cả các vùng sáng và tên ban đầu
        HideAllHighlights();

        // Gán sự kiện cho nút Start
        if (startButton != null)
        {
            startButton.onClick.AddListener(OnStartButtonClicked);
        }

        // Thêm sự kiện cho nút bỏ qua
        if (skipButton != null)
        {
            skipButton.onClick.AddListener(SkipTutorial);
        }
        else
        {
            Debug.LogWarning("Skip button not assigned!");
        }
    }

    private void OnStartButtonClicked()
    {
        if (loadingPanel != null)
        {
            StartCoroutine(HandleStartSequence());
        }
    }

    private IEnumerator HandleStartSequence()
    {
        // Hiển thị loading screen
        loadingPanel.SetActive(true);

        // Chờ 1 giây để hiển thị highlight mask
        yield return new WaitForSeconds(1f);
        highlightMask.SetActive(true); // Hiển thị highlight mask

        // Chờ thêm 2 giây để ẩn loading screen (tổng thời gian là 3 giây)
        yield return new WaitForSeconds(2f);
        loadingPanel.SetActive(false); // Ẩn loading screen

        // Hiển thị vùng sáng đầu tiên
        ShowHighlightArea(currentHighlightIndex);

        // Kích hoạt nút Skip
        skipButton.gameObject.SetActive(true);

        // Tạm dừng trò chơi
        Time.timeScale = 0;
    }

    public void HideTutorial()
    {
        Tutorial.SetActive(false); // Ẩn panel Tutorial
        Time.timeScale = 1;  // Tiếp tục trò chơi
    }

    public void ShowTutorialImmediately()
    {
        Tutorial.SetActive(true); // Hiển thị Tutorial
        Time.timeScale = 0;  // Tạm dừng trò chơi
    }

    private void Update()
    {
        // Kiểm tra người chơi có bấm chuột phải để chuyển sang vùng sáng tiếp theo không
        if (Input.GetMouseButtonDown(1))  // Chuột phải
        {
            // Chuyển sang vùng sáng tiếp theo
            MoveToNextHighlight();
        }
    }

    private void ShowHighlightArea(int index)
    {
        // Kiểm tra chỉ số trước khi truy cập mảng để tránh lỗi
        if (index >= 0 && index < highlightAreas.Length)
        {
            // Hiển thị vùng sáng và tên của vùng đó
            highlightAreas[index].SetActive(true);
            areaNames[index].gameObject.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Index out of bounds in ShowHighlightArea!");
        }
    }

    private void HideAllHighlights()
    {
        // Ẩn tất cả các vùng sáng và tên
        foreach (var area in highlightAreas)
        {
            area.SetActive(false);
        }
        foreach (var name in areaNames)
        {
            name.gameObject.SetActive(false);
        }
    }

    private void MoveToNextHighlight()
    {
        // Ẩn vùng sáng và tên hiện tại
        HideAllHighlights();

        // Kiểm tra nếu đã đến vùng sáng cuối cùng
        if (currentHighlightIndex >= highlightAreas.Length - 1)
        {
            // Nếu nhấn chuột phải thêm lần nữa sau khi đã hiển thị vùng sáng cuối cùng
            highlightMask.SetActive(false); // Ẩn highlight mask
            currentHighlightIndex = 0; // Reset chỉ số vùng sáng về 0 nếu muốn bắt đầu lại
            Time.timeScale = 1;
        }
        else
        {
            // Tăng chỉ số vùng sáng
            currentHighlightIndex++;
            ShowHighlightArea(currentHighlightIndex); // Hiển thị vùng sáng mới
        }
    }

    private void SkipTutorial()
    {
        // Ẩn tất cả các vùng sáng và tên khi người chơi bỏ qua tutorial
        HideAllHighlights();
        highlightMask.SetActive(false);
        skipButton.gameObject.SetActive(false);
        Debug.Log("Tutorial skipped!");
        // Tiếp tục trò chơi
        Time.timeScale = 1;
    }
}
