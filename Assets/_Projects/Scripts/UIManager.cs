using UnityEngine;
using UnityEngine.UI;
using System.Collections; // 코루틴(시간차 실행)을 쓰기 위해 필수!

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI 연결")]
    // [변경] GameObject 대신 CanvasGroup을 직접 제어합니다.
    public CanvasGroup scanGuideGroup;
    public GameObject showDescButton;

    private void Awake()
    {
        Instance = this;
    }

    public void OnImageRecognized(string artworkName)
    {
        // 이미 가이드가 꺼져있다면 무시 (중복 실행 방지)
        if (scanGuideGroup.gameObject.activeSelf == false) return;

        // 1. 페이드 아웃 코루틴 시작!
        StartCoroutine(FadeOutGuide());

        // 2. 설명 보기 버튼 켜기
        if (!showDescButton.activeSelf)
        {
            showDescButton.SetActive(true);
        }
    }

    // 0.5초 동안 서서히 투명해지는 마법의 함수
    IEnumerator FadeOutGuide()
    {
        float duration = 0.5f; // 0.5초 동안
        float timer = 0f;

        // 서서히 투명하게 (Alpha 1 -> 0)
        while (timer < duration)
        {
            timer += Time.deltaTime;
            // Lerp는 서서히 값을 바꾸는 수학 함수입니다.
            scanGuideGroup.alpha = Mathf.Lerp(1f, 0f, timer / duration);
            yield return null; // 한 프레임 대기
        }

        // 끝났으면 확실하게 0으로 만들고 끄기
        scanGuideGroup.alpha = 0f;
        scanGuideGroup.gameObject.SetActive(false);
    }

    public void ResetToScanMode()
    {
        // 다시 켜질 때는 투명도 100%로 복구하고 켜야 함
        scanGuideGroup.alpha = 1f;
        scanGuideGroup.gameObject.SetActive(true);

        showDescButton.SetActive(false);
    }
}