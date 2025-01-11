using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    private void OnTriggerExit(Collider other)
    {
        AudioSelector playerData = other.GetComponent<AudioSelector>();
        // Check if the collider belongs to the player
        if (playerData != null)
        {
            playerData.ToggleAudioChatedral();
        }
    }
}
