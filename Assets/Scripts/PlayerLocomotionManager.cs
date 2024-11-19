using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace DS_ripoff
{
    public class PlayerLocomotionManager : CharacterLocomotionManager
    {

        Player1Manager player;

        public float verticalMovement;
        public float horizontalMovement;
        public float moveAmount;

        [SerializeField] float runningSpeed = 2;
        [SerializeField] float walkingSpeed = 5;

        [SerializeField]
        private Vector3 moveDirection;
        public void HandleAllMovement()
        {
            HandleGroundedMovement();
        }

        protected override void Awake()
        {
            base.Awake();

            player= GetComponent<Player1Manager>();
        }

        private void GetVerticalAndHorizontalMovement()
        {
            verticalMovement = PlayerInputManager.instance.verticalInput;
            horizontalMovement = PlayerInputManager.instance.horizontalInput;

            //clamp inputs
        }

        private void HandleGroundedMovement()
        {
            GetVerticalAndHorizontalMovement();
            moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
            moveDirection = moveDirection + PlayerCamera.instance.transform.right * horizontalMovement;
            moveDirection.Normalize();
            moveDirection.y = 0;

            if (PlayerInputManager.instance.moveAmount > 0.5f)
            {
                //move at running speed
                player.characterController.Move(runningSpeed * Time.deltaTime * moveDirection);
            } else if (PlayerInputManager.instance.moveAmount <= 0.5)
            {
                //walking speed
                player.characterController.Move(Time.deltaTime * walkingSpeed * moveDirection);
            }
        }
    }
}
