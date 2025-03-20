using System;
using Objects;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Prefabs.Player
{
    public class PlayerInteract : NetworkBehaviour
    {
        [Header("Player")]
        [SerializeField] private PlayerObject player;        
        [SerializeField] private float reach;
        
        [NonSerialized] public ObjectGrabbable GrabbedObject;
        private InputAction _actionInput;
        private InputAction _grabInput;
        
        private void Start()
        {
            _actionInput = InputSystem.actions.FindAction("Gameplay/Interact");
            _grabInput = InputSystem.actions.FindAction("Gameplay/Grab");

            try
            {
                _actionInput.performed += _ => HandleAction();
                _grabInput.performed += _ => HandleGrab();
            }
            catch (NullReferenceException e)
            {
                Debug.LogError(e.Message);
            }
        }

        private void HandleAction()
        {
            Debug.DrawRay(
                player.playerCamera.transform.position, 
                player.playerCamera.transform.forward * reach, 
                Color.red
            );
            
            Physics.Raycast(
                player.playerCamera.transform.position,
                player.playerCamera.transform.forward,
                out RaycastHit actionHit,
                reach
            );
            
            if (actionHit.transform is not null && actionHit.transform.TryGetComponent(out ObjectActionable actionable))
                actionable.HandleAction(player);
        }
        
        private void HandleGrab()
        {
            Debug.DrawRay(
                player.playerCamera.transform.position, 
                player.playerCamera.transform.forward * reach, 
                Color.blue
            );
            
            Physics.Raycast(
                player.playerCamera.transform.position,
                player.playerCamera.transform.forward,
                out RaycastHit grabHit,
                reach
            );

            if (grabHit.transform is not null && grabHit.transform.TryGetComponent(out ObjectGrabbable grabbable))
            {
                if (grabbable == GrabbedObject)
                    GrabbedObject.Drop();
                
                // Grab an object
                else if (grabbable is { Grabbable: true } &&
                         GrabbedObject is null)
                {
                    GrabbedObject = grabbable;
                    GrabbedObject.Grab(player);
                }
            }
            else if (GrabbedObject is not null)
                GrabbedObject.Drop();
            
        }
    }
}