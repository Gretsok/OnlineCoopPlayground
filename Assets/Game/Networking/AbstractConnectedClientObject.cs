using System.Collections;
using Game.Gameplay.CharactersManagement;
using Game.Gameplay.LocalControls;
using Game.Gameplay.PlayerCharacter;
using Game.Gameplay.PlayerCharacter.MotorImplementations.Default;
using Steamworks;
using Unity.Netcode;
using UnityEngine;

namespace Game.Networking
{
    public class AbstractConnectedClientObject : NetworkBehaviour
    {
        private readonly NetworkVariable<ulong> m_steamId = new (writePerm: NetworkVariableWritePermission.Owner);
        public SteamId SteamId => m_steamId.Value;
        
        protected override void OnNetworkPostSpawn()
        {
            base.OnNetworkPostSpawn();
            
            if (!IsOwner)
                return;
            
            StartCoroutine(InitializeAsOwner());
        }

        public override void OnGainedOwnership()
        {
            base.OnGainedOwnership();
            StartCoroutine(InitializeAsOwner());
        }

        private IEnumerator InitializeAsOwner()
        {
            if (SteamClient.IsValid)
                m_steamId.Value = SteamClient.SteamId;

            yield return new WaitUntil(() => LocalPlayerController.Instance);
            
            LocalPlayerController.Instance.AssignClient(this);
            
            PlayersCharactersManager.Instance.CreateMotorFor_ForOwner(this, HandlePlayerCharacterReceived, 
                PlayersCharactersManager.EPlayerMotorType.Default);
        }

        private void HandlePlayerCharacterReceived(APlayerMotor a_obj)
        {
            LocalPlayerController.Instance.DefaultLocalPlayerInputProcessor.AssignCharacter(a_obj as DefaultPlayerMotor);
        }

        public override void OnNetworkPreDespawn()
        {
            base.OnNetworkPreDespawn();
            PlayersCharactersManager.Instance.DeleteMotorFor_ForOwner(this);
        }
    }
}
