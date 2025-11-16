using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Game.Gameplay.GameplayInteractionsSystems.HealthHandling.Editor
{
    [CustomEditor(typeof(HealthController))]
    public class HealthControllerEditor : UnityEditor.Editor
    {
        private Label m_healthText;
        private Label m_maxHealthText;

        public override VisualElement CreateInspectorGUI()
        {
            var castedTarget = (HealthController)target;

            var root = new VisualElement();

            var preexistingInspector = new VisualElement();
            InspectorElement.FillDefaultInspector(preexistingInspector, serializedObject, this);
            root.Add(preexistingInspector);

            root.Add(new VisualElement { style = { height = 30 } });
            
            m_healthText = new Label($"Current Health : {castedTarget.CurrentHealth}");
            m_maxHealthText = new Label($"Max Health : {castedTarget.MaxHealth}");
            
            
            root.Add(m_healthText);
            root.Add(m_maxHealthText);
            
            return root;
        }

        private void OnEnable()
        {
            EditorApplication.update += HandleEditorUpdate;
        }

        private void OnDisable()
        {
            EditorApplication.update -= HandleEditorUpdate;
        }

        private void HandleEditorUpdate()
        {
            var castedTarget = (HealthController)target;

            if (m_healthText != null)
                m_healthText.text = $"Current Health : {castedTarget.CurrentHealth}";
            if (m_maxHealthText != null)
                m_maxHealthText.text = $"Max Health : {castedTarget.MaxHealth}";
        }
    }
}