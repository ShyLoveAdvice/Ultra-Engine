using MoreMountains.Tools;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace MoreMountains.CorgiEngine
{
    public class UGameOverScreen : CorgiMonoBehaviour
    {
        /// the level to load after the start screen
        [Tooltip("the level to load after the start screen"), USceneName]
        public string StartScreen = "StartScreen";
        /// the name of the loading screen to use to load NextLevel
        [Tooltip("the name of the loading screen to use to load NextLevel"), USceneName]
        public string LoadingSceneName = "LoadingScreen";
        /// the delay after which the level should auto skip (if less than 1s, won't autoskip)
        [Tooltip("the delay after which the level should auto skip (if less than 1s, won't autoskip)")]
        public float AutoSkipDelay = 0f;

        [Header("Fades")]
        /// the duration of the fade in of the start screen, in seconds
        [Tooltip("the duration of the fade in of the start screen, in seconds")]
        public float FadeInDuration = 1f;
        /// the duration of the fade out of the start screen, in seconds
        [Tooltip("the duration of the fade out of the start screen, in seconds")]
        public float FadeOutDuration = 1f;
        /// the tween type this fade should happen on
        public MMTweenType Tween;

        protected async void Start()
        {
            await Task.Delay(1);

            GUIManager.Instance.SetHUDActive(false);
            MMFadeOutEvent.Trigger(FadeInDuration, Tween);
        }
        /// <summary>
		/// What happens when the main button is pressed
		/// </summary>
		public virtual void ButtonPressed()
        {
            MMFadeInEvent.Trigger(FadeOutDuration, Tween, 0, true);
            // if the user presses the "Jump" button, we start the first level.
            StartCoroutine(LoadFirstLevel());
        }

        /// <summary>
        /// Loads the next level.
        /// </summary>
        /// <returns>The first level.</returns>
        protected virtual IEnumerator LoadFirstLevel()
        {
            yield return new WaitForSeconds(FadeOutDuration);
            MMAdditiveSceneLoadingManager.LoadScene(StartScreen, LoadingSceneName);
        }
    }
}
