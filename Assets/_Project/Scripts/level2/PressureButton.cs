using UnityEngine;

public class PressureButton : MonoBehaviour
{
    public bool pressed;
    public bool canPress = true;
    public Sprite pressedSprite;
    public Sprite unpressedSprite;

    private int presserCount;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!canPress) return;

        presserCount++;
        UpdatePressedState();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if(!canPress) return;

        presserCount = Mathf.Max(0, presserCount - 1);
        UpdatePressedState();
    }

    private void UpdatePressedState()
    {
        pressed = presserCount > 0;

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = pressed ? pressedSprite : unpressedSprite;
        }
    }

    public void LockPressed()
    {
        canPress = false;
        pressed = true;

        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = pressedSprite;
        }
    }
}
