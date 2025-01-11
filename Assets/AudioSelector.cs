using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Unity.VisualScripting.Member;

public class AudioSelector : MonoBehaviour
{
    int current_index;

    public AudioClip villageSound;
    public AudioClip cathedralSound;
    public AudioClip cryptSund;

    private AudioClip currentSound;
    private AudioSource source;
    public void Start()
    {
        source = GetComponent<AudioSource>();
        current_index = SceneManager.GetActiveScene().buildIndex;
        Debug.Log(current_index);
        if (current_index == 3)
        {
            //scena catedrala
            Debug.Log("cat");
            currentSound = villageSound;

        }
        else if (current_index == 2)
        {
            //scena crypta
            Debug.Log("crypt");
            currentSound = cryptSund;
        }

        if (source != null)
        {
            source.loop = true;
            source.Stop();
            source.clip = currentSound;
            source.Play();
        }
        else
        {
            Debug.Log("null source");
        }

    }
    public void ToggleAudioChatedral()
    {
        Debug.Log("Toggle Sound");
        if (currentSound != null)
        {
            if (currentSound == cathedralSound)
            {
                currentSound = villageSound;
            }
            else
            {
                currentSound = cathedralSound;
            }
        }
        source.clip = currentSound;
        source.Play();
    }
}
