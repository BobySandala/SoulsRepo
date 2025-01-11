using System.Collections; // This is required for IEnumerator and coroutines
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider; // The main health slider (red bar)
    public Slider easeHealthSlider; // The ease slider (yellow bar)
    public float maxHealth = 400f;
    public float health;
    private float lerpSpeed = 5f; // Lerp speed multiplier for the yellow bar
    private float sliderLeftOffset = 275f;
    private Coroutine lerpCoroutine; // Reference to the running coroutine

    void Start()
    {
        // Initialize health to maximum
        health = maxHealth;

        // Set both sliders to reflect full health
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;

        easeHealthSlider.maxValue = maxHealth;
        easeHealthSlider.value = maxHealth;
    }


    void Update()
    {

    }

    public void SetMaxHealth(float maxHealthValue)
    {
        maxHealth = maxHealthValue;
        health = maxHealth;

        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;

        easeHealthSlider.maxValue = maxHealth;
        easeHealthSlider.value = maxHealth;
    }

    public void SetHealth(float currentHealth)
    {
        // Update the red bar (current health) immediately
        healthSlider.value = Mathf.Clamp(currentHealth, 0, maxHealth);

        // Smoothly update the yellow bar (easeHealthSlider) in a coroutine
        if (lerpCoroutine != null)
        {
            StopCoroutine(lerpCoroutine);
        }
        lerpCoroutine = StartCoroutine(LerpEaseHealth());
    }


    public void TakeDamage(float damage)
    {
        // Reduce health and update the sliders
        health = Mathf.Max(health - damage, 0);
        SetHealth(health);
    }

    public void HealDamage(float heal)
    {
        // Increase health and update the sliders
        health = Mathf.Min(health + heal, maxHealth);
        SetHealth(health);
    }

    private IEnumerator LerpEaseHealth()
    {
        // Smoothly move the yellow bar (easeHealthSlider) to match the red bar (healthSlider)
        while (Mathf.Abs(easeHealthSlider.value - healthSlider.value) > 0.01f)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, healthSlider.value, Time.deltaTime * lerpSpeed);
            yield return null;
        }

        // Snap the yellow bar to match the red bar exactly
        easeHealthSlider.value = healthSlider.value;
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
        }
    }
}
