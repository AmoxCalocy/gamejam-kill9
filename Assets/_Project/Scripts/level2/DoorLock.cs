using UnityEngine;

public class DoorLock : MonoBehaviour
{
    [SerializeField] private PressureButton button1;
    [SerializeField] private PressureButton button2;
    [SerializeField] private Sprite openDoor;
    [SerializeField] private Sprite closedDoor;

    public bool open;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (open)
        {
            return;
        }

        if (button1.pressed && button2.pressed)
        {
            open = true;
            button1.LockPressed();
            button2.LockPressed();
            spriteRenderer.sprite = openDoor;
        }
    }
}