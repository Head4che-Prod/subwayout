using System;
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

            try
            {
                _actionInput.performed += HandleAction;
                _grabInput.performed += HandleGrab;
            }
            catch (NullReferenceException e)
            {
                Debug.LogError(e.Message);
            }
        }

        private void HandleAction(InputAction.CallbackContext context)
        {
            Debug.Log("Try to action");

            RaycastHit actionHit;
            try
            {
                RaycastHit[] results = new RaycastHit[3];
                Physics.RaycastNonAlloc(player.playerCamera.transform.position, player.playerCamera.transform.forward,
                    results, reach);
                actionHit = results.First(hit => hit.transform.TryGetComponent<ObjectActionable>(out _));
            }
            catch (NullReferenceException)
            {
                return;
            }
            
            Debug.DrawRay(
                player.playerCamera.transform.position, 
                player.playerCamera.transform.forward * actionHit.distance, 
                Color.red
            );

            if (actionHit.transform is not null && actionHit.transform.TryGetComponent(out ObjectActionable actionable))
            {
                Debug.Log($"Action {actionable.name}");
                actionable.HandleAction(player);
            }
        }
        
        private void HandleGrab(InputAction.CallbackContext context)
        {
            Debug.Log("Try to grab");
            
            
            RaycastHit grabHit;
            try
            {
                RaycastHit[] hits = new RaycastHit[3];
                Physics.RaycastNonAlloc(
                    player.playerCamera.transform.position, 
                    player.playerCamera.transform.forward, 
                    hits, 
                    reach
                );
                grabHit = hits
                    .First(hit => hit.transform.TryGetComponent<ObjectGrabbable>(out _));
            }
            catch (NullReferenceException)
            {
                if (GrabbedObject is not null)
                {
                    Debug.Log($"Drop {GrabbedObject.name}");
                    GrabbedObject.Drop();
                }
                return;
            }
            
            Debug.DrawRay(
                player.playerCamera.transform.position, 
                player.playerCamera.transform.forward * grabHit.distance, 
                Color.blue
            );

            if (grabHit.transform is not null && grabHit.transform.TryGetComponent(out ObjectGrabbable grabbable))
            {
                if (grabbable == GrabbedObject)
                {
                    Debug.Log($"Drop pointed {GrabbedObject.name}");
                    GrabbedObject.Drop();
                }

                // Grab an object
                else if (grabbable is { Grabbable: true } && GrabbedObject is null)
                {
                    Debug.Log($"Grab {grabbable.name}");
                    GrabbedObject = grabbable;
                    GrabbedObject.Grab(player);
                }
            }
            else if (GrabbedObject is not null)
            {
                Debug.Log($"Drop {GrabbedObject.name}");
                GrabbedObject.Drop();
            }
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            if (_actionInput != null)
            {
                _actionInput.performed -= HandleAction;
            }

            if (_grabInput != null)
            {
                
                _grabInput.performed -= HandleGrab;
            }
        }
    }
}