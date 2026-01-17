using UnityEngine;

namespace PlayableRunner
{
    public sealed class MovingObstacle : MonoBehaviour
    {
        [SerializeField] private RunnerGameController game;
        [SerializeField] private Transform player;

        [Header("Behavior")]
        [SerializeField] private float activateDistanceX = 6.0f;
        [SerializeField] private float extraLeftSpeed = 1.5f;
        [SerializeField] private float disableBehindPlayerX = 2.0f;

        private bool activated;


        private void OnEnable()
        {
            activated = false;
        }

        private void Update()
        {
            if (game == null || player == null)
                return;

            if (!game.IsSimulating)
                return;

            var dx = transform.position.x - player.position.x;

            if (!activated)
            {
                if (dx <= activateDistanceX)
                    activated = true;
                else
                    return;
            }

            transform.position += Vector3.left * (extraLeftSpeed * game.DeltaTime);

            if (dx <= -disableBehindPlayerX)
                gameObject.SetActive(false);
        }
    }
}