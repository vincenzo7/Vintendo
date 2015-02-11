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

        [System.Serializable]
        public class StateSettings
        {
            public float BuildStateCooldown;

            public float TiltStateRotationSpeed;

            public float AutoWalkXMax;
            public float AutoWalkXMin;
            public float AutoWalkYMax;
            public float AutoWalkYMin;

            public float Speed;
        }

        public delegate void StepTargetHit(StepTarget step);

        public static StepTargetHit OnSetTargetHit;
        
        public static GamePlayer Instance;

        public PlayerStates PlayerStates;

        public StateSettings PlayerStateSettings;

        public Buildables Buildable;

        public RunTimeData Data;

        private void Awake()
        {
            Instance = this;
            GamePlayer.OnSetTargetHit += Instance.AutoWalk;
            Instance.PlayerStates = Instance.InitPlayerStates();
        }

        public virtual void Build()
        {
            Instance.HandleState(Instance.PlayerStates.Build);
        }

        public virtual void TiltStructure()
        {
            Instance.DeactivateState(Instance.PlayerStates.Build);
            Instance.HandleState(Instance.PlayerStates.TiltState);
        }

        public virtual void AutoWalk(StepTarget step)
        {
            if (Instance.Data.CurrentState != Instance.PlayerStates.AutoWalk && Instance.Data.LastState == Instance.PlayerStates.TiltState)
            {
                Instance.Data.LastState = Instance.PlayerStates.AutoWalk;
                Instance.DeactivateState(Instance.PlayerStates.TiltState);
                Instance.PlayerStates.AutoWalk.TargetStep = step;
                Instance.HandleState(Instance.PlayerStates.AutoWalk);
            }
        }

        public virtual void RemoveLadder()
        {
            Instance.DeactivateState(Instance.PlayerStates.TiltState);
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

        private void DeactivateState(GamePlayerState state) 
        {
            state.Active = false;
            Instance.Data.CurrentState = null;
        }

        private PlayerStates InitPlayerStates()
        {
            return new PlayerStates(
                new BuildState(
                    new BuildStateSettings(
                      coolDown: Instance.PlayerStateSettings.BuildStateCooldown
                )),
                new TiltStructureState(
                    new TiltStructurStateSettings(
                        rotationSpeed: Instance.PlayerStateSettings.TiltStateRotationSpeed
                )),
                new AutoWalkState(
                    new AutoWalkStateSettings(
                        xmax:  Instance.PlayerStateSettings.AutoWalkXMax,
                        xmin:  Instance.PlayerStateSettings.AutoWalkXMin,
                        ymax:  Instance.PlayerStateSettings.AutoWalkYMax,
                        ymin:  Instance.PlayerStateSettings.AutoWalkYMin,
                        speed: Instance.PlayerStateSettings.Speed
                    ))
                );
        }
    }
}
