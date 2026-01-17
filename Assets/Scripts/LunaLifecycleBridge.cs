using UnityEngine;

namespace PlayableRunner
{
    public sealed class LunaLifecycleBridge : MonoBehaviour
    {
        [SerializeField] private RunnerGameController game;


        private void OnEnable()
        {
#if UNITY_LUNA
            Luna.Unity.LifeCycle.OnPause += OnPaused;
            Luna.Unity.LifeCycle.OnResume += OnResumed;
#endif
        }

        private void OnDisable()
        {
#if UNITY_LUNA
            Luna.Unity.LifeCycle.OnPause -= OnPaused;
            Luna.Unity.LifeCycle.OnResume -= OnResumed;
#endif
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause) OnPaused();
            else OnResumed();
        }


        private void OnPaused() => game.PauseFromLifecycle();
        private void OnResumed() => game.ResumeFromLifecycle();

        public void ClickCta()
        {
#if UNITY_LUNA
            Luna.Unity.Playable.InstallFullGame();
#endif
        }

        public void ReportGameEnded()
        {
#if UNITY_LUNA
            Luna.Unity.LifeCycle.GameEnded();
#endif
        }
    }
}