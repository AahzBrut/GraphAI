using UnityEngine;

namespace BehaviourTree
{
    public class BehaviourTreeRunner : MonoBehaviour
    {
        [SerializeField] private BehaviourTree behaviourTree;

        public BehaviourTree CurrentTree => behaviourTree;
        
        private void Start()
        {
            behaviourTree = behaviourTree.Clone();
            behaviourTree.BindContext();
        }

        private void Update()
        {
            behaviourTree.Update();
        }
    }
}