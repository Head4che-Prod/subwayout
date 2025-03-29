using System;
using System.Collections.Generic;
using System.Linq;
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
            
            _actionInput.performed += HandleAction;
            _grabInput.performed += HandleGrab;
        }

        private void HandleAction(InputAction.CallbackContext context)
        {
            RaycastHit actionHit;
            try
            {
                RaycastHit[] hits = new RaycastHit[3];
                Physics.RaycastNonAlloc(
                    player.playerCamera.transform.position, 
                    player.playerCamera.transform.forward,
                    hits, 
                    reach
                );
                
                actionHit = hits
                    .OrderBy(hit => hit.distance > 0 ? hit.distance : float.MaxValue)
                    .TakeWhile(hit => hit.transform != null && hit.transform.TryGetComponent(out ObjectInteractable _))
                    .First(hit => hit.transform.TryGetComponent<ObjectActionable>(out _));
            }
            catch (Exception e)
            {
                if (e is NullReferenceException or InvalidOperationException) return; // actionHit not found
                throw;
            }
            
            Debug.DrawRay(
                player.playerCamera.transform.position, 
                player.playerCamera.transform.forward * actionHit.distance, 
                Color.red
            );

            if (actionHit.transform.TryGetComponent(out ObjectActionable actionable))
            {
                actionable.HandleAction(player);
            }
        }
        
        private void HandleGrab(InputAction.CallbackContext context)
        {
            RaycastHit grabHit;
            ObjectGrabbable grabbable = null;
            try
            {
                RaycastHit[] hits = new RaycastHit[4];
                Physics.RaycastNonAlloc(
                    player.playerCamera.transform.position,
                    player.playerCamera.transform.forward,
                    hits,
                    reach
                );
                
                grabHit = hits
                    .OrderBy(hit => hit.distance > 0 ? hit.distance : float.MaxValue)
                    .TakeWhile(hit => hit.transform != null && hit.transform.TryGetComponent<ObjectInteractable>(out _))
                    .First(hit => hit.transform.TryGetComponent<ObjectGrabbable>(out grabbable));
            }
            catch (Exception e)
            {
                if (e is NullReferenceException or InvalidOperationException)
                {
                    if (GrabbedObject is not null) GrabbedObject.Drop();
                    return;
                }
                throw;
            }
            
            Debug.DrawRay(
                player.playerCamera.transform.position, 
                player.playerCamera.transform.forward * grabHit.distance, 
                Color.blue
            );

            if (grabbable != null)
            {
                // Grab an object
                if (grabbable is { Grabbable: true } && GrabbedObject is null)
                {
                    GrabbedObject = grabbable;
                    GrabbedObject.Grab(player);
                }
                // Drop grabbed pointed grabbed object
                else if (grabbable == GrabbedObject)
                    GrabbedObject.Drop();
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            
            if (_actionInput != null)
                _actionInput.performed -= HandleAction;
            
            if (_grabInput != null)
                _grabInput.performed -= HandleGrab;
        }
    }
}