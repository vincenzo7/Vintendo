using UnityEngine;

namespace Engine.Game.States
{
    public class GameState : MonoBehaviour
    {
        public GameState NextState;

        protected Game Game;

        public virtual void StartState(Game game)
        {
            Game = game;
        }

        public void Activate()
        {
            gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            gameObject.SetActive(false);
        }

        public void ChangeToNextState()
        {
            Game.GoToNextState();
        }
    }
}
