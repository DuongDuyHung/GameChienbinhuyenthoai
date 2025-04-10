using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputFieldHandler : MonoBehaviour
{
    public TMP_InputField inputField;

    private void Start()
    {
        inputField.onValueChanged.AddListener(OnTextChanged);
    }

    private void OnTextChanged(string text)
    {
        // Tắt sự kiện tạm thời để tránh vòng lặp
        inputField.onValueChanged.RemoveListener(OnTextChanged);

        //// Chuyển toàn bộ text thành chữ hoa
        //inputField.text = text.ToUpper();

        // Kiểm tra nếu người dùng nhấn Space và thêm dấu cách
        if (Input.GetKeyDown(KeyCode.Space))
        {
            inputField.text += "Space";
        }

        // Bật lại sự kiện sau khi cập nhật xong
        inputField.onValueChanged.AddListener(OnTextChanged);
    }
}
