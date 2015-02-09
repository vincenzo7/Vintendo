using UnityEngine;

using Engine.Player;

namespace Engine.Components
{
    public class StepTarget : MonoBehaviour
    {
        private void OnCollisionEnter(Collision col)
        {
            if (col.gameObject.tag != "Fail Wall")
            {
                if (GamePlayer.Instance.Data.LastState != GamePlayer.Instance.PlayerStates.AutoWalk)
                {
                    GamePlayer.Instance.Data.LastState = GamePlayer.Instance.PlayerStates.TiltState;
                    GamePlayer.OnSetTargetHit(this);
                }
            }
        }
    }
}
