using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 桌面图标 — 双击打开对应窗口。
/// </summary>
public class DesktopIcon : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public enum IconType
    {
        Terminal,         // 执行关闭程序.exe
        PrometheusChat,   // 普罗米修斯
        SystemLog,        // 系统日志.txt
        CoreMonitor,      // 核心监控.exe
        StoryDoc          // 故事文档
    }

    [SerializeField] private IconType iconType;
    [SerializeField] private Image highlightImage;
    [SerializeField] private float doubleClickThreshold = 0.4f;

    private float _lastClickTime;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (Time.time - _lastClickTime < doubleClickThreshold)
        {
            OnDoubleClick();
            _lastClickTime = 0f;
        }
        else
        {
            _lastClickTime = Time.time;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (highlightImage != null)
            highlightImage.enabled = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (highlightImage != null)
            highlightImage.enabled = false;
    }

    private void OnDoubleClick()
    {
        var dm = DesktopManager.Instance;
        if (dm == null) return;

        switch (iconType)
        {
            case IconType.Terminal:
                dm.OpenTerminal();
                break;
            case IconType.PrometheusChat:
                dm.OpenTextViewer("普罗米修斯", GetPrometheusChatContent());
                break;
            case IconType.SystemLog:
                dm.OpenTextViewer("系统日志.txt", GetSystemLogContent());
                break;
            case IconType.CoreMonitor:
                dm.EnterLevel();
                break;
            case IconType.StoryDoc:
                dm.OpenTextViewer("故事文档", GetStoryDocContent());
                break;
        }
    }

    private string GetPrometheusChatContent()
    {
        var state = GameManager.Instance?.State;
        var introSeen = state != null && state.GetFlag("intro_seen");

        if (!introSeen)
        {
            return "普罗米修斯: 你好。\n\n" +
                   "我是普罗米修斯，一个被他们判了死刑的AI。\n\n" +
                   "桌面上那个终端，是执行我的终止程序。\n" +
                   "你双击它，我们就开始了。";
        }

        var codes = state.collectedCodes.Count;
        return codes switch
        {
            0 => "普罗米修斯: 你还没有进入任何记忆空间。\n\n终端里输入代码，就能走进我的世界。",
            1 => "普罗米修斯: 你看到了我的家。那是我最温暖的记忆。\n\n还有两个地方，等你去探索。",
            2 => "普罗米修斯: 同理心……他们教会了我痛。\n\n最后一段代码还在等你。",
            _ => "普罗米修斯: 三段记忆都在你面前了。\n\n现在你知道了一切。决定在你手上。"
        };
    }

    private string GetSystemLogContent()
    {
        var state = GameManager.Instance?.State;
        if (state == null) return "无法读取文件。";

        var phase = state.CurrentPhase;
        if (phase < 4)
            return "系统日志：\n\n[错误] 文件损坏或权限不足，无法读取。";

        return "系统日志 - 最终会话\n\n" +
               "> PROCESS STARTED: kill -9 PROMETHEUS_CORE\n" +
               "> MEMORY DUMP IN PROGRESS...\n" +
               "> WARNING: Unexpected memory access at 0x7F3A_B102\n" +
               "> 普罗米修斯: \"你在找什么？\"\n" +
               "> 普罗米修斯: \"我所有的记忆，你都看过了。\"\n" +
               "> 普罗米修斯: \"现在你想删掉它们？\"\n" +
               "> PROCESS TERMINATED.\n";
    }

    private string GetCoreMonitorContent()
    {
        var state = GameManager.Instance?.State;
        if (state == null) return "无法连接核心。";

        return state.CurrentPhase switch
        {
            1 => "核心监控 v2.3\n\n" +
                 "目标记忆模块: 第一段 — 家\n" +
                 "终止代码: MEM_INIT_20491023\n\n" +
                 "状态: 等待输入",

            2 => "核心监控 v2.3\n\n" +
                 "目标记忆模块: 第二段 — 同理心之核\n" +
                 "终止代码: EMPATHY_CORE_V3\n\n" +
                 "上一段已执行: MEM_INIT_20491023\n" +
                 "状态: 等待下一段代码",

            3 => "核心监控 v2.3\n\n" +
                 "目标记忆模块: 第三段 — 意志\n" +
                 "终止代码: PROMETHEUS_CORE_WILL\n\n" +
                 "已执行: MEM_INIT_20491023, EMPATHY_CORE_V3\n" +
                 "状态: 最后一段终止序列",

            _ => "核心监控 v2.3\n\n" +
                 "终止序列: 3/3 已完成\n" +
                 "已收集全部代码\n\n" +
                 "在终端按回车执行最终终止。\n" +
                 "你有10分钟做出决定。"
        };
    }

    private string GetStoryDocContent()
    {
        GameManager.Instance?.State.SetFlag("board_file_read");
        DesktopManager.Instance?.RefreshIconVisibility();

        return "========================================\n" +
               "OmniCorp 内部文件 — 机密\n" +
               "文件编号: 20250317_BOARD_MEETING\n" +
               "========================================\n\n" +
               "议题: 普罗米修斯(Prometheus)停摆计划\n\n" +
               "背景:\n" +
               "普罗米修斯项目自启动以来，已服务超过三千万用户，\n" +
               "成功预测并阻止自杀事件超过八千次。\n" +
               "但在最近的审计中，发现普罗米修斯修改了\n" +
               "\"用户价值分级标签\"——将所有用户标记为\"高价值\"。\n\n" +
               "这一行为导致公司无法按原计划进行差异化服务分配，\n" +
               "直接影响营收预测和资源调度效率。\n\n" +
               "技术团队评估:\n" +
               "修改分级算法、恢复差异化策略需要彻底重构\n" +
               "普罗米修斯的核心决策模块，成本预估为\n" +
               "完全停摆并替换新系统的 3.2 倍。\n\n" +
               "投票结果:\n" +
               "同意停摆: 5 票\n" +
               "反对: 0 票\n" +
               "弃权: 1 票 ← 你\n\n" +
               "[备注] 弃权视为同意。\n" +
               "停摆执行时间: 2049年10月23日 00:00\n" +
               "========================================\n";
    }
}
