using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StepSounds : MonoBehaviour
{
    public List<AudioClip> stoneSteps = new List<AudioClip>();
    //public List<AudioClip> carpetSteps = new List<AudioClip>();

    private Rigidbody rb;
    //public float frequencyFactor = 1.0f; // Factor to adjust the frequency scaling
    private enum Surface { stone, carpet };
    private Surface surface;

    private List<AudioClip> currentList;

    private AudioSource source;
    public float callFrequency = 0.3f; // Frequency in seconds

    private void Start()
    {
        source = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        StartCoroutine(CallFunctionBasedOnVelocity());
    }
    public void Update()
    {
        if (currentList == null)
            return;
        // Example: Dynamically adjust frequency based on some condition
        callFrequency = Mathf.Clamp(Time.time % 5, 0.1f, 2.0f);

        /*
        AudioClip clip = currentList[Random.Range(0, currentList.Count)];
        source.PlayOneShot(clip);
        */
    }

    private IEnumerator CallFunctionBasedOnVelocity()
    {
        while (true)
        {
            // Call your desired function here
            PerformTask();

            // Wait for the specified frequency
            yield return new WaitForSeconds(callFrequency);
        }
    }

    private void PerformTask()
    {
        // Your function logic goes here
        float vel = rb.velocity.magnitude;
        if (vel > 1)
        {
            //Debug.Log("Function called. Velocity: " + rb.velocity.magnitude);
            AudioClip clip = stoneSteps[Random.Range(0, stoneSteps.Count)];
            source.PlayOneShot(clip);
        }
    }
    /*
    private void SelectStepList()
    {
        switch (surface)
        {
            case Surface.stone:
                currentList = stoneSteps;
                break;
            case Surface.carpet:
                currentList = carpetSteps;
                break;
            default:
                currentList = null;
                break;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        surface = Surface.stone;

        if (hit.transform.tag == "Carpet")
        {
            surface = Surface.carpet;
        }

        SelectStepList();

    }*/

}