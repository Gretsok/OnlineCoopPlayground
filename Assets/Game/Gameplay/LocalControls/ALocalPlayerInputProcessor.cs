using Game.Playground.Controls;
using UnityEngine;

namespace Game.Gameplay.LocalControls
{
    public abstract class ALocalPlayerInputProcessor : MonoBehaviour
    {
        public PlayerActions Actions { get; protected set; }
    }
}