using System;
using System.Collections.Generic;
using System.Linq;
using Objects;
using Prefabs.GameManagers;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Prefabs.Player
{
    public class PlayerInteract : NetworkBehaviour
    {
        private static PlayerInteract _singleton;
        public static PlayerInteract LocalPlayerInteract
        {
            get 
            {
                if (!_singleton)
                    Debug.LogError("No PlayerInteract instance found.");
                return _singleton;
            }
            private set
            {
                if (!value)
                    _singleton = null;
                else if (value.IsLocalPlayer)
                {
                    if (!_singleton)
                        _singleton = value;
                    else 
                        Debug.LogError("Attempting to create a new PlayerInteract, but one already exists.");
                }
            }
        }
        
        
        [Header("Player")]
        [SerializeField] private PlayerObject player;
        [SerializeField] private float reach;

        [NonSerialized] public IObjectGrabbable GrabbedObject;
        private InputAction _actionInput;
        private InputAction _grabInput;
        
        private const int AllocationSize = 6;

        private void Start()
        {
            LocalPlayerInteract = this;
            
            _actionInput = player.Input.actions.FindAction("Gameplay/Interact");
            _grabInput = player.Input.actions.FindAction("Gameplay/Grab");
            
            _actionInput.performed += HandleAction;
            _grabInput.performed += HandleGrab;
        }

        private void HandleAction(InputAction.CallbackContext context)
        {
            IObjectActionable actionable = null;
            try
            {
                RaycastHit[] hits = new RaycastHit[AllocationSize];
                Physics.RaycastNonAlloc(
                    player.playerCamera.transform.position, 
                    player.playerCamera.transform.forward,
                    hits, 
                    reach
                );
                
                float distance = hits
                    .OrderBy(hit => hit.distance > 0 ? hit.distance : float.MaxValue)
                    .TakeWhile(hit => hit.collider.transform != null && hit.collider.transform.TryGetComponent<IRaycastResponsive>(out _))
                    .First(hit => hit.collider.transform.TryGetComponent<IObjectActionable>(out actionable)).distance;
                
                Debug.DrawRay(
                    player.playerCamera.transform.position, 
                    player.playerCamera.transform.forward * distance,
                    Color.red
                );
            }
            catch (Exception e)
            {
                if (e is NullReferenceException or InvalidOperationException) return; // actionHit not found
                throw;
            }

            if (actionable != null)
            {
                actionable.Action();
            }
        }
        
        private void HandleGrab(InputAction.CallbackContext context)
        {
            if (GrabbedObject is not null)
            {
                GrabbedObject.Drop();
                return;
            }
            
            IObjectGrabbable grabbable = null;
            try
            {
                RaycastHit[] hits = new RaycastHit[AllocationSize];
                Physics.RaycastNonAlloc(
                    player.playerCamera.transform.position,
                    player.playerCamera.transform.forward,
                    hits,
                    reach
                );
                
                float distance = hits
                    .OrderBy(hit => hit.distance > 0 ? hit.distance : float.MaxValue)
                    .TakeWhile(hit => hit.collider.transform != null && hit.collider.transform.TryGetComponent<IObjectGrabbable>(out _))
                    .First(hit => hit.collider.transform.TryGetComponent<IObjectGrabbable>(out grabbable)).distance;
                
                Debug.DrawRay(
                    player.playerCamera.transform.position, 
                    player.playerCamera.transform.forward * distance, 
                    Color.blue
                );
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
            
            if (grabbable != null)
            {
                // Grab an object
                if (grabbable is { Grabbable: true } && GrabbedObject is null)
                {
                    GrabbedObject = grabbable.GrabbedObject;
                    GrabbedObject.Grab();
                }
                // Drop grabbed pointed grabbed object
                else if (grabbable == GrabbedObject)
                    GrabbedObject.Drop();
            }
        }

        public override void OnDestroy()
        {
            if (_actionInput != null) _actionInput.performed -= HandleAction;
            if (_grabInput != null) _grabInput.performed -= HandleGrab;
            
            if (LocalPlayerInteract == this) LocalPlayerInteract = null;
            
            base.OnDestroy();
        }
    }
}