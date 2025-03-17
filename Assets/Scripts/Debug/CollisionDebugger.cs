using UnityEngine;

namespace Debug
{
    public class CollisionDetector : MonoBehaviour
    {
        void OnCollisionEnter(Collision collision)
        {
            UnityEngine.Debug.Log($"{name} collided with {collision.gameObject.name}");
        }
    }
}
