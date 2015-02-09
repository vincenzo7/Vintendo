using System.Linq;
using System.Collections.Generic;

using UnityEngine;

using Engine.Game.States;

namespace Engine.Game
{
    public class Game : MonoBehaviour
    {
        public static Game Instance;

        public List<GameState> GameStates;

        public GameState CurrentState;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            CurrentState = GameStates.First();
            Instance.StartState();
        }

        public void GoToNextState()
        {
            DeactivateState();
            CurrentState = CurrentState.NextState;
            StartState();
        }

        private void ActivateState()
        {
            CurrentState.Activate();
        }

        private void DeactivateState()
        {
            CurrentState.Deactivate();
        }

        private void StartState()
        {
            Instance.ActivateState();
            Instance.CurrentState.StartState(Instance);
        }
    }
}
