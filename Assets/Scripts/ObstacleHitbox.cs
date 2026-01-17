using UnityEngine;

namespace PlayableRunner
{
    public sealed class ObstacleHitbox : MonoBehaviour
    {
        [SerializeField] private RunnerGameController game;

        private bool spent;


        private void OnTriggerEnter2D(Collider2D other)
        {
            if (spent)
                return;

            if (!other.CompareTag("Player"))
                return;

            spent = true;
            game.NotifyObstacleHit();
        }
    }
}