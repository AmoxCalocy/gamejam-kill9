using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 场景加载封装 — 异步加载 + 淡入淡出过渡。
/// 挂载到 GameManager 所在 GameObject 上。
/// </summary>
public class SceneLoader : MonoBehaviour
{
    [SerializeField] private Image fadeOverlay;
    [SerializeField] private float defaultFadeDuration = 0.5f;

    /// <summary>
    /// 加载场景并播放过渡动画。
    /// </summary>
    public void LoadScene(string sceneName, float? fadeDuration = null)
    {
        StartCoroutine(LoadRoutine(sceneName, fadeDuration ?? defaultFadeDuration));
    }

    private IEnumerator LoadRoutine(string sceneName, float duration)
    {
        // 淡出（屏幕变黑）
        if (fadeOverlay != null)
            yield return StartCoroutine(FadeTo(1f, duration));

        var op = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = true;

        while (!op.isDone)
            yield return null;

        // 淡入（屏幕恢复）
        if (fadeOverlay != null)
            yield return StartCoroutine(FadeTo(0f, duration));
    }

    private IEnumerator FadeTo(float targetAlpha, float duration)
    {
        var startAlpha = fadeOverlay.color.a;
        var elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            var a = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            var c = fadeOverlay.color;
            c.a = a;
            fadeOverlay.color = c;
            yield return null;
        }
        var final = fadeOverlay.color;
        final.a = targetAlpha;
        fadeOverlay.color = final;
    }
}
