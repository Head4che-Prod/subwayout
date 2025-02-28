using System;
using Objects;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

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
                _actionInput.performed += HandlePress;
                _actionInput.canceled += HandleRelease;
                _grabInput.performed += HandlePress;
                _grabInput.canceled += HandleRelease;
            }
            catch (NullReferenceException e)
            {
                Debug.LogError(e.Message);
            }
        }
        
        /// <summary>
        /// This method handles button press.
        /// </summary>
        /// <param name="context"><see cref="InputAction"/>'s <see cref="InputAction.CallbackContext"/> of the press</param>
        private void HandlePress(InputAction.CallbackContext context)
        {
            Debug.DrawRay(player.playerCamera.transform.position, player.playerCamera.transform.forward * reach, Color.blue);
            /*
            RaycastHit[] hits = new RaycastHit[2];
            int hitCount = Physics.RaycastNonAlloc(playerCamera.transform.position, playerCamera.transform.forward, hits, reach);
            */

            Physics.Raycast(
                player.playerCamera.transform.position,
                player.playerCamera.transform.forward,
                out RaycastHit hit,
                reach
            );
            
            // Handle the first raycast hit
            if (/*hits[0]*/hit.transform is not null && /*hits[0]*/hit.transform.TryGetComponent(out ObjectInteractable interactable))
            {
                // Action an object
                if (context.action.id == _actionInput.id && interactable is ObjectActionable objActionable) 
                {   
                    objActionable.HandleAction(player);
                }
                
                // Grab an object
                else if (context.action.id == _grabInput.id &&
                         interactable is ObjectGrabbable { Grabbable: true } objGrabbable &&
                         GrabbedObject is null)
                {
                    GrabbedObject = objGrabbable;
                    GrabbedObject.Grab(player);
                    return;
                }
            } 
            
            if (GrabbedObject is not null && context.action.id == _grabInput.id)
            {
                // Place an object
                
                /*
                if (
                    hitCount > 1 &&
                    hits[1].transform is not null &&
                    hits[1].transform.TryGetComponent(out ObjectPlaceholder placeholder) &&
                    placeholder.Free &&
                    _grabbedObject.ConvertActionableType == placeholder.actionableType
                )
                {
                    _grabbedObject.ToActionable(placeholder);
                    _grabbedObject = null;
                }
                */
            
                // Drop an object
                
                // else
                {
                    GrabbedObject.Drop();
                }
            }
        }

        /// <summary>
        /// This method handles button release.   // Why tho?
        /// </summary>
        /// <param name="context"><see cref="InputAction"/>'s <see cref="InputAction.CallbackContext"/> of the release</param>
        private void HandleRelease(InputAction.CallbackContext context)
        {
        }
    }
}