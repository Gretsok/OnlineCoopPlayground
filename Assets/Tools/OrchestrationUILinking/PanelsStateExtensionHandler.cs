using System.Collections.Generic;
using Tools.Orchestration;
using Tools.UIManagement;
using UnityEngine;

namespace Tools.OrchestrationUILinking
{
    public class PanelsStateExtensionHandler : OrchestrationStateExtensionHandler
    {
        [SerializeField]
        private List<Panel> m_panels = new List<Panel>();

        protected override void HandleStateEntered(OrchestrationState a_state)
        {
            base.HandleStateEntered(a_state);
            m_panels.ForEach(a_panel => a_panel.Show());
        }

        protected override void HandleStateLeft(OrchestrationState a_state)
        {
            base.HandleStateLeft(a_state);
            m_panels.ForEach(a_panel => a_panel.Hide());
        }
    }
}
