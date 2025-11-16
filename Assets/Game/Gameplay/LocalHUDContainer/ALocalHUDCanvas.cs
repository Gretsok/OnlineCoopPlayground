using System.Collections;
using Game.Gameplay.Controls;
using UnityEngine;

namespace Game.Gameplay.LocalHUDContainer
{
    public class ALocalHUDCanvas : MonoBehaviour
    {
        protected PlayerCharacter.PlayerCharacter M_LocalCharacter { get; private set; }
        
        IEnumerator Start()
        {
            var localPlayerController = LocalPlayerController.Instance;
            yield return new WaitUntil(() => localPlayerController.AssignedCharacter != null);
            localPlayerController.OnCharacterAssigned += HandleNewCharacterAssigned;
            HandleNewCharacterAssigned(localPlayerController);
        }

        private void HandleNewCharacterAssigned(LocalPlayerController a_localPlayerController)
        {
            M_LocalCharacter = a_localPlayerController.AssignedCharacter;
        }
    }
}
