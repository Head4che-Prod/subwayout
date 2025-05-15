using Objects;
using Prefabs.GameManagers;
using Prefabs.Player;
using Unity.Netcode;
using UnityEngine;

public class TomeTomeAdManager : IObjectActionable
{
    [SerializeField] private ObjectGrabbable _keyGrabbable;
    [SerializeField] private GameObject _keyInAd;
    
    public void Action()
    {
        if ((ObjectGrabbable)PlayerInteract.LocalPlayerInteract.GrabbedObject == _keyGrabbable)
        {
            DeactivateGrabbedKey();
        }
    }
    
    /// <summary>
    /// Deactivate the grabbed key but activate the 3D model on in the ad.
    /// </summary>
    public void DeactivateGrabbedKey()
    {
        _keyGrabbable.Drop();
        DisableCheeseRpc(_keyGrabbable.NetworkObjectId);
        ActivateKeyInAdRpc();
    }
    /// <summary>
    /// Removes the key that was grabbed.
    /// </summary>
    [Rpc(SendTo.Server, RequireOwnership = false)]
    private void DisableCheeseRpc(ulong keyGrabbedID)
    {
        ObjectHighlightManager.ForgetHighlightableObject(keyGrabbedID);
        NetworkManager.Singleton.SpawnManager.SpawnedObjects[keyGrabbedID].Despawn();
    }
    
    /// <summary>
    /// Activate the key in the cheese ad.
    /// </summary>
    [Rpc(SendTo.Everyone)]
    private void ActivateKeyInAdRpc()
    {
        _keyInAd.SetActive(true);
    }

}
