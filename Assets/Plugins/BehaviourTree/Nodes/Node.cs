using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree.Nodes
{
    public abstract class Node : ScriptableObject
    {
        public enum State
        {
            Running,
            Failure,
            Success
        }

        [HideInInspector] public string guid;
        [HideInInspector] public State state = State.Running;
        [HideInInspector] public Vector2 position;
        [HideInInspector] public TreeContext treeContext;
        [TextArea] public string description;
        
        private bool _started;

        public bool IsStarted => _started;

        public State Update()
        {
            if (!_started)
            {
                OnStart();
                _started = true;
            }

            state = OnUpdate();

            if (state != State.Running)
            {
                OnStop();
                _started = false;
            }

            return state;
        }

        public virtual Node Clone()
        {
            return Instantiate(this);
        }

        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract State OnUpdate();
        
        public abstract void AddChild(Node child);

        public abstract void RemoveChild(Node child);

        public abstract List<Node> GetChildren();
    }
}
