using System;
using TMPro;
using UnityEngine;

/// <summary>
/// 底部任务栏 — 显示当前系统时间。
/// </summary>
public class Taskbar : MonoBehaviour
{
    [SerializeField] private TMP_Text clockText;

    private void Update()
    {
        if (clockText != null)
            clockText.text = DateTime.Now.ToString("HH:mm");
    }
}
