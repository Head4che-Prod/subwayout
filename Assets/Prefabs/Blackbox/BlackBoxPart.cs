using Objects;
using UnityEngine;

namespace Prefabs.Blackbox
{
    public class BlackBoxPart : MonoBehaviour, IObjectActionable
    {
        public BlackBox blackBox;
        
        public void Action() => blackBox.Action();
    }
}