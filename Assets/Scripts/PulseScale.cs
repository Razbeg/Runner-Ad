using UnityEngine;

namespace PlayableRunner
{
    public sealed class PulseScale : MonoBehaviour
    {
        [SerializeField] private float min = 0.92f;
        [SerializeField] private float max = 1.08f;
        [SerializeField] private float speed = 4.5f;

        private Vector3 baseScale;


        private void Awake()
        {
            baseScale = transform.localScale;
        }

        private void Update()
        {
            var t = (Mathf.Sin(Time.unscaledTime * speed) + 1f) * 0.5f;
            var s = Mathf.Lerp(min, max, t);
            transform.localScale = baseScale * s;
        }
    }
}