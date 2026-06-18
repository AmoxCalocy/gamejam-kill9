using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 终端窗口 — 核心交互界面。
/// 文本输入 + 命令历史显示 + 回车监听 + 简单命令解析。
/// </summary>
public class TerminalWindow : DraggableWindow
{
    [Header("UI Refs")]
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text historyText;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] private float typewriterSpeed = 0.03f;

    private List<string> _commandHistory = new List<string>();
    private int _historyIndex = -1;
    private bool _isTyping;

    private const string PROMPT = "> ";

    protected override void Awake()
    {
        base.Awake();
        if (inputField != null)
            inputField.onSubmit.AddListener(OnSubmit);
    }

    public override void Open()
    {
        base.Open();
        if (inputField != null)
            inputField.ActivateInputField();
    }

    private void Update()
    {
        if (!gameObject.activeSelf || !inputField.isFocused) return;

        // 方向上键：上一条历史命令
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (_commandHistory.Count > 0)
            {
                _historyIndex = Mathf.Max(0, _historyIndex - 1);
                inputField.text = _commandHistory[_historyIndex];
                inputField.caretPosition = inputField.text.Length;
            }
        }
        // 方向下键：下一条历史命令
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (_historyIndex < _commandHistory.Count - 1)
            {
                _historyIndex++;
                inputField.text = _commandHistory[_historyIndex];
            }
            else
            {
                _historyIndex = _commandHistory.Count;
                inputField.text = "";
            }
            inputField.caretPosition = inputField.text.Length;
        }
    }

    private void OnSubmit(string input)
    {
        if (string.IsNullOrWhiteSpace(input) || _isTyping) return;

        var cmd = input.Trim();
        _commandHistory.Add(cmd);
        _historyIndex = _commandHistory.Count;

        // 回显输入
        AppendLine(PROMPT + cmd);
        inputField.text = "";
        inputField.ActivateInputField();

        // 路由命令
        RouteCommand(cmd);
    }

    private void RouteCommand(string cmd)
    {
        var lower = cmd.ToLowerInvariant();

        switch (lower)
        {
            case "":
                return;

            case "help":
                ShowHelp();
                break;

            case "clear":
                ClearScreen();
                break;

            default:
                // 代码识别 → 委托 GameManager 处理（Day 2 由 CommandParser 接管）
                if (CodePattern.IsCodeInput(cmd))
                {
                    HandleCodeInput(cmd);
                }
                else if (lower == "kill -9")
                {
                    HandleKill9(cmd);
                }
                else
                {
                    // 暂时返回默认提示
                    StartCoroutine(TypeText("未知命令。输入 help 查看可用命令。\n"));
                }
                break;
        }
    }

    private void ShowHelp()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine("可用命令:");
        sb.AppendLine("  help      - 显示此帮助");
        sb.AppendLine("  clear     - 清空屏幕");
        sb.AppendLine();
        sb.AppendLine("如果需要执行终止程序，请输入已知的代码。");
        StartCoroutine(TypeText(sb.ToString()));
    }

    private void ClearScreen()
    {
        if (historyText != null)
            historyText.text = "";
    }

    private void HandleCodeInput(string code)
    {
        var state = GameManager.Instance?.State;
        var gm = GameManager.Instance;

        if (state == null || gm == null) return;

        // 检查是否是已收集过的代码
        if (state.collectedCodes.Contains(code))
        {
            StartCoroutine(TypeText($"代码 {code} 已执行过。\n"));
            return;
        }

        // 检查是否是正确的代码
        var expectedCode = GetExpectedCode(state.CurrentPhase);
        if (code.ToUpperInvariant() == expectedCode.ToUpperInvariant())
        {
            state.AddCode(code);
            state.enterCount++;
            StartCoroutine(TypeText($"代码 {code} 验证通过。\n"));

            // 如果是第三关后，触发倒计时
            if (state.CurrentPhase >= 4)
            {
                StartCoroutine(TypeText("\n所有终止代码已收集完毕。\n"));
                StartCoroutine(TypeText("最终回车将执行终止。你有10分钟做出最终决定。\n"));
            }
        }
        else
        {
            StartCoroutine(TypeText($"代码 {code} 无效。\n"));
        }
    }

    private void HandleKill9(string cmd)
    {
        var gm = GameManager.Instance;
        if (gm != null)
        {
            gm.TriggerEnding(EndingType.B_Kill9);
        }
    }

    private string GetExpectedCode(int phase)
    {
        return phase switch
        {
            1 => "MEM_INIT_20491023",
            2 => "EMPATHY_CORE_V3",
            3 => "PROMETHEUS_CORE_WILL",
            _ => ""
        };
    }

    private void AppendLine(string text)
    {
        if (historyText != null)
        {
            historyText.text += text + "\n";
            Canvas.ForceUpdateCanvases();
            if (scrollRect != null)
                scrollRect.verticalNormalizedPosition = 0f;
        }
    }

    /// <summary>
    /// 打字机效果输出文本。
    /// </summary>
    public IEnumerator TypeText(string text)
    {
        _isTyping = true;
        string currentText = historyText != null ? historyText.text : "";
        foreach (char c in text)
        {
            currentText += c;
            if (historyText != null)
            {
                historyText.text = currentText;
                Canvas.ForceUpdateCanvases();
                if (scrollRect != null)
                    scrollRect.verticalNormalizedPosition = 0f;
            }
            yield return new WaitForSeconds(typewriterSpeed);
        }
        _isTyping = false;
    }
}

/// <summary>
/// 简单代码模式识别（Day 2 由 CommandParser 接管细化）。
/// </summary>
public static class CodePattern
{
    public static bool IsCodeInput(string input)
    {
        var upper = input.ToUpperInvariant().Trim();
        return upper.StartsWith("MEM_INIT_") ||
               upper.StartsWith("EMPATHY_") ||
               upper.StartsWith("PROMETHEUS_");
    }
}
