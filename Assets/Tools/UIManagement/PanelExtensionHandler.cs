using System;
using UnityEngine;

namespace Tools.UIManagement
{
    
    public class PanelExtensionHandler : MonoBehaviour
    {
        [Header("Panel Extension")]
        [SerializeField]
        private Panel m_panel;

        private void Awake()
        {
            if (!m_panel)
                m_panel = GetComponent<Panel>();
            if (!m_panel)
                return;

            m_panel.OnShown += HandlePanelShown;
            m_panel.OnHidden += HandlePanelHidden;
        }

        private void OnDestroy()
        {
            if (!m_panel)
                return;
            
            m_panel.OnShown -= HandlePanelShown;
            m_panel.OnHidden -= HandlePanelHidden;
        }

        protected virtual void HandlePanelShown(Panel a_panel)
        { }

        protected virtual void HandlePanelHidden(Panel a_panel)
        { }
    }
}
