using UnityEngine;

namespace PlayableRunner
{
    public sealed class RunnerGameController : MonoBehaviour
    {
        [Header("Tuning")]
        [SerializeField] private float worldSpeed = 4.0f;
        [SerializeField] private float dtClamp = 0.05f;
        [SerializeField] private float tutorialStopDistance = 3.8f;
        [SerializeField] private float failToEndDelay = 1.0f;

        [Header("Refs")]
        [SerializeField] private Transform worldRoot;
        [SerializeField] private Transform player;
        [SerializeField] private Transform firstObstacle;
        [SerializeField] private Transform finishLine;
        [SerializeField] private RunnerCharacter2D character;
        [SerializeField] private RunnerUI ui;

        [Header("Debug")]
        [SerializeField] private bool debugFastEnd;

        public bool IsSimulating => state == RunnerGameState.Play;
        public float SimTime { get; private set; }
        public float DeltaTime { get; private set; }


        private RunnerGameState state = RunnerGameState.Start;
        private float lastT, failT, speed;
        private bool tutorialDone;
        private int hp = 3, money;


        private void Start()
        {
            speed = 0f;
            lastT = Time.unscaledTime;
            EnterStart();
        }

        private void Update()
        {
            var now = Time.unscaledTime;
            var dt = Mathf.Min(now - lastT, dtClamp);
            lastT = now;
            DeltaTime = dt;

            if (LegacyTapInput.TapDown())
            {
                OnTap();
            }

            if (state == RunnerGameState.Play)
            {
                SimTime += dt;
                worldRoot.position += Vector3.left * (speed * dt);

                if (!tutorialDone && firstObstacle != null && (firstObstacle.position.x - player.position.x) < tutorialStopDistance)
                {
                    EnterTutorial();
                }

                if (debugFastEnd && SimTime > 3f)
                {
                    EnterEnd();
                }
                else if (finishLine != null && finishLine.position.x <= player.position.x + 0.25f)
                {
                    EnterEnd();
                }
            }
            else if (state == RunnerGameState.Fail)
            {
                failT += dt;
                if (failT >= failToEndDelay)
                {
                    EnterEnd();
                }
            }
        }


        private void OnTap()
        {
            if (state == RunnerGameState.Start)
            {
                EnterPlay();
            }
            else if (state == RunnerGameState.Tutorial)
            {
                character.Jump(); ExitTutorialToPlay();
            }
            else if (state == RunnerGameState.Play)
            {
                character.Jump();
            }
        }

        private void EnterStart()
        {
            state = RunnerGameState.Start;
            ui.ShowStart(true);
            ui.ShowJumpHint(false);
            ui.ShowFail(false);
            ui.ShowEndCard(false, 0);
            ui.SetHearts(hp);
            ui.SetBalance(money);
            character.SetRunning(false);
        }

        private void EnterPlay()
        {
            state = RunnerGameState.Play;
            ui.ShowStart(false);
            ui.ShowJumpHint(false);
            speed = worldSpeed;
            character.SetRunning(true);
        }

        private void EnterTutorial()
        {
            tutorialDone = true;
            state = RunnerGameState.Tutorial;
            speed = 0f;
            character.SetRunning(false);
            ui.ShowJumpHint(true);
        }

        private void ExitTutorialToPlay()
        {
            ui.ShowJumpHint(false);
            state = RunnerGameState.Play;
            speed = worldSpeed;
            character.SetRunning(true);
        }


        public void NotifyMoneyPickup(int amount)
        {
            money += amount;
            ui.SetBalance(money);
        }

        public void NotifyObstacleHit()
        {
            if (state == RunnerGameState.End || state == RunnerGameState.Fail)
                return;

            character.PlayHurt();

            hp = Mathf.Max(0, hp - 1);
            ui.SetHearts(hp);

            if (hp <= 0) 
                EnterFail();
        }


        private void EnterFail()
        {
            state = RunnerGameState.Fail;
            speed = 0f;
            failT = 0f;
            character.SetRunning(false);
            ui.ShowFail(true);
        }

        private void EnterEnd()
        {
            state = RunnerGameState.End;
            speed = 0f;
            character.SetRunning(false);
            ui.ShowFail(false);
            ui.ShowEndCard(true, Mathf.Max(99, money * 10));
        }


        public void PauseFromLifecycle()
        {
            speed = 0f;
        }

        public void ResumeFromLifecycle()
        {
            lastT = Time.unscaledTime;
            if (state == RunnerGameState.Play)
                speed = worldSpeed;
        }
    }
}