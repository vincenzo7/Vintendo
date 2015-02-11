using UnityEngine;

using Engine.Player;

namespace Engine.Components
{
    public class LadderFailWall : MonoBehaviour
    {
        private void OnCollisionEnter(Collision col)
        {
            if (col.gameObject.tag == "Buildable" && GamePlayer.Instance.Data.CurrentState == null)
            {
                GamePlayer.Instance.RemoveLadder();
            }
        }
    }
}
