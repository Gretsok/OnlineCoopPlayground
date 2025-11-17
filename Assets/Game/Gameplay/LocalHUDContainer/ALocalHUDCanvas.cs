using System.Collections;
using Game.Gameplay.Controls;
using Game.Gameplay.PlayerCharacter;
using Game.Gameplay.PlayerCharacter.Implementations.Default;
using UnityEngine;

namespace Game.Gameplay.LocalHUDContainer
{
    public class ALocalHUDCanvas : MonoBehaviour
    {
        protected APlayerMotor LocalMotor { get; private set; }
        
        IEnumerator Start()
        {
            var localPlayerController = LocalPlayerController.Instance;
            yield return new WaitUntil(() => localPlayerController.DefaultLocalPlayerInputProcessor.AssignedMotor != null);
            localPlayerController.DefaultLocalPlayerInputProcessor.OnCharacterAssigned += HandleNewCharacterAssigned;
            HandleNewCharacterAssigned(localPlayerController.DefaultLocalPlayerInputProcessor);
        }

        private void HandleNewCharacterAssigned(DefaultLocalPlayerInputProcessor a_defaultLocalPlayerInputProcessor)
        {
            LocalMotor = a_defaultLocalPlayerInputProcessor.AssignedMotor;
        }
    }
}
