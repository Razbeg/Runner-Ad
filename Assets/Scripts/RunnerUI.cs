using System;
using UnityEngine;
using UnityEngine.UI;

namespace PlayableRunner
{
    public sealed class RunnerUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject startPrompt;
        [SerializeField] private GameObject jumpHint;
        [SerializeField] private GameObject failOverlay;
        [SerializeField] private GameObject endCard;

        [Header("HUD")]
        [SerializeField] private Image[] hearts;
        [SerializeField] private Text balanceText;

        [Header("End Card")]
        [SerializeField] private Text rewardText;
        [SerializeField] private Text endTitleText;
        [SerializeField] private Text endSubText;
        [SerializeField] private Button ctaButton;

        [Header("Reward Count Up")]
        [SerializeField] private float rewardCountDuration = 0.75f;
        [SerializeField] private bool rewardUseUnscaledTime = true;

        private bool rewardAnimating;
        private int rewardFrom;
        private int rewardTo;
        private float rewardStartTime;

        public event Action CtaClicked;


        private void Awake()
        {
            if (ctaButton != null)
                ctaButton.onClick.AddListener(() => CtaClicked?.Invoke());
        }

        private void Update()
        {
            if (!rewardAnimating)
                return;

            var now = rewardUseUnscaledTime ? Time.unscaledTime : Time.time;
            var t = (now - rewardStartTime) / Mathf.Max(0.0001f, rewardCountDuration);
            if (t >= 1f)
            {
                rewardAnimating = false;
                SetRewardText(rewardTo);
                return;
            }

            var eased = 1f - (1f - t) * (1f - t);
            var value = Mathf.RoundToInt(Mathf.Lerp(rewardFrom, rewardTo, eased));
            SetRewardText(value);
        }



        public void SetHearts(int hp)
        {
            for (var i = 0; i < hearts.Length; i++)
                hearts[i].enabled = i < hp;
        }

        public void SetBalance(int amount)
        {
            if (balanceText != null)
                balanceText.text = "$" + amount;
        }

        public void ShowStart(bool on)
        {
            if (startPrompt)
                startPrompt.SetActive(on);
        }

        public void ShowJumpHint(bool on)
        {
            if (jumpHint)
                jumpHint.SetActive(on);
        }

        public void ShowFail(bool on)
        {
            if (failOverlay)
                failOverlay.SetActive(on);
        }

        public void ShowEndCard(bool on, bool finished, int reward)
        {
            if (endCard)
                endCard.SetActive(on);

            rewardAnimating = false;

            if (!on)
                return;

            if (endTitleText != null)
                endTitleText.text = finished ? "Congratulations!" : "You didn't make it!";

            if (endSubText != null)
                endSubText.text = finished ? "Choose your reward" : "Try again on the app!";

            rewardFrom = 0;
            rewardTo = Mathf.Max(0, reward);
            rewardStartTime = (rewardUseUnscaledTime ? Time.unscaledTime : Time.time);
            rewardAnimating = true;

            SetRewardText(rewardFrom);
        }


        private void SetRewardText(int value)
        {
            if (rewardText != null)
                rewardText.text = $"${value}";
        }
    }
}