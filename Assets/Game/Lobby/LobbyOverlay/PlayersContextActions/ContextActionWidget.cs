using Steamworks;
using UnityEngine;

namespace Game.MainMenu.Orchestration.SteamLobby
{
    public class ContextActionWidget : MonoBehaviour
    {
        public enum ERequirementState
        {
            DoesNotMatter = 0,
            ShouldBeTrue = 1,
            ShouldBeFalse = 2,
        }
        
        [Header("Display conditions")]
        [SerializeField]
        private ERequirementState m_shouldLocalPlayerBeLobbyLeaderToDisplay = ERequirementState.DoesNotMatter;
        [SerializeField]
        private ERequirementState m_shouldLocalPlayerBeFriendWithThisPlayerToDisplay = ERequirementState.DoesNotMatter;

        public void UpdateWidgetDisplayStateFor(Friend a_lobbyMember)
        {
            gameObject.SetActive(ShouldBeDisplayedFor(a_lobbyMember));
        }

        private bool ShouldBeDisplayedFor(Friend a_lobbyMember)
        {
            SteamId lobbyOwnerSteamId = default;
            bool shouldLocalPlayerBeLobbyLeaderConditionMet = false;
            switch (m_shouldLocalPlayerBeLobbyLeaderToDisplay)
            {
                case ERequirementState.DoesNotMatter:
                {
                    shouldLocalPlayerBeLobbyLeaderConditionMet = true;
                    break;
                }
                case ERequirementState.ShouldBeTrue:
                {
                    shouldLocalPlayerBeLobbyLeaderConditionMet = Steamworks.SteamClient.SteamId == lobbyOwnerSteamId;
                    break;
                }
                case ERequirementState.ShouldBeFalse:
                {
                    shouldLocalPlayerBeLobbyLeaderConditionMet = Steamworks.SteamClient.SteamId != lobbyOwnerSteamId;
                    break;
                }
            }

            if (!shouldLocalPlayerBeLobbyLeaderConditionMet)
                return false;

            bool shouldLocalPlayerBeFriendWithThisPlayerConditionMet = false;
            switch (m_shouldLocalPlayerBeFriendWithThisPlayerToDisplay)
            {
                case ERequirementState.DoesNotMatter:
                {
                    shouldLocalPlayerBeFriendWithThisPlayerConditionMet = true;
                    break;
                }
                case ERequirementState.ShouldBeTrue:
                {
                    shouldLocalPlayerBeFriendWithThisPlayerConditionMet = a_lobbyMember.IsFriend;
                    break;
                }
                case ERequirementState.ShouldBeFalse:
                {
                    shouldLocalPlayerBeFriendWithThisPlayerConditionMet = !a_lobbyMember.IsFriend;
                    break;
                }
            }
            return shouldLocalPlayerBeFriendWithThisPlayerConditionMet;
        }
    }
}
