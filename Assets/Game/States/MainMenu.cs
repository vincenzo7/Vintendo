using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public List<UILabel> HighScores; 

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
            StartCoroutine(crGoToGameStateDelay());
        }

        private IEnumerator crGoToGameStateDelay()
        {
            yield return new WaitForSeconds(0.5f);

            Instance.NextState = Instance.PossibleStates.TheGameState;
            Instance.Game.GoToNextState();
        }

        public void GetHighScores()
        {
            StartCoroutine(crGetHighScores());
        }

        private IEnumerator crGetHighScores()
        {
            WWW scores = new WWW("http://localhost:15263/Vintendo/HighScores?format=json");

            yield return scores;

            string json = scores.text;
            List<string> dataa = new List<string>();
            List<string> parsedData = new List<string>();
            string data = json.Substring(
                json.IndexOf('[') + 1, 
                (json.IndexOf(']') - json.IndexOf('[')) - 1);
            var listedData = data.Split(',');
            listedData.ToList().ForEach(ld =>
            {
                ld = ld.Trim('{', '}');
                dataa.Add(ld);
            });

            for (int i = 0; i < dataa.Count; i += 2)
            {
                parsedData.Add(string.Format("{0} : {1}", dataa[i].Split(':')[1], dataa[i+1].Split(':')[1]));
            }

            parsedData.Take(HighScores.Count).ToList().ForEach(pd =>
            {
                HighScores[parsedData.IndexOf(pd)].text = pd;
            });
            
            // Fill UI with parsedData
        }

        private IEnumerator crTempWatcher()
        {
            while (Instance.gameObject.activeInHierarchy)
            {
                if (Input.GetKeyDown(KeyCode.D))
                {
                    Instance.GoToGameState();
                }
                if (Input.GetKeyDown(KeyCode.S))
                {
                    yield return StartCoroutine(crGetHighScores());
                }
                yield return null;
            }
        }
    }
}
