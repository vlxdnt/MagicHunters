using UnityEngine;
using System.Collections;
using Unity.Netcode;

public class Health : NetworkBehaviour
{
    [Header("Life settings")]
    public int maxHealth = 100;
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Immunity Settings")]
    public float immunityDuration = 1f;
    private bool isImmune = false;

    private PlayerUI playerUI;
    private SpriteRenderer spriteRenderer;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            currentHealth.Value = maxHealth;
            currentHealth.OnValueChanged += UpdateUI;

            StartCoroutine(GasesteUIDelay());
        }
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    private IEnumerator GasesteUIDelay()
    {
        // astept UI ul
        while (playerUI == null)
        {
            playerUI = FindFirstObjectByType<PlayerUI>();
            yield return new WaitForSeconds(0.2f);
        }

        // updatee
        playerUI.ActualizeazaHP(currentHealth.Value, maxHealth);
    }

    // update la UI pt heal
    private void UpdateUI(int oldValue, int newValue)
    {
        if (playerUI != null) playerUI.ActualizeazaHP(newValue, maxHealth);
    }

    public void TakeDamage(int damageAmount)
    {
        if (!IsOwner || isImmune) return;

        currentHealth.Value -= damageAmount;

        if (currentHealth.Value <= 0) Die();
        else StartCoroutine(ImmunityRoutine());
    }

    private IEnumerator ImmunityRoutine()
    {
        isImmune = true;
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);
            yield return new WaitForSeconds(immunityDuration);
            spriteRenderer.color = originalColor;
        }
        else yield return new WaitForSeconds(immunityDuration);
        isImmune = false;
    }

    public void Heal(int healAmount)
    {
        if (!IsOwner) return;
        currentHealth.Value = Mathf.Min(currentHealth.Value + healAmount, maxHealth);
    }

    void Die()
    {
        Debug.Log(gameObject.name + " a muritt");
        StartCoroutine(RespawnCuFade());
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner) currentHealth.OnValueChanged -= UpdateUI;
    }

    private IEnumerator RespawnCuFade()
    {
        if (SceneFade.Instance != null)
            yield return StartCoroutine(SceneFade.Instance.FadeOut(0.3f));

        PlayerRespawn respawn = GetComponent<PlayerRespawn>();
        if (respawn != null) respawn.Respawn();

        currentHealth.Value = maxHealth;

        if (SceneFade.Instance != null)
            yield return StartCoroutine(SceneFade.Instance.FadeIn(0.3f));

        StartCoroutine(ImmunityRoutine());
    }
}