using UnityEngine;

/// <summary>
/// Desktop 场景管理器 — 控制桌面图标显隐、窗口管理、关卡入口、返回流程。
/// </summary>
public class DesktopManager : MonoBehaviour
{
    public static DesktopManager Instance { get; private set; }

    private bool _terminalFirstOpen = true;
    private bool _coreMonitorLocked;
    private string _pendingCode;

    [Header("Icons — 始终可见")]
    [SerializeField] private GameObject iconTerminal;
    [SerializeField] private GameObject iconPrometheusChat;
    [SerializeField] private GameObject iconSystemLog;

    [Header("Icons — 条件出现")]
    [SerializeField] private GameObject iconCoreMonitor;
    [SerializeField] private GameObject iconStoryDoc;

    [Header("Window Prefabs")]
    [SerializeField] private GameObject terminalWindowPrefab;
    [SerializeField] private GameObject textWindowPrefab;

    private TerminalWindow _terminalWindow;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        RefreshIconVisibility();
        ProcessLevelReturn();
    }

    // ────────── 关卡入口 ──────────

    /// <summary>
    /// 点击核心监控 → 进入当前阶段对应的关卡。
    /// </summary>
    public void EnterLevel()
    {
        if (_coreMonitorLocked) return;

        var state = GameManager.Instance?.State;
        if (state == null) return;

        string levelName = state.CurrentPhase switch
        {
            1 => "Level1_Home",
            2 => "Level2_Empathy",
            3 => "Level3_Will",
            _ => ""
        };

        if (string.IsNullOrEmpty(levelName)) return;

        _coreMonitorLocked = true;

        // 关卡完成后由关卡代码调用 ReturnToDesktop 传回代码
        GameManager.Instance?.LoadScene(levelName);
    }

    // ────────── 关卡返回处理 ──────────

    private void ProcessLevelReturn()
    {
        var payload = GameManager.Instance?.PendingPayload;
        if (payload == null || !payload.success) return;

        // 消费 payload
        GameManager.Instance.PendingPayload = null;
        _pendingCode = payload.collectedCode;

        // 打开终端，自动显示收集到的代码
        OpenTerminal();
        if (_terminalWindow != null)
            _terminalWindow.DisplayCollectedCode(_pendingCode);
    }

    /// <summary>
    /// 核心监控解锁（对话结束后由 DialogueManager 调用）。
    /// </summary>
    public void UnlockCoreMonitor()
    {
        _coreMonitorLocked = false;
        RefreshIconVisibility();
    }

    // ────────── 图标 ──────────

    public void RefreshIconVisibility()
    {
        var state = GameManager.Instance?.State;
        if (state == null) return;

        // 核心监控：首次对话后出现；phase=4 后隐藏（没有更多关卡）
        if (iconCoreMonitor != null)
            iconCoreMonitor.SetActive(
                state.GetFlag("intro_done") && state.CurrentPhase < 4
            );

        // 故事文档：phase >= 3 出现
        if (iconStoryDoc != null)
            iconStoryDoc.SetActive(state.CurrentPhase >= 3);
    }

    // ────────── 窗口 ──────────

    public void OpenTerminal()
    {
        if (_terminalWindow == null && terminalWindowPrefab != null)
        {
            var go = Instantiate(terminalWindowPrefab, transform);
            _terminalWindow = go.GetComponent<TerminalWindow>();
        }

        if (_terminalWindow != null)
            _terminalWindow.Open();

        if (_terminalFirstOpen)
        {
            _terminalFirstOpen = false;
            var dm = FindObjectOfType<DialogueManager>();
            if (dm != null)
                dm.TriggerDialogues("Desktop");
        }
    }

    public void OpenTextViewer(string fileName, string content)
    {
        if (textWindowPrefab != null)
        {
            var go = Instantiate(textWindowPrefab, transform);
            var viewer = go.GetComponent<TextViewerWindow>();
            if (viewer != null)
                viewer.SetContent(fileName, content);
        }
    }
}
