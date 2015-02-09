using System.Collections;
using Engine.Components;
using UnityEngine;

namespace Engine.Player.States
{
    public class GamePlayerState
    {
        public virtual void HandleState(GamePlayer player)
        {
        }
    }

    public class BuildState : GamePlayerState
    {
        public bool Active = false;

        private float CoolDown = 0.1f;

        private Transform StructureRoot;

        public override void HandleState(GamePlayer player)
        {

            if (!Active)
            {
                Active = true;
                player.StartCoroutine(crBuildUpdate(player));
            }
        }

        private IEnumerator crBuildUpdate(GamePlayer player)
        {
            while (Active)
            {
                if (player.Data.LastBlockPosition != Vector3.zero)
                {
                    player.Data.LastBlockPosition = new Vector3(
                        player.Data.LastBlockPosition.x,
                        player.Data.LastBlockPosition.y + 1,
                        player.Data.LastBlockPosition.z);
                }
                else
                {
                    player.Data.LastBlockPosition = player.transform.position + player.transform.up;
                }

                GameObject newBuildable =
                GameObject.Instantiate(
                    player.Buildable.Ladder,
                    player.Data.LastBlockPosition,
                    Quaternion.identity)
                as GameObject;

                yield return null;

                if (StructureRoot == null)
                {
                    StructureRoot = newBuildable.transform;
                    player.Data.RootBuildableNode = StructureRoot;
                }
                else
                {
                    newBuildable.transform.parent = StructureRoot;
                    StructureRoot = newBuildable.transform;
                    player.Data.LastBlock = StructureRoot;
                }

                yield return new WaitForSeconds(CoolDown);
            }

            player.Data.LastState = this;
            player.Data.CurrentState = null;
        }
    }

    public class TiltStructureState : GamePlayerState
    {
        public bool Active = false;

        private float StateDuration = 0f;

        public override void HandleState(GamePlayer player)
        {
            if (!Active)
            {
                Active = !Active;
                player.StartCoroutine(crTiltUpdate(player));
            }
        }

        private IEnumerator crTiltUpdate(GamePlayer player)
        {
            while (Active)
            {
                if (player.Data.RootBuildableNode != null)
                {
                    player.Data.RootBuildableNode.Rotate(0, 0, (player.Data.Left) ? 2 : -2);
                }
                else
                {
                    player.Data.CurrentState = null;
                    player.Data.LastBlockPosition = Vector3.zero;
                    Active = false;
                }
                yield return null;
            }
        }
    }

    public class AutoWalkState : GamePlayerState
    {
        public StepTarget TargetStep;

        public override void HandleState(GamePlayer player)
        {
            GameObject newStep =
                GameObject.Instantiate(
                    player.Data.StepPrefab,
                    new Vector3(
                        TargetStep.transform.position.x + ((
                                ((Random.Range(0, 4) % 2 == 0) ? -1 : 1) * Random.Range(5f, 15f)
                            )),
                        TargetStep.transform.position.y + (Random.Range(5f, 10f)),
                        TargetStep.transform.position.z),
                    Quaternion.identity) as GameObject;

            player.Data.Left = (newStep.transform.position.x > TargetStep.transform.position.x)
                ? false
                : true;

            player.StartCoroutine(crAutoWalkUpdate(player));
        }

        private IEnumerator crAutoWalkUpdate(GamePlayer player)
        {
            bool walking = true;

            Vector3 pos = TargetStep.transform.position + TargetStep.transform.up;

            while (walking)
            {
                player.transform.position = Vector3.MoveTowards(
                    player.transform.position,
                    pos,
                    10f * Time.deltaTime);

                yield return null;

                if (Vector3.Distance(player.transform.position, pos) < 0.1f)
                {
                    walking = false;
                    player.RemoveLadder();
                }

                yield return null;
            }

            player.Data.LastState = this;
            player.Data.CurrentState = null;
            player.Data.LastBlockPosition = Vector3.zero;
        }
    }

    public class PlayerStates
    {
        public BuildState Build = new BuildState();
        public TiltStructureState TiltState = new TiltStructureState();
        public AutoWalkState AutoWalk = new AutoWalkState();
    }
}
