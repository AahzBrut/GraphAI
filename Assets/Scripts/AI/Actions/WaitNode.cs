using BehaviourTree.Nodes;
using UnityEngine;

namespace AI.Actions
{
    public class WaitNode : ActionNode
    {
        public float delay;
        private float _timeElapsed;
        
        protected override void OnStart()
        {
            _timeElapsed = 0;
            Debug.Log($"Start waiting for {delay} seconds");
        }

        protected override void OnStop()
        {
            Debug.Log("Wait complete");
        }

        protected override State OnUpdate()
        {
            _timeElapsed += Time.deltaTime;
            return _timeElapsed < delay ? State.Running : State.Success;
        }
    }
}