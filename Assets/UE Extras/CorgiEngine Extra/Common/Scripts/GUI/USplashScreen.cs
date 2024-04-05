using MoreMountains.Tools;
using System.Collections;
using UnityEngine;
using System.Threading.Tasks;

namespace MoreMountains.CorgiEngine
{
    public class USplashScreen : CorgiMonoBehaviour
    {
        /// the level to load after the start screen
        [USceneName]
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
        [Tooltip("the duration of the fade out of the start screen, in seconds")]
        public float FadeOutDuration = 1f;
        /// the tween type this fade should happen on
        public MMTweenType Tween;

        protected virtual async void Start()
        {
            //In order to trigger fade out event after fader initialization
            await Task.Delay(1);

            GUIManager.Instance.SetHUDActive(false);
            MMFadeOutEvent.Trigger(FadeOutDuration, Tween);

            StartCoroutine(LoadFirstLevel());
        }
        /// <summary>
		/// Loads the next level.
		/// </summary>
		/// <returns>The first level.</returns>
		protected virtual IEnumerator LoadFirstLevel()
        {
            yield return new WaitForSeconds(FadeOutDuration);

            if (AutoSkipDelay - FadeInDuration > 0)
            {
                yield return new WaitForSeconds(AutoSkipDelay - FadeInDuration);
            }

            MMFadeInEvent.Trigger(FadeInDuration, Tween);
            yield return new WaitForSeconds(FadeInDuration);
            MMAdditiveSceneLoadingManager.LoadScene(StartScreen, LoadingSceneName);
        }
    }
}
