using UnityEngine;

/// <summary>
/// CRT 后处理效果 — 挂载到主摄像机上。
/// 使用 OnRenderImage 应用 CRT Shader。
/// </summary>
[RequireComponent(typeof(Camera))]
public class CRTEffect : MonoBehaviour
{
    [SerializeField] private Material crtMaterial;

    private void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (crtMaterial != null)
            Graphics.Blit(src, dest, crtMaterial);
        else
            Graphics.Blit(src, dest);
    }
}
