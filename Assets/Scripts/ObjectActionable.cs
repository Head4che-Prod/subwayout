using Unity.Netcode;
using UnityEngine;

public class ObjectActionable : ObjectInteractive
{
    [SerializeField] private NetworkAnimatorP2P animator;
    public virtual void Action()
    {
    }
}