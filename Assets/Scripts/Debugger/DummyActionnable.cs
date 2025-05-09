using Objects;
using Prefabs.Player;
using UnityEngine;

namespace Debugger
{
    public class DummyActionnable : MonoBehaviour, IObjectActionable
    {
        public string soundEffectName => "test";
        public void Action()
        {
            Debug.Log($"i'm {gameObject.name} dummy!");
        }
    }
}