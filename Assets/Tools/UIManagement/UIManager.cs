using System.Collections.Generic;
using UnityEngine;

namespace Tools.UIManagement
{
    public class UIManager : MonoBehaviour
    {
        #region Singleton
        public static UIManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance)
                Destroy(Instance.gameObject);
            Instance = this;
            
            m_registeredPanels.ForEach(a_panel => a_panel.Init());
        }
        #endregion
        
        [SerializeField]
        private List<Panel> m_registeredPanels = new List<Panel>();

        public TPanel GetPanel<TPanel>() where TPanel : Panel
        {
            return m_registeredPanels.Find(a_panel => a_panel is TPanel) as TPanel;
        }
    }
}
