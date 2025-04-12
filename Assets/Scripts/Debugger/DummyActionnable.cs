using Objects;
using Prefabs.Player;
using UnityEngine;

namespace Debugger
{
    public class DummyActionnable : ObjectActionable
    {
        protected override void Action(PlayerObject player)
        {
            Debug.Log($"i'm {gameObject.name} dummy!");
        }
    }
}