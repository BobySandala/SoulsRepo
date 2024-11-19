using UnityEngine;

public class PlayerAnimControl : MonoBehaviour
{
    private Animator playerAnim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerAnim = GetComponent<Animator> ();
    }

    // Update is called once per frame
    void Update()
    {
        if (playerAnim != null)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                playerAnim.SetBool("IsIdle", false);
                playerAnim.SetBool("IsRunning", true);
            } else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                playerAnim.SetBool("IsIdle", true);
                playerAnim.SetBool("IsRunning", false);
            } else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                playerAnim.SetTrigger("trig1");
            }
        }
        
    }
}
