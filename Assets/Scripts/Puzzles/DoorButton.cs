using UnityEngine;

public class DoorButton : MonoBehaviour
{
    public Door targetDoor;
    public Sprite pressedSprite;
    private SpriteRenderer spriteRenderer;
    private bool isPressed = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isPressed)
        {
            PressButton();
        }
    }

    void PressButton()
    {
        isPressed = true;
        spriteRenderer.sprite = pressedSprite;
        targetDoor.OpenDoor();
    }
}