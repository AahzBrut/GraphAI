using System;
using BehaviourTree.Nodes;

namespace AI.Selectors
{
    public class SequencerNode : SelectorNode
    {
        private int _currentIndex;
        
        protected override void OnStart()
        {
            _currentIndex = 0;
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            var childState = childNodes[_currentIndex].Update();

            switch (childState)
            {
                case State.Failure:
                    return State.Failure;
                case State.Running:
                    return State.Running;
                case State.Success:
                    _currentIndex++;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return _currentIndex == childNodes.Count ? State.Success : State.Running;
        }
    }
}