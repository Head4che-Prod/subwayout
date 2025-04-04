﻿using System;
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
        [Header("Raycasting")]
        [SerializeField] private int allocationSize;
        
        private void Start()
        {
            _actionInput = InputSystem.actions.FindAction("Gameplay/Interact");
            _grabInput = InputSystem.actions.FindAction("Gameplay/Grab");
            
            _actionInput.performed += HandleAction;
            _grabInput.performed += HandleGrab;
        }

        private void HandleAction(InputAction.CallbackContext context)
        {
            ObjectActionable actionable = null;
            try
            {
                RaycastHit[] hits = new RaycastHit[allocationSize];
                Physics.RaycastNonAlloc(
                    player.playerCamera.transform.position, 
                    player.playerCamera.transform.forward,
                    hits, 
                    reach
                );
                
                float distance = hits
                    .OrderBy(hit => hit.distance > 0 ? hit.distance : float.MaxValue)
                    .TakeWhile(hit => hit.transform != null && hit.transform.TryGetComponent<ObjectInteractable>(out _))
                    .First(hit => hit.transform.TryGetComponent<ObjectActionable>(out actionable)).distance;
                
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
                actionable.HandleAction(player);
            }
        }
        
        private void HandleGrab(InputAction.CallbackContext context)
        {
            ObjectGrabbable grabbable = null;
            try
            {
                RaycastHit[] hits = new RaycastHit[allocationSize];
                Physics.RaycastNonAlloc(
                    player.playerCamera.transform.position,
                    player.playerCamera.transform.forward,
                    hits,
                    reach
                );
                
                float distance = hits
                    .OrderBy(hit => hit.distance > 0 ? hit.distance : float.MaxValue)
                    .TakeWhile(hit => hit.transform != null && hit.transform.TryGetComponent<ObjectInteractable>(out _))
                    .First(hit => hit.transform.TryGetComponent<ObjectGrabbable>(out grabbable)).distance;
                
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
            
            if (_actionInput != null) _actionInput.performed -= HandleAction;
            if (_grabInput != null) _grabInput.performed -= HandleGrab;
        }
    }
}