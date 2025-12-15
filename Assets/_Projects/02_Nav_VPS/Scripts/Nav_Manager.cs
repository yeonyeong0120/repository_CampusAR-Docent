using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing; // 효과 제어용
using System.Collections;
using TMPro;

public class Nav_Manager : MonoBehaviour
{
    [Header("필수 연결")]
    public Transform targetObject; // Goal_Point
    public Transform arCamera;     // Main Camera
    public GameObject enterButton; // Btn_EnterGallery

    [Header("디버그 & 효과 (새로 추가됨)")]
    public TextMeshProUGUI distanceText; // Debug_Text
    public PostProcessVolume globalVolume; // 방금 만든 Global Volume을 여기에 넣으세요!

    [Header("설정")]
    public float arrivalDistance = 1.5f;
    public string nextSceneName = "02_Viewing"; // 이동할 씬 이름

    private bool isArrived = false;

    void Update()
    {
        if (targetObject == null || arCamera == null) return;

        // 거리 계산 및 표시
        float distance = Vector3.Distance(arCamera.position, targetObject.position);

        if (distanceText != null)
            distanceText.text = $"남은 거리: {distance:F2}m";

        // 도착 판정
        if (!isArrived && distance < arrivalDistance)
        {
            isArrived = true;
            if (enterButton != null) enterButton.SetActive(true);
        }
    }

    // 버튼 클릭 시 실행
    public void OnClickEnterButton()
    {
        // 코루틴(시간차 공격) 시작
        StartCoroutine(PlayEffectAndLoad());
    }

    IEnumerator PlayEffectAndLoad()
    {
        Debug.Log("🚀 효과 시작! 씬 이동 준비...");

        // 버튼과 텍스트 숨기기 (깔끔하게)
        if (enterButton != null) enterButton.SetActive(false);
        if (distanceText != null) distanceText.gameObject.SetActive(false);

        float duration = 1.0f; // 1초 동안 효과 진행
        float startTime = Time.time;

        LensDistortion ld = null;
        ChromaticAberration ca = null;

        // 볼륨에서 효과 설정 가져오기
        if (globalVolume != null && globalVolume.profile != null)
        {
            globalVolume.profile.TryGetSettings(out ld);
            globalVolume.profile.TryGetSettings(out ca);
        }

        // 효과 애니메이션 재생
        if (ld != null && ca != null)
        {
            while (Time.time < startTime + duration)
            {
                float t = (Time.time - startTime) / duration;

                // 렌즈 왜곡: 0에서 -100까지 (빨려들어감)
                ld.intensity.value = Mathf.Lerp(0f, -100f, t);

                // 색수차: 0에서 1까지 (색 번짐)
                ca.intensity.value = Mathf.Lerp(0f, 1f, t);

                yield return null;
            }
        }
        else
        {
            Debug.LogWarning("⚠️ 효과 컴포넌트를 못 찾았습니다. 그냥 이동합니다.");
        }

        // 효과 끝! 씬 이동
        SceneManager.LoadScene(nextSceneName);
    }
}