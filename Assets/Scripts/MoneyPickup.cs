using UnityEngine;

namespace PlayableRunner
{
    public sealed class MoneyPickup : MonoBehaviour
    {
        [SerializeField] private int value = 5;
        [SerializeField] private RunnerGameController game;


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
                return;

            game.NotifyMoneyPickup(value);
            gameObject.SetActive(false);
        }
    }
}