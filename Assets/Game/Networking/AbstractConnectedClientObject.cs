using System.Collections;
using Game.Gameplay.CharactersManagement;
using Game.Gameplay.Controls;
using Game.Gameplay.PlayerCharacter;
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
            
            PlayersCharactersManager.Instance.RetrieveCharacterFor_ForOwner(this, HandlePlayerCharacterReceived);
        }

        private void HandlePlayerCharacterReceived(APlayerMotor a_obj)
        {
            LocalPlayerController.Instance.DefaultLocalPlayerInputProcessor.AssignCharacter(a_obj as DefaultPlayerMotor);
        }

        public override void OnNetworkPreDespawn()
        {
            base.OnNetworkPreDespawn();
            PlayersCharactersManager.Instance.DeleteCharacterFor_ForOwner(this);
        }
    }
}
