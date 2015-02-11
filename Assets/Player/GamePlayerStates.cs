using System.Collections;
using System.Configuration;
using System.Xml.Serialization;
using Engine.Components;
using UnityEngine;

namespace Engine.Player.States
{
    public class GamePlayerState
    {
        public bool Active = false;

        public virtual void HandleState(GamePlayer player)
        {
        }
    }

    public class BuildStateSettings
    {
        public float CoolDown; 
     
        public BuildStateSettings(float coolDown = 0.1f)
        {
            CoolDown = coolDown;
        }
    }

    public class BuildState : GamePlayerState
    {     
        private Transform StructureRoot;

        private BuildStateSettings Settings;

        public BuildState(BuildStateSettings bss)
        {
            Settings = bss;
        }

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

                yield return new WaitForSeconds(Settings.CoolDown);
            }

            player.Data.LastState = this;
            player.Data.CurrentState = null;
        }
    }

    public class TiltStructurStateSettings
    {
        public float RotationSpeed;

        public TiltStructurStateSettings(float rotationSpeed = 2f)
        {
            RotationSpeed = rotationSpeed;
        }
    }

    public class TiltStructureState : GamePlayerState
    {       
        private TiltStructurStateSettings Settings;

        public TiltStructureState(TiltStructurStateSettings settings)
        {
            Settings = settings;
        }

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
                    player
                        .Data
                        .RootBuildableNode
                        .Rotate(
                            0, 
                            0, 
                            (player.Data.Left) 
                                ? Settings.RotationSpeed 
                                : -Settings.RotationSpeed);
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

    public class AutoWalkStateSettings
    {
        public float xMax;
        public float xMin;
        public float yMax;
        public float yMin;
        public float Speed;

        public AutoWalkStateSettings(float xmax = 15f, float xmin = 5f, float ymax = 10f, float ymin = 5f, float speed = 5f)
        {
            xMax = xmax;
            xMin = xmin;
            yMax = ymax;
            yMin = ymin;
            Speed = speed;
        }
    }

    public class AutoWalkState : GamePlayerState
    {
        public StepTarget TargetStep;

        private AutoWalkStateSettings Settings;

        public AutoWalkState(AutoWalkStateSettings settings)
        {
            Settings = settings;
        }

        public override void HandleState(GamePlayer player)
        {
            GameObject newStep =
                GameObject.Instantiate(
                    player.Data.StepPrefab,
                    new Vector3(
                        TargetStep.transform.position.x + ((
                                ((Random.Range(0, 4) % 2 == 0) ? -1 : 1) 
                                * Random.Range(Settings.xMin, Settings.xMax)
                            )),
                        TargetStep.transform.position.y + (Random.Range(Settings.yMin, Settings.yMax)),
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
                    Settings.Speed * Time.deltaTime);

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
        public BuildState Build = new BuildState(
            new BuildStateSettings(
                coolDown: 0.1f
            ));

        public TiltStructureState TiltState = new TiltStructureState(
            new TiltStructurStateSettings(
                rotationSpeed: 2f
            ));

        public AutoWalkState AutoWalk = new AutoWalkState(
            new AutoWalkStateSettings(
                xmax: 15f, 
                xmin: 5f, 
                ymax: 10f,
                ymin: 5f, 
                speed: 15f
            ));

        public PlayerStates(BuildState bs, TiltStructureState ts, AutoWalkState aw)
        {
            Build = bs;
            TiltState = ts;
            AutoWalk = aw;
        }
    }
}
