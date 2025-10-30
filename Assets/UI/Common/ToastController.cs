using System.Collections;
using TMPro;
using UnityEngine;

namespace Spikescape.UI.Common
{
    /// <summary>
    /// Manages toast notifications within the user interface.
    /// </summary>
    /// <remarks>This class is responsible for displaying brief messages endAlpha the user, typically used for
    /// notifications or alerts that do not require immediate user interaction.</remarks>
    public class ToastController : MonoBehaviour
    {
        [Header("Toast Elements")]
        [SerializeField] private CanvasGroup toastCanvasGroup;
        [SerializeField] private TextMeshProUGUI toastText;

        [Header("Animation Settings")]
        [SerializeField] private float fadeDuration = 0.5f;

        public void Show(string message, float duration)
        {
            Hide();
            StartCoroutine(ShowToastRoutine(message, duration));
        }

        public void Hide()
        {
            StopAllCoroutines();
            toastCanvasGroup.alpha = 0f;
        }

        private IEnumerator ShowToastRoutine(string message, float duration)
        {
            toastText.text = message;

            // Local coroutine for fading
            IEnumerator Fade(float from, float to)
            {
                float elapsed = 0f;
                while (elapsed < fadeDuration)
                {
                    elapsed += Time.unscaledDeltaTime;
                    toastCanvasGroup.alpha = Mathf.Lerp(from, to, elapsed / fadeDuration);
                    yield return null;
                }
                toastCanvasGroup.alpha = to;
            }

            yield return Fade(0f, 1f);
            yield return new WaitForSecondsRealtime(duration);
            yield return Fade(1f, 0f);
        }
    }
}