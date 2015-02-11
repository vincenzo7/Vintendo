using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.Game.States
{
    public class PostMenu : GameState
    {
        public override void StartState(Game game)
        {
            Game = game;
        }

        public void SendHighScore(int score)
        {
            int id = 1;

        }

        private IEnumerator crSendHighScore()
        {
            yield return new WWW(
                "url", 
                new System
                    .Text
                    .UTF8Encoding()
                    .GetBytes("data"), 
                new Dictionary<string, string>()
                {
                    {"Content-Type", "text/json"},
                    {"Content-Length", "1"}
                });
        } 
    }
}
