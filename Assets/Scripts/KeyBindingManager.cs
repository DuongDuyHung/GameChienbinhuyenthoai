using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.InputSystem.Utilities;
using System.IO;

public class KeybindingManager : MonoBehaviour
{
    public static KeybindingManager Instance { get; private set; }
    public InputActionAsset playerInput;
    public Button applyButton;

    // Các InputField để người chơi nhập phím mới
    public TMP_InputField jumpInputField;
    public TMP_InputField leftInputField;
    public TMP_InputField rightInputField;
    public TMP_InputField shootInputField;
    public TMP_InputField castInputField;
    public TMP_InputField ultiInputField;
    public TMP_InputField runInputField;

    private InputAction jumpAction;
    private InputAction moveAction;
    private InputAction shootAction;
    private InputAction castAction;
    private InputAction ultiAction;
    private InputAction runAction;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {

        // Lấy action "Jump" từ InputActionAsset
        jumpAction = playerInput.FindAction("Player/Jump"); // Thay "Player/Jump" bằng đường dẫn đúng của action
        shootAction = playerInput.FindAction("Player/BanCung");
        castAction = playerInput.FindAction("Player/TungChuong");
        ultiAction = playerInput.FindAction("Player/Combo");
        runAction = playerInput.FindAction("Player/Chay");
        // Lấy action composite "Move"
        moveAction = playerInput.FindAction("Player/Move");

        // Lấy các binding của composite action "Move" (WASD)
        var moveBindings = moveAction.bindings;
        // Cập nhật InputField với phím hiện tại của Jump
        UpdateInputField(jumpAction, jumpInputField);
        UpdateInputField(shootAction, shootInputField);
        UpdateInputField(castAction, castInputField);
        UpdateInputField(ultiAction, ultiInputField);
        UpdateInputField(runAction, runInputField);
        // Cập nhật các binding của Left và Right trong composite action "WASD"
        UpdateMoveInputField(moveBindings, "left", leftInputField);
        UpdateMoveInputField(moveBindings, "right", rightInputField);
        // Thêm listener cho việc thay đổi phím trong input field
        jumpInputField.onValueChanged.AddListener(OnJumpKeyChange);
        shootInputField.onValueChanged.AddListener(OnShootKeyChange);
        castInputField.onValueChanged.AddListener(OnCastKeyChange);
        ultiInputField.onValueChanged.AddListener(OnUltiKeyChange);
        leftInputField.onValueChanged.AddListener(OnLeftKeyChange);
        rightInputField.onValueChanged.AddListener(OnRightKeyChange);
        runInputField.onValueChanged.AddListener(OnRunKeyChange);
        // Thêm listener cho nút "Lưu"
        applyButton.onClick.AddListener(SaveKeyBindings);
    }
    private void UpdateMoveInputField(ReadOnlyArray<InputBinding> bindings, string direction, TMP_InputField inputField)
    {
        for (int i = 0; i < bindings.Count; i++)
        {
            if (bindings[i].name != null && bindings[i].name.Contains(direction))
            {
                inputField.text = bindings[i].ToDisplayString(); // Hiển thị tên phím hiện tại
                break;
            }
        }
    }


    private void OnLeftKeyChange(string newKey)
    {
        if (newKey.Length > 0)
        {
            int leftBindingIndex = -1;
            for (int i = 0; i < moveAction.bindings.Count; i++)
            {
                if (moveAction.bindings[i].name == "left")
                {
                    leftBindingIndex = i;
                    break;
                }
            }

            if (leftBindingIndex != -1)
            {
                moveAction.ApplyBindingOverride(leftBindingIndex, $"<Keyboard>/{newKey}");
            }
        }
    }

    private void OnRightKeyChange(string newKey)
    {
        if (newKey.Length > 0)
        {
            int rightBindingIndex = -1;
            for (int i = 0; i < moveAction.bindings.Count; i++)
            {
                if (moveAction.bindings[i].name == "right")
                {
                    rightBindingIndex = i;
                    break;
                }
            }

            if (rightBindingIndex != -1)
            {
                moveAction.ApplyBindingOverride(rightBindingIndex, $"<Keyboard>/{newKey}");
            }
        }
    }

