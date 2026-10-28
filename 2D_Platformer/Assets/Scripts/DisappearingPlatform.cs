using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Plateforme qui disparaît quand un player reste dessus trop longtemps.
// Elle clignote avant de disparaître, puis réapparaît après un délai (optionnel).
public class DisappearingPlatform : MonoBehaviour
{
    [Header("Disparition")]
    [Tooltip("Temps (en secondes) qu'un player peut rester sur la plateforme avant qu'elle disparaisse.")]
    [SerializeField] private float timeBeforeDisappear = 1.5f;

    [Tooltip("Si coché, le compteur repart à zéro quand le player quitte la plateforme. Sinon, le temps s'additionne.")]
    [SerializeField] private bool resetTimerWhenLeaving = true;

    [Header("Réapparition")]
    [SerializeField] private bool respawn = true;
    [SerializeField] private float respawnDelay = 3f;

    [Header("Feedback visuel")]
    [Tooltip("Pourcentage du temps (0-1) à partir duquel la plateforme commence à clignoter.")]
    [Range(0f, 1f)] [SerializeField] private float blinkStart = 0.5f;
    [SerializeField] private float blinkSpeed = 15f;
    [Range(0f, 1f)] [SerializeField] private float blinkMinAlpha = 0.3f;

    private readonly HashSet<CharacterController2D> playersTouching = new();
    private Collider2D[] colliders;
    private SpriteRenderer[] renderers;
    private Color[] originalColors;
    private float timer;
    private bool hidden;

    private void Awake()
    {
        colliders = GetComponents<Collider2D>();
        renderers = GetComponentsInChildren<SpriteRenderer>();
        originalColors = new Color[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
            originalColors[i] = renderers[i].color;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CharacterController2D player = collision.gameObject.GetComponent<CharacterController2D>();
        if (player)
            playersTouching.Add(player);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        CharacterController2D player = collision.gameObject.GetComponent<CharacterController2D>();
        if (player)
            playersTouching.Remove(player);
    }

    private void Update()
    {
        if (hidden)
            return;

        if (IsPlayerStandingOnTop())
        {
            timer += Time.deltaTime;
            if (timer >= timeBeforeDisappear)
            {
                StartCoroutine(Disappear());
                return;
            }
        }
        else if (resetTimerWhenLeaving)
        {
            timer = 0f;
        }

        UpdateBlink();
    }

    // Vrai seulement si un player est posé au-dessus (pas s'il touche le côté ou le dessous)
    private bool IsPlayerStandingOnTop()
    {
        playersTouching.RemoveWhere(p => p == null);
        if (colliders.Length == 0)
            return false;

        float platformTop = colliders[0].bounds.max.y;
        foreach (Collider2D c in colliders)
            platformTop = Mathf.Max(platformTop, c.bounds.max.y);

        foreach (CharacterController2D player in playersTouching)
        {
            Collider2D playerCollider = player.GetComponent<Collider2D>();
            if (playerCollider && playerCollider.bounds.min.y >= platformTop - 0.1f)
                return true;
        }
        return false;
    }

    private void UpdateBlink()
    {
        float progress = timeBeforeDisappear > 0f ? timer / timeBeforeDisappear : 1f;
        float alpha = 1f;
        if (progress >= blinkStart && timer > 0f)
            alpha = Mathf.Lerp(blinkMinAlpha, 1f, (Mathf.Sin(Time.time * blinkSpeed) + 1f) * 0.5f);
        SetAlpha(alpha);
    }

    private void SetAlpha(float alpha)
    {
        for (int i = 0; i < renderers.Length; i++)
        {
            Color c = originalColors[i];
            c.a *= alpha;
            renderers[i].color = c;
        }
    }

    private IEnumerator Disappear()
    {
        hidden = true;
        timer = 0f;
        playersTouching.Clear();
        SetVisible(false);

        if (!respawn)
            yield break;

        yield return new WaitForSeconds(respawnDelay);

        SetVisible(true);
        SetAlpha(1f);
        hidden = false;
    }

    private void SetVisible(bool visible)
    {
        foreach (Collider2D c in colliders)
            c.enabled = visible;
        foreach (SpriteRenderer r in renderers)
            r.enabled = visible;
    }
}
