using System.Collections;
using System.Collections.Generic;
using DS_ripoff;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player1Manager : CharacterManager
{
    PlayerLocomotionManager playerLocomotionManager;
    protected override void Awake()
    {
        base.Awake();
        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
    }
    private void Start()
    {

    }
    protected override void Update()
    {
        playerLocomotionManager.HandleAllMovement();
    }

}
