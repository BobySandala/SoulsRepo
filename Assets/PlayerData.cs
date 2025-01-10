using UnityEngine;
using UnityEngine.XR;

public class PlayerData : MonoBehaviour
{
    public bool canRest = false;

    public int health = 100;
    public int maxHealth = 100;

    public int maxStamina = 100;
    public int stamina = 100;

    [Header("HUD References")]
    public HealthBar healthBar; // Reference to the HealthBar script
    public HealthBar staminaBar; // Reference to the HealthBar script
    public OverlayControl overlayControl; // Manages UI overlays

    public AudioSource restAtBonfire;

    // 0 - Game state, 1 - Resting state
    public int state = 0;

    private void Start()
    {
        // Ensure health does not exceed maxHealth
        health = Mathf.Clamp(health, 0, maxHealth);
        stamina = Mathf.Clamp(stamina, 0, maxStamina);

        healthBar.SetMaxHealth(maxHealth);
        staminaBar.SetMaxHealth(maxStamina);

        Cursor.lockState = CursorLockMode.Locked;  // Lock the cursor when the game starts
        Cursor.visible = false;  // Hide the cursor when the game starts
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.E) && canRest)
        {
            ChangePlayerState(-1);
        }
    }

    public void SetCanRest(bool canRest_)
    {
        canRest = canRest_;
        overlayControl.ShowRestAtBonfire(canRest);
    }

    public void ChangePlayerState(int st)
    {
        // Toggle between game and resting states
        if (st == -1)
        {
            state = (state + 1) % 2;
        } else
        {
            state = st;
        }
        Debug.Log("Player state changed: " + state);

        // Trigger the UI update based on the state
        overlayControl.ChangePlayerState();

        // Handle logic specific to the state
        StateLogic();
    }

    // Handle state-specific logic, like cursor behavior and UI updates
    public void StateLogic()
    {
        if (state == 0)
        {
            // Game state
            Cursor.lockState = CursorLockMode.Locked;  // Lock the cursor to the game window
            Cursor.visible = false;  // Hide the cursor to prevent interference with the game

            // Ensure the correct overlay is shown (In-game UI)
            if (overlayControl != null)
            {
                overlayControl.ShowIngameOverlay();
            }
        }
        else if (state == 1)
        {
            // Resting state
            Cursor.lockState = CursorLockMode.None;  // Unlock the cursor for UI interaction
            Cursor.visible = true;  // Make the cursor visible for interaction with UI

            // Ensure the correct overlay is shown (Resting UI, e.g., Pause Menu)
            if (overlayControl != null)
            {
                overlayControl.ShowRestingOverlay();
            }
            restAtBonfire.Play();
        }
    }

    // Method to consume stamina
    public void ConsumeStamina(int amount)
    {
        stamina = Mathf.Clamp(stamina - amount, 0, maxStamina);
        Debug.Log($"Player consumed stamina. Current stamina: {stamina}");

        // Update the stamina bar UI
        if (staminaBar != null)
        {
            staminaBar.TakeDamage(amount);
        }
    }

    // Method to regenerate stamina
    public void RegenStamina(int amount)
    {
        stamina = Mathf.Clamp(stamina + amount, 0, maxStamina);
        Debug.Log($"Player regained stamina. Current stamina: {stamina}");

        // Update the stamina bar UI
        if (staminaBar != null)
        {
            staminaBar.HealDamage(amount);
        }
    }

    // Method to deal damage to the player
    public void TakeDamage(int amount)
    {
        health = Mathf.Clamp(health - amount, 0, maxHealth);
        Debug.Log($"Player took damage. Current health: {health}");

        // Update the health bar UI
        if (healthBar != null)
        {
            healthBar.TakeDamage(amount);
        }
    }

    // Method to heal the player
    public void HealDamage(int amount)
    {
        health = Mathf.Clamp(health + amount, 0, maxHealth);
        Debug.Log($"Player healed. Current health: {health}");

        // Update the health bar UI
        if (healthBar != null)
        {
            healthBar.HealDamage(amount);
        }
    }
}
