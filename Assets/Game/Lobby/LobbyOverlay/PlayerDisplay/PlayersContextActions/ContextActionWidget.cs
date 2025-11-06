using System;
using Steamworks;
using Tools.UIManagement;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Lobby.LobbyOverlay.PlayerDisplay.PlayersContextActions
{
    public class ContextActionWidget : MonoBehaviour, ICrossPanelRequester,
        ISelectHandler, IDeselectHandler
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
        [SerializeField]
        private ERequirementState m_shouldThePlayerBeLeaderToDisplay = ERequirementState.DoesNotMatter;

        public Friend AssociatedLobbyMember { get; private set; }
        
        public void UpdateWidgetDisplayStateFor(Friend a_lobbyMember)
        {
            gameObject.SetActive(ShouldBeDisplayedFor(a_lobbyMember));
            
            AssociatedLobbyMember = a_lobbyMember;
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
            if (!shouldLocalPlayerBeFriendWithThisPlayerConditionMet)
                return false;
            
            bool shouldThePlayerBeLeaderConditionMet = false;
            var lobby = LobbyManager.Instance.Lobby;
            switch (m_shouldThePlayerBeLeaderToDisplay)
            {
                case ERequirementState.DoesNotMatter:
                {
                    shouldThePlayerBeLeaderConditionMet = true;
                    break;
                }
                case ERequirementState.ShouldBeTrue:
                {
                    shouldThePlayerBeLeaderConditionMet = lobby.IsOwnedBy(a_lobbyMember.Id);
                    break;
                }
                case ERequirementState.ShouldBeFalse:
                {
                    shouldThePlayerBeLeaderConditionMet = !lobby.IsOwnedBy(a_lobbyMember.Id);
                    break;
                }
            }
            
            return shouldThePlayerBeLeaderConditionMet;
        }

        public event Action<ContextActionWidget, BaseEventData> OnSelectEvent;
        public event Action<ContextActionWidget, BaseEventData> OnDeselectEvent;
        public void OnSelect(BaseEventData a_eventData)
        {
            OnSelectEvent?.Invoke(this, a_eventData);
        }

        public void OnDeselect(BaseEventData a_eventData)
        {
            OnDeselectEvent?.Invoke(this, a_eventData);
        }
    }
}
