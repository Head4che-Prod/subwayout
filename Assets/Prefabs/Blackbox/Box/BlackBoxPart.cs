using Objects;
using UnityEngine;

namespace Prefabs.Blackbox.Box
{
    public class BlackBoxPart : MonoBehaviour, IObjectActionable
    {
        public BlackBox blackBox;
        
        public void Action() => blackBox.Action();
    }
}