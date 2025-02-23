using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Prefabs.Player
{
    public class PlayerObjectPick : NetworkBehaviour
    {
        [SerializeField] private int reach;
        
        private PlayerObject _player;
        private ObjectGrabbable _objectGrabbable;
        private InputAction _interactInput;

        private void Start()
        {
            _player = GetComponent<PlayerObject>();
            _interactInput = _player.Input.actions["Interact"];
        }

        private void Update()
        {
            Debug.DrawRay(_player.Camera.transform.position, _player.Camera.transform.forward * reach, Color.blue);
            if (_interactInput.WasPressedThisFrame())
            {
                if (_objectGrabbable is null) // No item in hand
                {
                    if (Physics.Raycast(_player.Camera.transform.position,
                            _player.Camera.transform.forward, out RaycastHit raycastHit, reach))
                    {
                        // Debug.Log(raycastHit.collider.gameObject.name);
                        if (raycastHit.transform.TryGetComponent(out ObjectGrabbable objGrabbable) && objGrabbable.Grabbable)
                        {
                            _objectGrabbable = objGrabbable;
                            _objectGrabbable.Grab(_player.GrabPointTransform, _player.Camera.transform);
                        }
                    }
                }
                else
                {
                    _objectGrabbable.Drop();
                    _objectGrabbable = null;
                }
            }
        }
    }
}