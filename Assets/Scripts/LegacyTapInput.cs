using UnityEngine;

namespace PlayableRunner
{
    public static class LegacyTapInput
    {
        public static bool TapDown()
        {
            if (Input.GetMouseButtonDown(0))
                return true;

            if (Input.touchCount <= 0)
                return false;

            return Input.GetTouch(0).phase == TouchPhase.Began;
        }
    }
}