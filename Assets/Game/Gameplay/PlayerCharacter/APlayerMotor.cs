using Game.Gameplay.GameplayInteractionsSystems.EffectsSystem;
using Game.Gameplay.GameplayInteractionsSystems.HealthHandling;
using Game.Gameplay.PlayerCharacter.Animation;
using Game.Gameplay.PlayerCharacter.CharacterImplementations;
using Game.Gameplay.VehiclesSystem;
using Unity.Netcode;
using UnityEngine;

namespace Game.Gameplay.PlayerCharacter
{
    public class APlayerMotor : NetworkBehaviour,
        IHealthControllerHolder,
        IEffectsControllerHolder,
        IPlayerCharacterAnimationControllerHolder,
        IPlayerCharacterGameDataRetrieverAndInjectorHolder,
        IVehiclePassengerControllerHolder
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
        public VehiclePassengerController VehiclePassengerController => PlayerCharacterPawn?.VehiclePassengerController;

        public void SetPlayerCharacterPawn(PlayerCharacterPawn a_playerPawn)
        {
            PlayerCharacterPawn = a_playerPawn;
        }
        
        protected override void OnNetworkPostSpawn()
        {
            base.OnNetworkPostSpawn();

            if (!IsServer)
            {
                RequestReferences_ServerRpc(NetworkManager.LocalClientId);
            }
        }

        private bool m_hasBeenSetUp_ServerOnly = false;
        public bool HasBeenSetUp_ServerOnly => m_hasBeenSetUp_ServerOnly;
        public void SetUpPawnInMotor_ForServer(PlayerCharacterPawn a_existingPawn = null)
        {
            if (!IsServer)
                return;
            if (m_hasBeenSetUp_ServerOnly)
                return;


            
            CharacterPawnAnchor = Instantiate(CharacterPawnAnchorPrefab);
            CharacterPawnAnchor.SpawnWithOwnership(OwnerClientId);
            CharacterPawnAnchor.TrySetParent(NetworkObject);

            if (!a_existingPawn)
                PlayerCharacterPawn = Instantiate(PlayerCharacterPawnPrefab);
            else
                PlayerCharacterPawn = a_existingPawn;
            
            if (!PlayerCharacterPawn.IsSpawned)
                PlayerCharacterPawn.NetworkObject.SpawnWithOwnership(OwnerClientId);

            PlayerCharacterPawn.NetworkObject.TrySetParent(CharacterPawnAnchor);

            SetUpDependencies();
            m_hasBeenSetUp_ServerOnly = true;
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
            VehiclePassengerController.SetDependencies(this);
        }

        public override void OnNetworkPreDespawn()
        {
            base.OnNetworkPreDespawn();
            if (!IsServer)
                return;
            
            Destroy(CharacterPawnAnchor.gameObject);
        }
    }
}