    // Hàm này sẽ cập nhật input field với phím hiện tại của action
    private void UpdateInputField(InputAction action, TMP_InputField inputField)
    {
        var bindings = action.bindings;
        if (bindings.Count > 0)
        {
            inputField.text = bindings[0].ToDisplayString(); // Hiển thị binding đầu tiên
        }
    }

    // Hàm này sẽ được gọi khi người dùng thay đổi phím trong input field
    private void OnJumpKeyChange(string newKey)
    {
        if (newKey.Length > 0)
        {
            jumpAction.RemoveAllBindingOverrides();
            jumpAction.ApplyBindingOverride("<Keyboard>/" + newKey);
        }
    }
    private void OnShootKeyChange(string newKey)
    {
        if (newKey.Length > 0)
        {
            shootAction.RemoveAllBindingOverrides();
            shootAction.ApplyBindingOverride("<Keyboard>/" + newKey);
        }
    }
    private void OnCastKeyChange(string newKey)
    {
        if (newKey.Length > 0)
        {
            castAction.RemoveAllBindingOverrides();
            castAction.ApplyBindingOverride("<Keyboard>/" + newKey);
        }
    }
    private void OnUltiKeyChange(string newKey)
    {
        if (newKey.Length > 0)
        {
            ultiAction.RemoveAllBindingOverrides();
            ultiAction.ApplyBindingOverride("<Keyboard>/" + newKey);
        }
    }
    private void OnRunKeyChange(string newKey)
    {
        if (newKey.Length > 0)
        {
            runAction.RemoveAllBindingOverrides();
            runAction.ApplyBindingOverride("<Keyboard>/" + newKey);
        }
    }
    public void SaveKeyBindings()
    {
        KeybindingsData keybindings = new KeybindingsData
        {
            jumpKey = jumpInputField.text,
            shootKey = shootInputField.text,
            castKey = castInputField.text,
            ultiKey = ultiInputField.text,
            leftKey = leftInputField.text,
            rightKey = rightInputField.text,
            runKey = runInputField.text,
        };

        // Convert dữ liệu keybindings thành JSON
        string json = JsonUtility.ToJson(keybindings, true);

        // Tạo thư mục nếu chưa tồn tại
        string directoryPath = @"C:\SaveLoadTest";
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        // Đường dẫn file JSON
        string filePath = Path.Combine(directoryPath, "keybindings.json");

        // Lưu JSON vào file
        File.WriteAllText(filePath, json);

        Debug.Log("Keybindings đã được lưu vào file: " + filePath);
    }
    public void LoadKeyBindings()
    {
        string directoryPath = @"C:\SaveLoadTest";
        string filePath = Path.Combine(directoryPath, "keybindings.json");

        // Kiểm tra nếu file tồn tại
        if (File.Exists(filePath))
        {
            // Đọc nội dung file
            string json = File.ReadAllText(filePath);

            // Chuyển đổi JSON thành đối tượng KeybindingsData
            KeybindingsData keybindings = JsonUtility.FromJson<KeybindingsData>(json);

            // Cập nhật lại các keybindings vào input fields
            jumpInputField.text = keybindings.jumpKey;
            shootInputField.text = keybindings.shootKey;
            castInputField.text = keybindings.castKey;
            ultiInputField.text = keybindings.ultiKey;
            leftInputField.text = keybindings.leftKey;
            rightInputField.text = keybindings.rightKey;
            runInputField.text = keybindings.runKey;

            // Áp dụng lại các keybindings vào game (có thể cần phải áp dụng lại trực tiếp cho các action)
            OnJumpKeyChange(keybindings.jumpKey);
            OnShootKeyChange(keybindings.shootKey);
            OnCastKeyChange(keybindings.castKey);
            OnUltiKeyChange(keybindings.ultiKey);
            OnLeftKeyChange(keybindings.leftKey);
            OnRightKeyChange(keybindings.rightKey);
            OnRunKeyChange(keybindings.runKey);

            Debug.Log("Keybindings đã được tải từ file: " + filePath);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy file keybindings.json");
        }
    }

}
[System.Serializable]
public class KeybindingsData
{
    public string jumpKey;
    public string shootKey;
    public string castKey;
    public string ultiKey;
    public string leftKey;
    public string rightKey;
    public string runKey;
}
