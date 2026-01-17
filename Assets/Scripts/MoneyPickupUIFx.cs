using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace PlayableRunner
{
    public sealed class MoneyPickupUIFx : MonoBehaviour
    {
        [Header("Popup")]
        [SerializeField] private GameObject popup;
        [SerializeField] private float popupDuration = 0.35f;
        [SerializeField] private float popupMaxScale = 1.15f;

        [Header("Popup Text")]
        [SerializeField] private Text popupText;
        [SerializeField] private string[] popupMessages;
        [SerializeField] private bool avoidSameMessageTwice = true;

        [Header("Fly (optional)")]
        [SerializeField] private RectTransform flyRoot;
        [SerializeField] private Image flyPrefab;
        [SerializeField] private RectTransform targetIcon;
        [SerializeField] private int poolSize = 6;
        [SerializeField] private float flyDuration = 0.45f;
        [SerializeField] private float arcHeight = 80f;

        [Header("Cameras")]
        [SerializeField] private Camera worldCamera;
        [SerializeField] private Canvas canvas;

        private RectTransform popupRt;
        private Vector3 popupBaseScale = Vector3.one;
        private bool popupAnimating;
        private int lastPopupMsgIndex = -1;

        private Image[] pool;
        private bool[] inUse;
        private Camera uiCam;


        private void Awake()
        {
            if (popup != null)
            {
                popupRt = popup.transform as RectTransform;
                if (popupRt != null)
                    popupBaseScale = popupRt.localScale;

                if (popupText == null)
                    popupText = popup.GetComponentInChildren<Text>(true);

                popup.SetActive(false);
            }

            if (canvas == null)
                canvas = GetComponentInParent<Canvas>();

            uiCam = (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay) ? canvas.worldCamera: null;

            if (flyRoot == null && canvas != null)
                flyRoot = canvas.transform as RectTransform;

            InitPool();
        }


        public void Play(Vector3 pickupWorldPos)
        {
            TryPlayPopup();
            TryPlayFly(pickupWorldPos);
        }


        private void TryPlayPopup()
        {
            if (popup == null)
                return;

            if (popupAnimating || popup.activeSelf)
                return;

            SetRandomPopupText();
            StartCoroutine(PopupRoutine());
        }

        private void SetRandomPopupText()
        {
            if (popupText == null)
                return;

            if (popupMessages == null || popupMessages.Length == 0)
                return;

            var idx = Random.Range(0, popupMessages.Length);

            if (avoidSameMessageTwice && popupMessages.Length > 1 && idx == lastPopupMsgIndex)
            {
                idx = (idx + 1 + Random.Range(0, popupMessages.Length - 1)) % popupMessages.Length;
            }

            lastPopupMsgIndex = idx;
            popupText.text = popupMessages[idx];
        }

        private IEnumerator PopupRoutine()
        {
            popupAnimating = true;
            popup.SetActive(true);

            var half = Mathf.Max(0.0001f, popupDuration * 0.5f);
            var t = 0f;

            while (t < popupDuration)
            {
                t += Time.unscaledDeltaTime;
                var s = (t <= half)
                    ? Mathf.Lerp(0f, popupMaxScale, EaseOutQuad(t / half))
                    : Mathf.Lerp(popupMaxScale, 0f, EaseInQuad((t - half) / half));

                if (popupRt != null)
                    popupRt.localScale = popupBaseScale * s;

                yield return null;
            }

            if (popupRt != null)
                popupRt.localScale = popupBaseScale;

            popup.SetActive(false);
            popupAnimating = false;
        }


        private void TryPlayFly(Vector3 pickupWorldPos)
        {
            if (flyRoot == null || flyPrefab == null || targetIcon == null)
                return;

            var idx = GetFreeIndex();
            if (idx < 0)
                return;

            var img = pool[idx];
            var rt = img.rectTransform;

            var start = WorldToFlyRootLocal(pickupWorldPos);
            var end = (Vector2)flyRoot.InverseTransformPoint(targetIcon.position);

            rt.anchoredPosition = start;
            rt.localScale = Vector3.one;
            img.gameObject.SetActive(true);
            inUse[idx] = true;

            StartCoroutine(FlyRoutine(idx, rt, start, end));
        }

        private IEnumerator FlyRoutine(int idx, RectTransform rt, Vector2 start, Vector2 end)
        {
            var t = 0f;
            var d = Mathf.Max(0.0001f, flyDuration);

            while (t < d)
            {
                t += Time.unscaledDeltaTime;
                var p = Mathf.Clamp01(t / d);
                var e = EaseOutCubic(p);

                var pos = Vector2.LerpUnclamped(start, end, e);
                pos.y += arcHeight * 4f * p * (1f - p);

                rt.anchoredPosition = pos;
                rt.localScale = Vector3.one * Mathf.Lerp(1f, 0.75f, p);

                yield return null;
            }

            rt.anchoredPosition = end;
            rt.localScale = Vector3.one;

            pool[idx].gameObject.SetActive(false);
            inUse[idx] = false;
        }


        private void InitPool()
        {
            if (flyRoot == null || flyPrefab == null || poolSize <= 0)
                return;

            pool = new Image[poolSize];
            inUse = new bool[poolSize];

            for (var i = 0; i < poolSize; i++)
            {
                var img = Instantiate(flyPrefab, flyRoot);
                img.gameObject.SetActive(false);
                pool[i] = img;
                inUse[i] = false;
            }

            flyPrefab.gameObject.SetActive(false);
        }

        private int GetFreeIndex()
        {
            if (pool == null)
                return -1;

            for (var i = 0; i < pool.Length; i++)
                if (!inUse[i])
                    return i;

            return -1;
        }

        private Vector2 WorldToFlyRootLocal(Vector3 worldPos)
        {
            var cam = worldCamera != null ? worldCamera : Camera.main;

            var screen = RectTransformUtility.WorldToScreenPoint(cam, worldPos);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(flyRoot, screen, uiCam, out var local);
            return local;
        }



        private static float EaseInQuad(float t) => t * t;
        private static float EaseOutQuad(float t) => 1f - (1f - t) * (1f - t);
        private static float EaseOutCubic(float t)
        {
            var u = 1f - t;
            return 1f - u * u * u;
        }
    }
}
