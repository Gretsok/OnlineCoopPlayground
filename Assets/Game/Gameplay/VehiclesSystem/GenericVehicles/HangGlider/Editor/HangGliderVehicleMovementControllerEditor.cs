using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Game.Gameplay.VehiclesSystem.GenericVehicles.HangGlider.Editor
{
    [CustomEditor(typeof(HangGliderVehicleMovementController))]
    public class HangGliderVehicleMovementControllerEditor : UnityEditor.Editor
    {
        private Label m_horizontalValueLabel;
        private Label m_verticalValueLabel;

        
        public override VisualElement CreateInspectorGUI()
        {
            var castedTarget = (HangGliderVehicleMovementController)target;

            var root = new VisualElement();

            var preexistingInspector = new VisualElement();
            InspectorElement.FillDefaultInspector(preexistingInspector, serializedObject, this);
            root.Add(preexistingInspector);

            root.Add(new VisualElement { style = { height = 30 } });
            
            m_horizontalValueLabel = new Label($"Horizontal Input Value: {castedTarget.HorizontalValue}");
            m_verticalValueLabel = new Label($"Vertical Input Value: {castedTarget.VerticalValue}");

            root.Add(m_horizontalValueLabel);
            root.Add(m_verticalValueLabel);
            
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
            var castedTarget = (HangGliderVehicleMovementController)target;

            if (m_horizontalValueLabel != null)
                m_horizontalValueLabel.text = $"Horizontal Input Value: {castedTarget.HorizontalValue}";
            if (m_verticalValueLabel != null)
                m_verticalValueLabel.text = $"Vertical Input Value: {castedTarget.VerticalValue}";
        }
    }
}