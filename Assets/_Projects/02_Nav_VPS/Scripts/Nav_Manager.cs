// Nav_Manager.cs
//
// 역할: VPS (02_vps 씬)에서 이미지 트래킹 (03_art 씬)으로 전환을 관리하는 스크립트입니다.
// 주요 기능:
// 1. 사용자 위치 기반 도착 판정 (Navigation 종료)
// 2. Immersal (VPS) 리소스 비활성화 및 정리
// 3. Post Processing을 사용한 '빨려 들어가는' 시각 효과 애니메이션 연출
// 4. 애니메이션 완료 후 다음 씬(03_art) 로드 (제어권 전달)
//
// 담당자: B (VPS Tech / Scene Transition Manager)
// 연결된 씬: 02_vps (Navigation) -> 03_art (Viewing)
// ==========================================================

// Nav_Manager.cs (02_vps 씬의 전환 제어 스크립트)
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
// 사용하는 렌더 파이프라인에 따라 추가 (예: URP)
// using UnityEngine.Rendering.Universal; 

public class Nav_Manager : MonoBehaviour
{
    // ==========================================================
    // 1. 변수 설정: Inspector에서 설정합니다.
    // ==========================================================
    [Header("1. 도착 및 다음 씬 설정")]
    [SerializeField] private Vector3 targetPoint = Vector3.zero; // 도착 목표 좌표
    [SerializeField] private float arrivalDistance = 1.0f;       // 도착 허용 오차 (1m)
    [SerializeField] private string nextSceneName = "03_art";     // 다음 씬 이름

    [Header("2. 시각 효과 설정")]
    [SerializeField] private GameObject LoadingScreenUI;          // 검은 화면 가림막 UI

    // Post Processing 이펙트 제어 변수
    private Volume globalVolume;
    private LensDistortion lensDistortion;
    private ChromaticAberration chroma;
    private bool isTransitionStarted = false;

    // ==========================================================
    // 2. 초기화 (Start)
    // ==========================================================
    void Start()
    {
        // 씬에 있는 Global Volume 컴포넌트 찾기
        globalVolume = FindObjectOfType<Volume>();
        if (globalVolume != null && globalVolume.profile != null)
        {
            // Volume Profile에서 이펙트 컴포넌트 가져오기
            globalVolume.profile.TryGet(out lensDistortion);
            globalVolume.profile.TryGet(out chroma);
        }
        else
        {
            Debug.LogError("Post Processing Volume이 설정되지 않았습니다. 시각 효과가 작동하지 않습니다.");
        }
    }

    // ==========================================================
    // 3. 도착 판정 및 전환 트리거
    // (VPS Localization Manager에서 사용자 위치가 업데이트될 때 호출되어야 함)
    // ==========================================================
    public void OnUserLocalized(Vector3 userPosition)
    {
        if (isTransitionStarted) return;

        // 도착 판정
        if (Vector3.Distance(userPosition, targetPoint) < arrivalDistance)
        {
            isTransitionStarted = true;

            // VPS 비활성화 (다음 씬과의 리소스 충돌 방지)
            DisableImmersal();

            // 시각 효과 및 씬 로드 코루틴 시작
            StartCoroutine(AnimateAndLoadScene(nextSceneName));
        }
    }

    private void DisableImmersal()
    {
        // 여기에 Immersal SDK 및 관련 AR Session 컴포넌트를 비활성화하는 로직 삽입
    }

    // ==========================================================
    // 4. 시각 효과 애니메이션 및 씬 로드 (핵심 로직)
    // ==========================================================
    private IEnumerator AnimateAndLoadScene(string targetSceneName)
    {
        // 1. 검은 화면 가림막 활성화
        if (LoadingScreenUI != null) LoadingScreenUI.SetActive(true);

        float duration = 0.5f; // '뿅' 하는 효과를 위한 빠른 시간
        float startTime = Time.time;

        if (lensDistortion != null && chroma != null)
        {
            // 2. '빨려 들어가는' 시각 효과 애니메이션 연출
            while (Time.time < startTime + duration)
            {
                float t = (Time.time - startTime) / duration;

                // 렌즈 왜곡: 0에서 -100으로 증가
                lensDistortion.intensity.value = Mathf.Lerp(0f, -100f, t);

                // 색수차: 0에서 1로 증가
                chroma.intensity.value = Mathf.Lerp(0f, 1f, t);

                yield return null;
            }
        }

        // 3. 애니메이션 완료 후, 즉시 다음 씬(03_art) 로드
        SceneManager.LoadScene(targetSceneName);
    }
}