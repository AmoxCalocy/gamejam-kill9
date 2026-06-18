using System.Collections.Generic;

/// <summary>
/// 全局游戏状态数据，由 GameManager 持有，跨场景持久化。
/// </summary>
[System.Serializable]
public class GameState
{
    public int enterCount;
    public List<string> collectedCodes = new List<string>();
    public EndingType endingType = EndingType.None;
    public bool boardFileRevealed;
    public string currentScene;
    public Dictionary<string, bool> flags = new Dictionary<string, bool>();

    /// <summary>
    /// 当前流程阶段，根据收集代码数量推断。
    /// </summary>
    public int CurrentPhase
    {
        get
        {
            if (collectedCodes.Count >= 3) return 4;
            if (collectedCodes.Count >= 2) return 3;
            if (collectedCodes.Count >= 1) return 2;
            return 1;
        }
    }

    public void AddCode(string code)
    {
        if (!collectedCodes.Contains(code))
            collectedCodes.Add(code);
    }

    public void SetFlag(string key, bool value = true)
    {
        flags[key] = value;
    }

    public bool GetFlag(string key)
    {
        return flags.TryGetValue(key, out bool v) && v;
    }

    public void Reset()
    {
        enterCount = 0;
        collectedCodes.Clear();
        endingType = EndingType.None;
        boardFileRevealed = false;
        currentScene = null;
        flags.Clear();
    }
}

public enum EndingType
{
    None,
    A_Deceived, // 被骗结局：正常三次回车通关
    B_Kill9,    // 主动输入 kill -9
    C_WhoAmI    // 发现自己是普罗米修斯
}
