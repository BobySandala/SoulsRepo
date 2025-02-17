using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OverlayControl : MonoBehaviour
{
    public PlayerData playerObject;

    public GameObject ingameOverlay;
    public GameObject restingOverlay;
    public GameObject bossOverlay;
    public GameObject gameOverOverlay;
    public HealthBar bossHPBar;

    public AudioSelector audioSelector;

    public EnemyController enemyController;
    public EnemyHealth enemyHealth;
    public Image restImage; // Reference to the Image in the IngameOverlay
    public bool isBossFight = false;

    public Image estusImg;

    public Sprite[] estusImages; 

    public void SetEstusImage(int index)
    {
        if (index < 0 || index > 10) return;
        estusImg.sprite = estusImages[index];
    }

    public void ContinueBtnPressed()
    {
        playerObject.ChangePlayerState(0);
        //Debug.Log("ContinuePressed");
        
    }

    public void ExitButtonPressed()
    {
        // Get the current active scene index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Increment the index to load the next scene
        int nextSceneIndex = 0;

        // Ensure the next scene index is valid (there are no scenes after the last one)
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            // Load the next scene
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            //Debug.LogWarning("No next scene to load!");
        }
    }
    public void TravelBtnPressed()
    {
        // Get the current active scene index
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // Increment the index to load the next scene
        int nextSceneIndex = currentSceneIndex + 1;
        int prevSceneIndex = currentSceneIndex - 1;

        // Ensure the next scene index is valid (there are no scenes after the last one)
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            // Load the next scene
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            if (prevSceneIndex > 0)
            {
                // Load the next scene
                SceneManager.LoadScene(prevSceneIndex);
            }
            else
            {
                //Debug.Log("no valid Scene");
            }
        }
    }

    public void ShowIngameOverlay()
    {
        // Show in-game overlay and hide resting overlay
        if (ingameOverlay != null)
        {
            ingameOverlay.SetActive(true);
        }
        if (restingOverlay != null)
        {
            restingOverlay.SetActive(false);
        }
    }

    public void ShowRestingOverlay()
    {
        // Show resting overlay (pause menu) and hide in-game overlay
        if (ingameOverlay != null)
        {
            ingameOverlay.SetActive(false);
        }
        if (restingOverlay != null)
        {
            restingOverlay.SetActive(true);
        }
    }

    void Start()
    {
        if (playerObject != null)
        {
            if (playerObject.state == 0)
            {
                //gaming state
                ingameOverlay.SetActive(true);
                restingOverlay.SetActive(false);
            }
            else if (playerObject.state == 1)
            {
                //resting state
                ingameOverlay.SetActive(false);
                restingOverlay.SetActive(true);
            }
        }
        if (enemyController != null)
        {
            bossHPBar.SetMaxHealth(enemyHealth.maxHealth);
        }
        if (bossOverlay != null)
        {
            bossOverlay.SetActive(false);
        }
        if (gameOverOverlay != null)
        {
            gameOverOverlay.SetActive(false);
        }
        estusImg.sprite = estusImages[10];
    }

    public void YouDied()
    {
        //Debug.Log("died");
        bossOverlay.SetActive(false);
        ingameOverlay.SetActive(false);
        restingOverlay.SetActive(false);
        gameOverOverlay.SetActive(true);
        audioSelector.YouDiedSound();
    }

    public void ShowRestAtBonfire(bool val)
    {
        restImage.gameObject.SetActive(val); // Set image active if canRest is true
    }

    public void ChangePlayerState()
    {
        if (playerObject != null)
        {
            if (playerObject.state == 0)
            {
                //gaming state
                ingameOverlay.SetActive(true);
                restingOverlay.SetActive(false);
            }
            else if (playerObject.state == 1)
            {
                //resting state
                ingameOverlay.SetActive(false);
                restingOverlay.SetActive(true);
            }
        }
    }

    private void Update()
    {
        if (enemyController != null)
        {
            if (enemyController.isActive)
            {
                bossOverlay.SetActive(true);
                //suntem in bossfight
                if (!isBossFight)
                {
                    audioSelector.toggleBossFightSound();
                }
                isBossFight = true;

                float bossHP = enemyHealth.currentHealth;
                bossHPBar.SetHealth(bossHP);
            }
        }
    }

    public void RestartBtn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
