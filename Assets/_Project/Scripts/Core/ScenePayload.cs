/// <summary>
/// 场景间传递数据的载体。
/// Desktop → 关卡：targetCode 为要寻找的代码。
/// 关卡 → Desktop：collectedCode 为获取到的代码字符串，success 表示是否成功。
/// </summary>
public class ScenePayload
{
    /// <summary>目标代码（Desktop → 关卡）</summary>
    public string targetCode;

    /// <summary>是否成功获取（关卡 → Desktop）</summary>
    public bool success;

    /// <summary>获取到的代码字符串（关卡 → Desktop）</summary>
    public string collectedCode;

    /// <summary>扩展参数字典</summary>
    public System.Collections.Generic.Dictionary<string, object> extra;

    public ScenePayload() { }

    public ScenePayload(string targetCode)
    {
        this.targetCode = targetCode;
    }

    public T GetExtra<T>(string key, T defaultValue = default)
    {
        if (extra == null || !extra.TryGetValue(key, out object val))
            return defaultValue;
        return val is T t ? t : defaultValue;
    }

    public void SetExtra(string key, object value)
    {
        extra ??= new System.Collections.Generic.Dictionary<string, object>();
        extra[key] = value;
    }
}
