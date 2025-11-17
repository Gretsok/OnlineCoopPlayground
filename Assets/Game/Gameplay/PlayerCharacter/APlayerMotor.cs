using Game.Gameplay.GameplayInteractionsSystems.EffectsSystem;
using Game.Gameplay.GameplayInteractionsSystems.HealthHandling;
using Game.Gameplay.PlayerCharacter.Animation;
using Game.Gameplay.PlayerCharacter.CharacterImplementations;
using Unity.Netcode;
using UnityEngine;

namespace Game.Gameplay.PlayerCharacter
{
    public class APlayerMotor : NetworkBehaviour,
        IHealthControllerHolder,
        IEffectsControllerHolder,
        IPlayerCharacterAnimationControllerHolder,
        IPlayerCharacterGameDataRetrieverAndInjectorHolder
    {
        [field: SerializeField]
        public NetworkObject CharacterPawnAnchorPrefab { get; private set; }
        public NetworkObject CharacterPawnAnchor { get; private set; }
        [field: SerializeField]
        public PlayerCharacterPawn PlayerCharacterPawnPrefab { get; private set; }
        public PlayerCharacterPawn PlayerCharacterPawn { get; private set; }

        public HealthController HealthController => PlayerCharacterPawn?.HealthController;
        public EffectsController EffectsController => PlayerCharacterPawn?.EffectsController;
        public PlayerCharacterAnimationController PlayerCharacterAnimationController => PlayerCharacterPawn?.PlayerCharacterAnimationController;
        public PlayerCharacterGameDataRetrieverAndInjector PlayerCharacterGameDataRetrieverAndInjector => PlayerCharacterPawn?.PlayerCharacterGameDataRetrieverAndInjector;
        
        
        protected override void OnNetworkPostSpawn()
        {
            base.OnNetworkPostSpawn();
            if (IsServer)
            {
                CharacterPawnAnchor = Instantiate(CharacterPawnAnchorPrefab);
                CharacterPawnAnchor.SpawnWithOwnership(OwnerClientId);
                CharacterPawnAnchor.TrySetParent(NetworkObject);
            
                PlayerCharacterPawn = Instantiate(PlayerCharacterPawnPrefab);
                PlayerCharacterPawn.NetworkObject.SpawnWithOwnership(OwnerClientId);
                PlayerCharacterPawn.NetworkObject.TrySetParent(CharacterPawnAnchor);
            
                SetUpDependencies();
            }
            else
            {
                RequestReferences_ServerRpc(NetworkManager.LocalClientId);
            }

        }

        [Rpc(SendTo.Server)]
        private void RequestReferences_ServerRpc(ulong a_clientId)
        {
            SendReferences_SpecificClientRpc(CharacterPawnAnchor, PlayerCharacterPawn, RpcTarget.Single(a_clientId, RpcTargetUse.Temp));
        }

        [Rpc(SendTo.SpecifiedInParams)]
        private void SendReferences_SpecificClientRpc(NetworkObjectReference a_anchorReference,
            NetworkBehaviourReference a_characterPawnReference, RpcParams a_rpcParams = default)
        {
            if (a_anchorReference.TryGet(out NetworkObject anchor))
            {
                CharacterPawnAnchor = anchor;
            }
            else
            {
                Debug.LogError($"[CLIENT] Anchor reference is invalid.");
            }

            if (a_characterPawnReference.TryGet(out PlayerCharacterPawn characterPawn))
            {
                PlayerCharacterPawn = characterPawn;
            }
            else
            {
                Debug.LogError($"[CLIENT] Character pawn reference is invalid.");
            }
            
            SetUpDependencies();
        }

        protected virtual void SetUpDependencies()
        {
            PlayerCharacterGameDataRetrieverAndInjector.SetDependencies(this);
        }
    }
}