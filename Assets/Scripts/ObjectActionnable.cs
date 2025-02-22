using Unity.Netcode;
using UnityEngine;

public class ObjectActionnable : NetworkBehaviour
{
    [SerializeField] private NetworkAnimatorP2P animator;
    public virtual void Action()
    {
    }
}