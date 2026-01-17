using UnityEngine;

namespace PlayableRunner
{
    public sealed class PlayableAudioLoop : MonoBehaviour
    {
        [SerializeField] private AudioSource source;
        [SerializeField, Range(0f, 1f)] private float volume = 0.6f;

        private bool started;


        private void Awake()
        {
            if (source == null)
                source = GetComponent<AudioSource>();

            if (source != null)
            {
                source.playOnAwake = false;
                source.loop = true;
                source.volume = volume;
            }
        }

        private void OnApplicationPause(bool pause)
        {
            SetPaused(pause);
        }


        public void StartOnFirstTouch()
        {
            if (started)
                return;

            if (source == null || source.clip == null)
                return;

            started = true;
            source.volume = volume;
            source.Play();
        }

        public void SetPaused(bool paused)
        {
            if (!started || source == null)
                return;

            if (paused)
                source.Pause();
            else
                source.UnPause();
        }

        public void Stop()
        {
            if (source == null)
                return;

            source.Stop();
            started = false;
        }
    }
}