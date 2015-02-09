using UnityEngine;

using Engine.Player;

namespace Engine.Components
{
    public class LadderFailWall : MonoBehaviour
    {
        private void OnCollisionEnter(Collision col)
        {
            GamePlayer.Instance.RemoveLadder();
        }
    }
}
