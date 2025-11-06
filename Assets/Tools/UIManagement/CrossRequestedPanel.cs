using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tools.UIManagement
{
    public interface ICrossPanelRequester
    {}
    
    public class CrossRequestedPanel : MonoBehaviour
    {
        private readonly List<ICrossPanelRequester> m_requesters = new();

        public void StartRequest(ICrossPanelRequester a_requester)
        {
            if (!m_requesters.Contains(a_requester))
                m_requesters.Add(a_requester);
            UpdateDisplayState();
        }

        public void StopRequest(ICrossPanelRequester a_requester)
        {
            m_requesters.RemoveAll(a_req => a_req == a_requester);
            UpdateDisplayState();
        }

        private void UpdateDisplayState()
        {
            if (m_requesters.Count == 0 && IsShown)
                Hide();
            else if (m_requesters.Count > 0 && !IsShown)
                Show();
        }

        public bool IsShown { get; private set; }
        private Coroutine m_displayStateUpdateCoroutine;
        private void Show()
        {
            if (m_displayStateUpdateCoroutine != null)
            {
                UIManager.Instance.StopCoroutine(m_displayStateUpdateCoroutine);
            }
            
            m_displayStateUpdateCoroutine = UIManager.Instance.StartCoroutine(HandleShow());
            IsShown = true;
        }

        protected virtual IEnumerator HandleShow()
        {
            gameObject.SetActive(true);
            yield break;
        }

        private void Hide()
        {
            if (m_displayStateUpdateCoroutine != null)
            {
                UIManager.Instance.StopCoroutine(m_displayStateUpdateCoroutine);
            }
            
            m_displayStateUpdateCoroutine = UIManager.Instance.StartCoroutine(HandleHide());
            IsShown = false;
        }

        protected virtual IEnumerator HandleHide()
        {
            gameObject.SetActive(false);
            yield break;
        }
    }
}
