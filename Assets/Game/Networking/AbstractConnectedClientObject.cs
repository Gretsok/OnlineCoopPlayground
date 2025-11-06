using Steamworks;
using Unity.Netcode;

namespace Game.Networking
{
    public class AbstractConnectedClientObject : NetworkBehaviour
    {
        public readonly NetworkVariable<SteamId> SteamId = new NetworkVariable<SteamId>(writePerm: NetworkVariableWritePermission.Owner);

        protected override void OnNetworkPostSpawn()
        {
            base.OnNetworkPostSpawn();
            SteamId.Value = SteamClient.SteamId;
        }

        public override void OnGainedOwnership()
        {
            base.OnGainedOwnership();
            SteamId.Value = SteamClient.SteamId;;
        }
    }
}
