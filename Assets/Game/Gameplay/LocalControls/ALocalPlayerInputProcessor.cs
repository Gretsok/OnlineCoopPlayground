using System;
using Game.Playground.Controls;
using UnityEngine;

namespace Game.Gameplay.LocalControls
{
    public abstract class ALocalPlayerInputProcessor : MonoBehaviour
    {
        public PlayerActions Actions { get; private set; }

        public bool IsActivated { get; private set; }
        
        public void Activate()
        {
            Actions = new PlayerActions();
            Actions.Enable();
            HandleActivation();
            
            IsActivated = true;
        }
        
        protected virtual void HandleActivation()
        {}

        private void Update()
        {
            if (!IsActivated)
                return;
            
            UpdateInput();
        }
        
        protected virtual void UpdateInput()
        { }

        public void Deactivate()
        {
            Actions.Dispose();
            Actions = null;
            HandleDeactivation();
            IsActivated = false;
        }

        protected virtual void HandleDeactivation()
        {
            
        }
    }
}