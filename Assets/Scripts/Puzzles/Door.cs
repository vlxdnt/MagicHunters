using UnityEngine;

public class Door : MonoBehaviour
{
    public Sprite openSprite;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D doorCollider;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        doorCollider = GetComponent<BoxCollider2D>();
    }

    public void OpenDoor()
    {
        spriteRenderer.sprite = openSprite;
        doorCollider.enabled = false;
        Debug.Log("door open");
    }
}