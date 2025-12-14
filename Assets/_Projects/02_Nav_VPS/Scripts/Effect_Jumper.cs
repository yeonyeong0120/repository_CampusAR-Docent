using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal; // [중요] 이 줄이 추가되어야 효과를 찾을 수 있습니다!
using System.Collections;

public class Effect_Jumper : MonoBehaviour
{
    // C 담당자의 씬 이름
    private const string FINAL_SCENE_NAME = "03_art";

    [Header("UI 연결")]
    [SerializeField] private GameObject LoadingScreenUI; // 검은 화면

    // 효과 제어 변수
    private Volume globalVolume;
    private LensDistortion lensDistortion;
    private ChromaticAberration chroma; // [수정] 띄어쓰기를 없애고 올바른 이름으로 고쳤습니다.

    void Start()
    {
        // 시작하자마자 효과 실행
        InitializePostProcessing();
        StartCoroutine(AnimateAndLoadScene(FINAL_SCENE_NAME));
    }

    private void InitializePostProcessing()
    {
        globalVolume = FindObjectOfType<Volume>();
        if (globalVolume != null && globalVolume.profile != null)
        {
            // 프로필에서 효과 가져오기
            globalVolume.profile.TryGet(out lensDistortion);
            globalVolume.profile.TryGet(out chroma);
        }
    }

    private IEnumerator AnimateAndLoadScene(string targetSceneName)
    {
        // 1. 검은 화면 켜기
        if (LoadingScreenUI != null) LoadingScreenUI.SetActive(true);

        float duration = 0.5f; // 0.5초 동안 빨려들어감
        float startTime = Time.time;

        // 효과가 연결되었다면 애니메이션 재생
        if (lensDistortion != null && chroma != null)
        {
            while (Time.time < startTime + duration)
            {
                float t = (Time.time - startTime) / duration;

                // 렌즈 왜곡: 0 -> -1 (안으로 빨려들어감)
                lensDistortion.intensity.value = Mathf.Lerp(0f, -1f, t);

                // 색수차: 0 -> 1 (색 번짐 효과)
                chroma.intensity.value = Mathf.Lerp(0f, 1f, t);

                yield return null;
            }
        }

        // 2. 애니메이션 끝나면 바로 이동
        SceneManager.LoadScene(targetSceneName);
    }
}