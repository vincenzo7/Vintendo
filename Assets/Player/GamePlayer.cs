using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Components;
using Engine.Player.States;

namespace Engine.Player
{  
    public class GamePlayer : MonoBehaviour
    {
        [System.Serializable]
        public class Buildables
        {
            public GameObject Ladder;
        }

        [System.Serializable]
        public class RunTimeData
        {
            public GameObject StepPrefab;

            public Vector3 LastBlockPosition = Vector3.zero;

            public Transform LastBlock;

            public Transform RootBuildableNode;

            public bool Left = false;

            public GamePlayerState CurrentState = null;

            public GamePlayerState LastState = null;
        }

        public delegate void StepTargetHit(StepTarget step);

        public static StepTargetHit OnSetTargetHit;
        
        public static GamePlayer Instance;

        public PlayerStates PlayerStates = new PlayerStates();

        public List<KeyValuePair<int, List<GameObject>>> StepSpawnNodes; 

        public Buildables Buildable;

        public RunTimeData Data;

        private void Awake()
        {
            Instance = this;
            GamePlayer.OnSetTargetHit += AutoWalk;
        }

        public virtual void Build()
        {
            Instance.HandleState(Instance.PlayerStates.Build);
        }

        public virtual void TiltStructure()
        {
            Instance.PlayerStates.Build.Active = false;
            Instance.Data.CurrentState = null;
            Instance.HandleState(Instance.PlayerStates.TiltState);
        }

        public virtual void AutoWalk(StepTarget step)
        {
            if (Instance.Data.CurrentState != Instance.PlayerStates.AutoWalk && Instance.Data.LastState == Instance.PlayerStates.TiltState)
            {
                Instance.Data.LastState = Instance.PlayerStates.AutoWalk;
                Instance.PlayerStates.TiltState.Active = false;
                Instance.Data.CurrentState = null;

                Instance.PlayerStates.AutoWalk.TargetStep = step;
                Instance.HandleState(Instance.PlayerStates.AutoWalk);
            }
        }

        public virtual void RemoveLadder()
        {
            Instance.PlayerStates.TiltState.Active = false;
            Instance.Data.CurrentState = null;
            Instance.Data.LastBlockPosition = Vector3.zero;

            Instance.StartCoroutine(Instance.crRemoveLadderUpdate());
        }

        private void HandleState(GamePlayerState state)
        {
            if (Instance.Data.CurrentState == null)
            {
                Instance.Data.CurrentState = state;
                Instance.Data.CurrentState.HandleState(Instance);
            }
        }

        private IEnumerator crRemoveLadderUpdate()
        {
            bool removingLadder = true;

            Transform lastBlock = Instance.Data.LastBlock;
            Transform t;
            while (removingLadder)
            {
                if (lastBlock != null)
                {
                    if (lastBlock.parent != null)
                    {
                        t = lastBlock.parent;
                        Destroy(lastBlock.gameObject);
                        yield return null;
                        lastBlock = t;
                    }
                    else
                    {
                        Destroy(lastBlock.gameObject);
                        removingLadder = false;
                    }
                }
                else
                {
                    removingLadder = false;
                }
                yield return null;
            }
        }
    }
}
