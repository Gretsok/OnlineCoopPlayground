using Steamworks.Data;
using UnityEngine;

namespace Game.SteamIntegration
{
    /// <summary>
    /// What this class does is already done in <see cref="Netcode.Transports.Facepunch.FacepunchTransport"/>. Do not use this class if you are already using the <see cref="Netcode.Transports.Facepunch.FacepunchTransport"/>.
    /// </summary>
    
    public class SteamLifeCycleManager : MonoBehaviour
    {
        public static SteamLifeCycleManager Instance { get; private set; }
        public bool SteamHasBeenInitialized { get; private set; }

        [SerializeField]
        private uint m_appId = 480;
        
        void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
                return;
            }
            
            DontDestroyOnLoad(gameObject);
            Instance = this;
            
            try
            {
                Steamworks.SteamClient.Init(m_appId);
                SteamHasBeenInitialized = true;
            }
            catch (System.Exception e)
            {
                // Something went wrong - it's one of these:
                //
                //     Steam is closed?
                //     Can't find steam_api dll?
                //     Don't have permission to play app?
                //
                Debug.LogError(e);
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!SteamHasBeenInitialized)
                return;

            Steamworks.SteamClient.RunCallbacks();
        }

        private void OnDestroy()
        {
            if (!SteamHasBeenInitialized)
                return;
            
            Steamworks.SteamClient.Shutdown();
        }
    }
}
