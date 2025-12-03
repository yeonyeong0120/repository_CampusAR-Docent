using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;

    [Header("Loading UI")]
    public CanvasGroup loadingCanvasGroup; // 검은 화면 가림막
    public float fadeDuration = 0.5f;

    private void Awake()
    {
        // 싱글톤 패턴 // 이 오브젝트는 씬이 바뀌어도 파괴되지 않음
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 이동 시 살아남음
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 시작할 때는 로딩 화면을 숨김
        loadingCanvasGroup.alpha = 0;
        loadingCanvasGroup.blocksRaycasts = false;
    }

    // 외부에서 이 함수를 호출해서 씬을 이동함
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        // 1. 페이드 아웃 (검은 화면이 나타남) - "작품 관람 모드로 전환 중..." 
        loadingCanvasGroup.blocksRaycasts = true; // 터치 차단
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            loadingCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }
        loadingCanvasGroup.alpha = 1f;

        // 2. 비동기 씬 로드 (메모리 정리 및 로딩)
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false; // 로딩 다 될 때까지 대기

        while (!op.isDone)
        {
            // 로딩이 90% 이상 되면 진행
            if (op.progress >= 0.9f)
            {
                op.allowSceneActivation = true;
            }
            yield return null;
        }

        // 3. 페이드 인 (검은 화면이 사라짐)
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            loadingCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }
        loadingCanvasGroup.alpha = 0f;
        loadingCanvasGroup.blocksRaycasts = false; // 터치 허용
    }
}