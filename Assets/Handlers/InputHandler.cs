using System.Linq;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Engine.Player;
using Engine.Commands;

namespace Engine.Handlers
{
    public class InputHandler : MonoBehaviour
    {
        public static InputHandler Instance;

        public List<KeyValuePair<KeyCode, Command>> Commands;

        private void Awake()
        {
            Instance = this;
            Commands = new List<KeyValuePair<KeyCode, Command>>()
            {
                new KeyValuePair<KeyCode, Command>(KeyCode.A, new BuildCommand()),
                new KeyValuePair<KeyCode, Command>(KeyCode.S, new TiltCommand())
            };
        }

        public IEnumerator HandleInput()
        {
            while (Instance.gameObject.activeInHierarchy)
            {
                SAPlatformInput();
                IPhonePlatformInput();
                AndroidPlatformInput();

                yield return null;
            }
        }

        public void Activate()
        {
            Instance.gameObject.SetActive(true);
        }

        public void Deactivate()
        {
            Instance.gameObject.SetActive(false);
        }

        public void StartWatchingInput()
        {
            Instance.StartCoroutine(Instance.HandleInput());
        }

        private void FireCommand(KeyCode kc)
        {
            Instance.Commands.First(c => c.Key == kc).Value.Execute(GamePlayer.Instance);
        }

        [Conditional("UNITY_EDITOR_WIN"), Conditional("UNITY_EDITOR_OSX"), Conditional("UNITY_STANDALONE")]
        private void SAPlatformInput()
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                FireCommand(KeyCode.A);
            }
            if (Input.GetKeyUp(KeyCode.S))
            {
                FireCommand(KeyCode.S);
            }
        }

        [Conditional("UNITY_IPHONE")]
        private void IPhonePlatformInput()
        {
        }

        [Conditional("UNITY_ANDROID")]
        private void AndroidPlatformInput()
        {
        }
    }
}
