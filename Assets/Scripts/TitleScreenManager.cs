using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using JetBrains.Annotations;
using UnityEngine.SceneManagement;

namespace DS_ripoff
{
    public class TitleScreenManager : MonoBehaviour
    {
        public void StartNetworkAsHost()
        {
            StartCoroutine(DelayedStartNetworkAsHost());
        }

        public void StartPlayerSelect()
        {
            StartCoroutine(DelayedStartPlayerSelect());
        }

        private IEnumerator DelayedStartNetworkAsHost()
        {
            // Wait for 1 second
            yield return new WaitForSeconds(1);

            // NetworkManager.Singleton.StartHost();
        }

        private IEnumerator DelayedStartPlayerSelect()
        {
            // Wait for 1 second
            yield return new WaitForSeconds(2);

            // Get the current active scene index
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            // Increment the index to load the next scene
            int nextSceneIndex = currentSceneIndex + 1;

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
    }
}
