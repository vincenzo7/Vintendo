using System.Security.Permissions;
using Engine.Handlers;

namespace Engine.Game.States
{
    public class TheGame : GameState
    {
        [System.Serializable]
        public class GameHandlers
        {
            public InputHandler InputHandler;
        }

        public static TheGame Instance;

        public GameHandlers Handlers;

        private void Awake()
        {
            Instance = this;
        }

        public override void StartState(Game game)
        {
            Instance.Game = game;

            Instance.Handlers.InputHandler.Activate();
            Instance.Handlers.InputHandler.StartWatchingInput();
        }
    }
}
