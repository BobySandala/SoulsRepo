using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZone : MonoBehaviour
{

    public int healthChange = -10;
    public int scoreChange = 10;
    // Function called when the player enters the trigger zone
    public void OnPlayerEnter()
    {
        Debug.Log("Player entered the trigger zone!");
        // Add your custom logic here
    }

    // Function called when the player exits the trigger zone
    public void OnPlayerExit()
    {
        Debug.Log("Player exited the trigger zone!");
        // Add your custom logic here
    }

    // Called when another collider enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider belongs to a GameObject with PlayerData
        PlayerData playerData = other.GetComponent<PlayerData>();
        if (playerData != null)
        {
            playerData.SetCanRest(true);
        }
    }

    // Called when another collider exits the trigger
    private void OnTriggerExit(Collider other)
    {
        PlayerData playerData = other.GetComponent<PlayerData>();
        // Check if the collider belongs to the player
        if (playerData != null)
        {
            playerData.SetCanRest(false);
        }
    }
}
