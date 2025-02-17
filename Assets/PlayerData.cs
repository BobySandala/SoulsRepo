using System.Collections.Generic;
using UnityEngine;
using static Unity.VisualScripting.Member;

public class PlayerData : MonoBehaviour
{
    public bool canRest = false;

    public int health = 400;
    public int maxHealth = 400;

    public int maxStamina = 300;
    public int stamina = 300;

    [Header("HUD References")]
    public HealthBar healthBar; // Reference to the HealthBar script
    public HealthBar staminaBar; // Reference to the HealthBar script
    public OverlayControl overlayControl; // Manages UI overlays

    public AudioSource restAtBonfire;

    // Stamina costs (doubled)
    public int rollStaminaCost = 20;            // Original: 25
    public int lightAttackStaminaCost = 15;    // Original: 10
    public int heavyAttackStaminaCost = 30;    // Original: 20
    public int jumpAttackStaminaCost = 60;     // Original: 30
    public int jumpStaminaCost = 10;           // Original: 25


    // Existing variables for stamina management
    private float staminaRegenTimer = 0f;
    private float lastStaminaConsumeTime = 0f;
    private float staminaRegenDelay = 1.5f; // Delay before stamina starts regenerating
    private float staminaRegenRate = 1f; // Stamina regenerates every second
    private int staminaRegenAmount;
    private bool isDead = false;

    private bool isInvulnerable = false;

    public Animator animator;             // Reference to the Animator component

    public List<AudioClip> dmgSounds = new List<AudioClip>();
    public AudioClip nomnomnom;

    // 0 - Game state, 1 - Resting state
    public int state = 0;

    private void Start()
    {

        health = maxHealth;
        healthBar.health = Mathf.Clamp(health, 0, maxHealth);
        stamina = maxStamina; // Ensure stamina starts within valid range
        staminaBar.health = Mathf.Clamp(stamina, 0, maxStamina); // Ensure stamina starts within valid range

        staminaRegenAmount = maxStamina / 4;

        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
            healthBar.SetHealth(health);
        }

        if (staminaBar != null)
        {
            staminaBar.SetMaxHealth(maxStamina);
            staminaBar.SetHealth(stamina);
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //Debug.Log($"Initial Stamina: {stamina}"); // Debug log to verify starting stamina

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }


    private void Update()
    {
        // Check if enough time has passed since last stamina consumption
        if (Time.time - lastStaminaConsumeTime >= staminaRegenDelay)
        {
            // Stamina regeneration logic
            staminaRegenTimer += Time.deltaTime;
            if (staminaRegenTimer >= staminaRegenRate)
            {
                RegenStamina(staminaRegenAmount); // Increase stamina faster
                staminaRegenTimer = 0f;
            }
        }

        // Toggle resting state when pressing "E" near a rest point
        if (Input.GetKeyUp(KeyCode.E) && canRest)
        {
            ChangePlayerState(-1);
        }
    }

    public void ConsumeStamina(int amount)
    {
        if (stamina >= amount)
        {
            stamina = Mathf.Clamp(stamina - amount, 0, maxStamina);
            //Debug.Log($"Player consumed stamina. Current stamina: {stamina}");

            if (staminaBar != null)
            {
                staminaBar.SetHealth(stamina);
            }

            staminaRegenTimer = 0f;
            lastStaminaConsumeTime = Time.time;
        }
        else
        {
            //Debug.Log("Not enough stamina to perform the action.");
        }
    }

    public void SetCanRest(bool canRest_)
    {
        // Enable or disable the option to rest based on proximity to a rest point
        canRest = canRest_;
        if (overlayControl != null)
        {
            overlayControl.ShowRestAtBonfire(canRest);
        }
    }

    public void ChangePlayerState(int st)
    {
        // Toggle between game and resting states
        if (st == -1)
        {
            state = (state + 1) % 2; // Cycle between states
        }
        else
        {
            state = st; // Explicitly set the state
        }
        if (state == 0)
        {
            animator.SetBool("Sit",false);
        }
        else if (state == 1)
        {
            animator.SetBool("Sit",true);
        }

        //Debug.Log("Player state changed: " + state);

        // Update the overlay and perform state-specific logic
        if (overlayControl != null)
        {
            overlayControl.ChangePlayerState();
        }

        StateLogic();
    }

    private void StateLogic()
    {
        if (state == 0)
        {
            // Game state
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (overlayControl != null)
            {
                overlayControl.ShowIngameOverlay();
            }
        }
        else if (state == 1)
        {
            // Resting state
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (overlayControl != null)
            {
                overlayControl.ShowRestingOverlay();
            }

            if (restAtBonfire != null)
            {
                restAtBonfire.Play();
            }
        }
    }


    public void RegenStamina(int amount)
    {
        // Increase stamina, clamping it within valid limits
        stamina = Mathf.Clamp(stamina + amount, 0, maxStamina);
        //Debug.Log($"Player regained stamina. Current stamina: {stamina}");

        if (staminaBar != null)
        {
            staminaBar.SetHealth(stamina);
        }
    }


    public void TakeDamage(int amount)
    {
        if (isDead)
        {
            //Debug.Log("Player is already dead. No damage applied.");
            return;
        }
        if (isInvulnerable)
        {
            //Debug.Log("Player is invulnerable. No damage applied.");
            return;
        }
        bool isBlocking = GetComponent<player_Movement>().isBlocking;
        if (isBlocking)
        {
            amount = amount / 2;
        }
        Debug.Log("bosul a dat damage");
        Debug.Log(amount);

        health = Mathf.Clamp(health - amount, 0, maxHealth);
        //Debug.Log($"Player took damage. Current health: {health}");

        if (healthBar != null)
        {
            healthBar.SetHealth(health);
            AudioClip clip = dmgSounds[Random.Range(0, dmgSounds.Count)];
            GetComponent<AudioSource>().PlayOneShot(clip);
        }

        if (health <= 0)
        {
            isDead = true;
            //Debug.Log("Player has died.");
            Animator playerAnimator = GetComponent<Animator>();
            if (playerAnimator != null)
            {
                playerAnimator.SetBool("Die", true);
            }
            overlayControl.YouDied();
            // Other death logic here
        }
    }




    public void HealDamage(int amount)
    {
        health = Mathf.Clamp(health + amount, 0, maxHealth);
        //Debug.Log($"Player healed by {amount}. Current health: {health}");

        if (healthBar != null)
        {
            healthBar.SetHealth(health);
            GetComponent<AudioSource>().PlayOneShot(nomnomnom);
        }
    }

    public void SetInvulnerability(bool value)
    {
        isInvulnerable = value;
        //Debug.Log("Player invulnerability: " + (value ? "Enabled" : "Disabled"));
    }


}
