using UnityEngine;

namespace PlayableRunner
{
    public sealed class RunnerCharacter2D : MonoBehaviour
    {
        [Header("Jump")]
        [SerializeField] private float jumpVelocity = 8.5f;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private float groundRadius = 0.12f;
        [SerializeField] private LayerMask groundMask;

        [Header("Refs")]
        [SerializeField] private Animator animator;
        [SerializeField] private Rigidbody2D rb;

        private bool running;

        public bool IsJump { get; private set; }


        private void Reset()
        {
            rb = GetComponent<Rigidbody2D>();
            animator = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            IsJump = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundMask);
            if (animator != null)
            {
                animator.SetBool("IsRunning", running);
                animator.SetBool("IsJump", !IsJump);
            }
        }


        public void SetRunning(bool running) => this.running = running;

        public void Jump()
        {
            if (!IsJump)
                return;

            var v = rb.velocity;
            v.y = jumpVelocity;
            rb.velocity = v;

            if (animator != null)
                animator.SetBool("IsJump", !IsJump);
        }

        public void PlayHurt()
        {
            if (animator != null)
                animator.SetTrigger("Hurt");
        }
    }
}