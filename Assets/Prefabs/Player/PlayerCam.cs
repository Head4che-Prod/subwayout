using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Prefabs.Player
{
    public class PlayerCam : NetworkBehaviour
    {
        [Header("Camera")]
        public GameObject playerCamera;
        private Camera _cameraObject;
        
        public static float Sensi = 30;

        private float _xRotation;
        private float _yRotation;
    
        private InputAction _lookAction;
        private PlayerObject _player;
        
        private bool _isActive;

        private NetworkVariable<Quaternion> _rotation = new NetworkVariable<Quaternion>();
        
        public override void OnNetworkSpawn()
        {
            _isActive = IsLocalPlayer;
            playerCamera.gameObject.SetActive(_isActive);
            if (_isActive)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                _player = GetComponentInParent<PlayerObject>();
                _lookAction = _player.Input.actions["Look"];

                _player.playerCamera.cullingMask &= ~(1 << 7);

                _player.playerCharacter.layer = 7;
                SetLayerAllChildren(_player.playerCharacter.transform, 7);

                _cameraObject = playerCamera.GetComponent<Camera>();
                _cameraObject.fieldOfView = 60;
                _player.Input.actions["Zoom"].started += ZoomIn;
                _player.Input.actions["Zoom"].canceled += ZoomOut;
            }
        }

        private void ZoomIn(InputAction.CallbackContext _) => _cameraObject.fieldOfView = 30;
        private void ZoomOut(InputAction.CallbackContext _) => _cameraObject.fieldOfView = 60;
    
        void Update()
        {
            if (_isActive)
            {
                Vector2 lookVector2 = _lookAction.ReadValue<Vector2>();
                float lookX = lookVector2.x * Time.deltaTime * Sensi;
                float lookY = lookVector2.y * Time.deltaTime * Sensi;

                _yRotation += lookX;
                _xRotation -= lookY;
                _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

                _player.transform.rotation = Quaternion.Euler(0, _yRotation, 0);
                transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
                RotateServerRpc(transform.rotation);
            }
            else
                transform.rotation = _rotation.Value;
        }

        [Rpc(SendTo.Server, RequireOwnership = false)]
        private void RotateServerRpc(Quaternion rotation)
        {
            _rotation.Value = rotation;
        }
        
        private void SetLayerAllChildren(Transform root, int layer)
        {
            Transform[] children = root.GetComponentsInChildren<Transform>(includeInactive: true);
            foreach (Transform child in children)
            {
                child.gameObject.layer = layer;
            }
        }

        public override void OnDestroy()
        {
            _player.Input.actions["Zoom"].started -= ZoomIn;
            _player.Input.actions["Zoom"].canceled -= ZoomOut;
            base.OnDestroy();
        }
    }
}