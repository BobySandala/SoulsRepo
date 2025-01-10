using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public Slider easeHealthSlider;
    public float maxHealth = 100f;
    public float health;
    private float lerpSpeed = 5f; // Lerp speed multiplier
    private float sliderLeftOffset = 275f;
    private Coroutine lerpCoroutine; // Reference to the running coroutine

    void Start()
    {
        health = maxHealth;
        SetSliderPosition(sliderLeftOffset);
        SetSliderSize();

        healthSlider.maxValue = maxHealth;
        easeHealthSlider.maxValue = maxHealth;

        healthSlider.value = maxHealth;
        easeHealthSlider.value = maxHealth;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(60);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            HealDamage(60);
        }

        // Smoothly update easeHealthSlider
        if (easeHealthSlider.value != healthSlider.value)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, healthSlider.value, Time.deltaTime * lerpSpeed);
        }
    }

    public void SetMaxHealth(float val)
    {
        maxHealth = val;
        health = maxHealth;
        SetSliderSize();
        SetSliderPosition(sliderLeftOffset);
    }

    public void TakeDamage(float damage)
    {
        health = Mathf.Max(health - damage, 0);
        UpdateHealthSlider();
    }

    public void HealDamage(float heal)
    {
        health = Mathf.Min(health + heal, maxHealth);
        UpdateHealthSlider();
    }

    private void UpdateHealthSlider()
    {
        // Stop any running coroutine before starting a new one
        if (lerpCoroutine != null)
        {
            StopCoroutine(lerpCoroutine);
        }

        // Start a new coroutine
        lerpCoroutine = StartCoroutine(LerpHealth(health));
    }

    private IEnumerator LerpHealth(float targetHealth)
    {
        while (Mathf.Abs(healthSlider.value - targetHealth) > 0.01f)
        {
            healthSlider.value = Mathf.Lerp(healthSlider.value, targetHealth, Time.deltaTime * lerpSpeed);
            yield return null;
        }

        healthSlider.value = targetHealth; // Snap to the exact target value
        lerpCoroutine = null; // Clear the coroutine reference
    }

    private void SetSliderSize()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.sizeDelta = new Vector2(maxHealth, rectTransform.sizeDelta.y);
        }
    }

    private void SetSliderPosition(float leftOffset)
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            float width = maxHealth;
            rectTransform.sizeDelta = new Vector2(width, rectTransform.sizeDelta.y);
            rectTransform.anchoredPosition = new Vector2(leftOffset + (width / 2), rectTransform.anchoredPosition.y);
            rectTransform.anchorMin = new Vector2(0, rectTransform.anchorMin.y);
            rectTransform.anchorMax = new Vector2(0, rectTransform.anchorMax.y);
            rectTransform.pivot = new Vector2(0.5f, rectTransform.pivot.y);
        }
    }
}
