using BehaviourTree.Nodes;
using UnityEngine;

namespace AI.Actions
{
    public class DebugLogNode : ActionNode
    {
        public string message;
        
        protected override void OnStart()
        {
            Debug.Log($"Message from DebugLogNode OnStart: {message}");
        }

        protected override void OnStop()
        {
            Debug.Log($"Message from DebugLogNode OnStop: {message}");
        }

        protected override State OnUpdate()
        {
            Debug.Log($"Message from DebugLogNode OnUpdate: {message}");
            return State.Success;
        }
    }
}