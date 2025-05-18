using Objects;
using UnityEngine;

namespace Collisions
{
    [RequireComponent(typeof(Collider))]
    public class KillZone : MonoBehaviour
    {
        public void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out IResettablePosition resettablePosition))
                resettablePosition.ResetPosition();
            else 
                Debug.LogWarning($"Kill plane {name} hit an unexpected object: {collision.gameObject.name}.");
        }
    }
}