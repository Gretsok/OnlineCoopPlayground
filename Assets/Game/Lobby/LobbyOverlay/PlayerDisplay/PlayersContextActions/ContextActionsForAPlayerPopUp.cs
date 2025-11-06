using System;
using System.Collections;
using System.Collections.Generic;
using Steamworks;
using Tools.UIManagement;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Lobby.LobbyOverlay.PlayerDisplay.PlayersContextActions
{
    public class ContextActionsForAPlayerPopUp : CrossRequestedPanel
    {
        [SerializeField]
        private List<ContextActionWidget> m_contextActionWidgets;
        public IReadOnlyList<ContextActionWidget> ContextActionWidgets => m_contextActionWidgets;
        
        public Friend ContextualizedMember { get; private set; }


        private void Start()
        {
            m_contextActionWidgets.ForEach(a_widget =>
            {
                a_widget.OnSelectEvent += HandleWidgetSelected;
                a_widget.OnDeselectEvent += HandleWidgetDeselected;
            });
        }
        
        private void OnDestroy()
        {
            m_contextActionWidgets.ForEach(a_widget =>
            {
                a_widget.OnSelectEvent -= HandleWidgetSelected;
                a_widget.OnDeselectEvent -= HandleWidgetDeselected;
            });
        }

        private void HandleWidgetSelected(ContextActionWidget a_arg1, BaseEventData a_arg2)
        {
            StartRequest(a_arg1);
        }

        private void HandleWidgetDeselected(ContextActionWidget a_arg1, BaseEventData a_arg2)
        {
            StopRequest(a_arg1);
        }

        public void SetAssociatedLobbyMember(Friend a_lobbyMember)
        {
            ContextualizedMember = a_lobbyMember;
            m_contextActionWidgets.ForEach(a_widget => a_widget.UpdateWidgetDisplayStateFor(a_lobbyMember));
        }


        protected override IEnumerator HandleShow()
        {
            yield return new WaitForEndOfFrame();
            yield return base.HandleShow();
        }

        protected override IEnumerator HandleHide()
        {
            yield return new WaitForEndOfFrame();
            yield return base.HandleHide();
        }
    }
}
