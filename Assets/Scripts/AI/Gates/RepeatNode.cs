using BehaviourTree.Nodes;

namespace AI.Gates
{
    public class RepeatNode : GateNode
    {
        public int count = 5;

        protected override void OnStart()
        {
        }

        protected override void OnStop()
        {
        }

        protected override State OnUpdate()
        {
            childNode.Update();
            return --count > 0 ? State.Running : State.Success;
        }
    }
}