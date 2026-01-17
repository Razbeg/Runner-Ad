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
        [SerializeField] private Button ctaButton;

        public event Action CtaClicked;


        private void Awake()
        {
            if (ctaButton != null)
                ctaButton.onClick.AddListener(() => CtaClicked?.Invoke());
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

        public void ShowEndCard(bool on, int reward)
        {
            if (rewardText != null)
                rewardText.text = "$" + reward.ToString("0") + ".00";

            if (endCard)
                endCard.SetActive(on);
        }
    }
}