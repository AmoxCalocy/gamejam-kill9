using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 全局游戏管理器 — 单例，跨场景持久化。
/// 管理流程状态、关卡切换、数据传递。
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState State { get; private set; } = new GameState();

    /// <summary>
    /// 场景间传递数据用的临时载体（进入关卡时设置目标代码，
    /// 返回 Desktop 时关卡结果写入此处）。
    /// </summary>
    public ScenePayload PendingPayload { get; set; }

    [Header("Transition")]
    [SerializeField] private float fadeDuration = 0.5f;

    [SerializeField] private SceneLoader sceneLoader;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 请求加载场景。参数 payload 携带目标代码等数据。
    /// </summary>
    public void LoadScene(string sceneName, ScenePayload payload = null)
    {
        PendingPayload = payload;
        State.currentScene = sceneName;

        if (sceneLoader != null)
            sceneLoader.LoadScene(sceneName, fadeDuration);
        else
            SceneManager.LoadSceneAsync(sceneName);
    }

    /// <summary>
    /// 返回 Desktop 场景，携带可选的 payload。
    /// </summary>
    public void ReturnToDesktop(ScenePayload payload = null)
    {
        PendingPayload = payload;
        var desktopScene = SceneManager.GetActiveScene().name == "Endings"
            ? "Desktop"
            : "Desktop";
        // 如果当前已是结局场景，不重复跳转
        if (SceneManager.GetActiveScene().name == "Endings")
        {
            LoadScene("Endings", payload);
            return;
        }
        LoadScene(desktopScene, payload);
    }

    /// <summary>
    /// 触发结局。
    /// </summary>
    public void TriggerEnding(EndingType ending)
    {
        State.endingType = ending;
        LoadScene("Endings");
    }
}
