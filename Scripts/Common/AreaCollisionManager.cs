using System.Collections.Generic;
using Godot;

namespace StormTime.Common
{
    public class AreaCollisionManager : Node
    {
        [Export] public PackedScene collisionCheckerPrefab;
        [Export] public int initialCount;

        private List<AreaCollisionChecker> _pooledCollisionCheckers;

        public override void _Ready()
        {
            if (instance == null)
            {
                instance = this;
            }

            _pooledCollisionCheckers = new List<AreaCollisionChecker>();
            for (int i = 0; i < initialCount; i++)
            {
                CreateAndAddNewChecker();
            }
        }

        #region External Functions

        public AreaCollisionChecker GetAreaCollisionChecker()
        {
            AreaCollisionChecker areaCollisionChecker = GetFreeCollisionChecker();
            areaCollisionChecker.SetInUse();

            return areaCollisionChecker;
        }

        public void ReturnCollisionChecker(AreaCollisionChecker areaCollisionChecker) => areaCollisionChecker.SetFree();

        #endregion

        #region Utility Functions

        private AreaCollisionChecker GetFreeCollisionChecker()
        {
            for (int i = 0; i < _pooledCollisionCheckers.Count; i++)
            {
                if (!_pooledCollisionCheckers[i].IsCollisionCheckerInUse())
                {
                    return _pooledCollisionCheckers[i];
                }
            }

            CreateAndAddNewChecker();
            return _pooledCollisionCheckers[_pooledCollisionCheckers.Count - 1];
        }

        private void CreateAndAddNewChecker()
        {
            AreaCollisionChecker collisionCheckerInstance = (AreaCollisionChecker)collisionCheckerPrefab.Instance();
            GetParent().CallDeferred("add_child", collisionCheckerInstance);

            _pooledCollisionCheckers.Add(collisionCheckerInstance);
        }

        #endregion

        #region Singleton

        public static AreaCollisionManager instance;

        #endregion
    }
}