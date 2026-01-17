using UnityEngine;

namespace PlayableRunner
{
    public sealed class UiRotateScaleLoop : MonoBehaviour
    {
        [Header("Rotation")]
        [SerializeField] private float degreesPerSecond = 180f;

        [Header("Scale Pulse")]
        [SerializeField] private float minScale = 0.92f;
        [SerializeField] private float maxScale = 1.08f;
        [SerializeField] private float pulseSpeed = 3.5f;

        [Header("Options")]
        [SerializeField] private bool useUnscaledTime = true;
        [SerializeField] private bool resetOnEnable = true;

        private Vector3 baseScale;
        private float baseZ;

        private void Awake()
        {
            baseScale = transform.localScale;
            baseZ = transform.localEulerAngles.z;
        }

        private void OnEnable()
        {
            if (!resetOnEnable)
                return;

            baseScale = transform.localScale;
            baseZ = transform.localEulerAngles.z;
        }

        private void Update()
        {
            var dt = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            var t = useUnscaledTime ? Time.unscaledTime : Time.time;

            // Rotate
            transform.Rotate(0f, 0f, degreesPerSecond * dt, Space.Self);

            // Pulse scale
            var s01 = (Mathf.Sin(t * pulseSpeed * Mathf.PI * 2f) + 1f) * 0.5f;
            var s = Mathf.Lerp(minScale, maxScale, s01);
            transform.localScale = baseScale * s;

            // Optional: keep Z stable around base (if you prefer absolute instead of accumulating rotation)
            // transform.localEulerAngles = new Vector3(0f, 0f, _baseZ + t * degreesPerSecond);
        }
    }
}