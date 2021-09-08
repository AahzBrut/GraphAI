using UnityEngine.UIElements;

namespace BehaviourTree.Editor
{
    public class InspectorView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }

        private UnityEditor.Editor _editor;
        
        public InspectorView() { }

        public void UpdateSelection(NodeView nodeView)
        {
            Clear();
            
            UnityEngine.Object.DestroyImmediate(_editor);
            _editor = UnityEditor.Editor.CreateEditor(nodeView.Node);
            var container = new IMGUIContainer(OnInspectorGUI);
            Add(container);
        }

        private void OnInspectorGUI()
        {
            if (_editor.target == null) return;
            _editor.OnInspectorGUI();
        }
    }
}