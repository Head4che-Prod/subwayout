using UnityEngine;
using UnityEngine.Serialization;

namespace PlayerSelectionMenu
{
    public class PlayerSelectionAnimation : MonoBehaviour
    {
        private Vector3 _startPosition;
        private Vector3 _midPosition;

        [FormerlySerializedAs("finalPosition")] [SerializeField]
        public Vector3 targetPosition = new Vector3(0, 0, 0);

        private void Awake()
        {
            _midPosition = targetPosition;
            _startPosition = transform.position; // set to the offscreen pos
            targetPosition = _startPosition;
        }

        private void Update()
        {
            transform.position =
                Vector3.Lerp(transform.position, targetPosition, 0.03f); // Move the object towards the target position
        }

        /// <summary>
        /// Sets the new target position to a side of the screen.
        /// </summary>
        /// <param name="onTheLeft">Whether the model leaves to the left.</param>
        public void MoveOut(bool onTheLeft)
        {
            targetPosition =
                new Vector3((onTheLeft ? 1 : -1) * _startPosition.x, _startPosition.y,
                    _startPosition.z); // When disabled pos returns to the start one (offscreen one)
        }

        /// <summary>
        /// Sets the new target position to the center of the screen.
        /// </summary>
        /// <param name="fromTheLeft">Whether the model comes in from the left.</param>
        public void MoveIn(bool fromTheLeft)
        {
            transform.position = fromTheLeft ? _startPosition : -_startPosition;
            targetPosition = _midPosition;
        }

        /// <summary>
        /// Sets the model to the center of selection screen without animations.
        /// </summary>
        public void ShowAtStart()
        {
            transform.position = _midPosition;
            targetPosition = _midPosition;
        }
    }
}