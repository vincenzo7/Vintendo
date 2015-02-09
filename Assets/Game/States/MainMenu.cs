using System.Collections;
using UnityEngine;

namespace Engine.Game.States
{
    public class MainMenu : GameState
    {
        [System.Serializable]
        public class NextPossibleStates
        {
            public TheGame TheGameState;
        }

        public static MainMenu Instance;

        public NextPossibleStates PossibleStates;

        private void Awake()
        {
            Instance = this;
        }

        public override void StartState(Game game)
        {
            Instance.Game = game;

            StartCoroutine(crTempWatcher());
        }

        public void GoToGameState()
        {
            Instance.NextState = Instance.PossibleStates.TheGameState;
            Instance.Game.GoToNextState();
        }

        private IEnumerator crTempWatcher()
        {
            while (Instance.gameObject.activeInHierarchy)
            {
                if (Input.GetKeyDown(KeyCode.D))
                {
                    Instance.GoToGameState();
                }
                yield return null;
            }
        }
    }
}
