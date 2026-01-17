using UnityEngine;

namespace PlayableRunner
{
    public sealed class MovingObstacle : MonoBehaviour
    {
        [SerializeField] private RunnerGameController game;
        [SerializeField] private float extraLeftSpeed = 1.5f;


        private void Update()
        {
            if (game == null || !game.IsSimulating)
                return;

            transform.position += Vector3.left * (extraLeftSpeed * game.DeltaTime);
        }
    }
}