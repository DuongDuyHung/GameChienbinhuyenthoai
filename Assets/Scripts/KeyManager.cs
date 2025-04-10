using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBindingManager : MonoBehaviour
{
    public GameObject settingkeyboard;
    public void HideSettingkeyboard()
    {
        settingkeyboard.SetActive(false); // Ẩn panel Tutorial

    }

    public void ShowSettingkeyboard()
    {
        settingkeyboard.SetActive(true); // Hiện panel SettingKeyboard
    }

}
