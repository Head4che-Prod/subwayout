using UnityEngine;

namespace Debugger 
{
    public class CollisionDetector : MonoBehaviour
    {
        void OnCollisionEnter(Collision collision)
        {
            Debug.Log($"{name} collided with {collision.gameObject.name}");
        }
    }
}
