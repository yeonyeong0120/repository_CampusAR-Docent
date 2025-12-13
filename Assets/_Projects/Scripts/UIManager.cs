using UnityEngine;
using UnityEngine.UI;
using TMPro; // [필수] 텍스트(TextMeshPro) 제어를 위해 필요
using System.Collections;
using System.Collections.Generic; // [필수] 리스트(List) 사용을 위해 필요

// 1. 작품 정보를 담을 '데이터 가방' 정의
// [System.Serializable]을 붙여야 인스펙터 창에서 입력칸이 보입니다.
[System.Serializable]
public class ArtworkInfo
{
    public string id;           // 구분용 ID (예: StarryNight) - 라이브러리 이름과 똑같아야 함!
    public string title;        // 화면에 띄울 제목 (예: 별이 빛나는 밤)
    [TextArea(3, 10)]           // 인스펙터에서 글상자 크게 보기 옵션
    public string description;  // 설명 내용
}

public class UIManager : MonoBehaviour
{
    public static UIManager Instance; // 싱글톤

    [Header("UI 연결: 기본")]
    public CanvasGroup scanGuideGroup;  // 스캔 가이드 (투명도 조절용)
    public GameObject showDescButton;   // 왼쪽 아래 '설명' 버튼
    public GameObject toggleARButton; // 토글

    [Header("UI 연결: 설명창(Bottom Sheet)")]
    public GameObject bottomSheetPanel; // 검은색 설명 패널 전체
    public TextMeshProUGUI txtTitle;    // 제목 글자 오브젝트
    public TextMeshProUGUI txtBody;     // 본문 글자 오브젝트

    [Header("데이터베이스")]
    // 팀장님이 직접 채워 넣을 작품 정보 리스트
    public List<ArtworkInfo> artworkDatabase;

    private ArtworkInfo currentInfo; // 현재 인식된 작품의 정보를 임시 저장할 변수

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // 시작할 때 설명창과 버튼은 꺼둡니다.
        bottomSheetPanel.SetActive(false);
        showDescButton.SetActive(false);
        if (toggleARButton != null) toggleARButton.SetActive(false);
    }

    // [핵심] 이미지가 인식되면 ImageContentController가 이 함수를 부릅니다.
    public void OnImageRecognized(string artworkName)
    {
        // 1. 스캔 가이드가 켜져 있다면 부드럽게 끈다.
        if (scanGuideGroup.gameObject.activeSelf)
        {
            StartCoroutine(FadeOutGuide());
        }

        // 2. 데이터베이스(리스트)에서 이름(ID)이 같은 정보를 찾는다.
        // "내 가방(database) 뒤져서 x.id가 artworkName이랑 같은 녀석(Find)을 찾아줘!"
        currentInfo = artworkDatabase.Find(x => x.id == artworkName);

        if (currentInfo != null)
        {
            if (!showDescButton.activeSelf) showDescButton.SetActive(true);
            if (toggleARButton != null) toggleARButton.SetActive(true);

            Debug.Log($"[UI] 1차 매칭 완료 (로컬 데이터): {currentInfo.title}");

            // ==========================================================
            // [추가] 2. 위키백과 API에게 "최신 설명 좀 찾아봐!" 라고 시킴
            // ==========================================================
            if (WikiAPIManager.Instance != null)
            {
                // currentInfo.title (예: "별이 빛나는 밤")을 검색어로 넘김
                WikiAPIManager.Instance.GetDescription(currentInfo.title, (string apiDescription) =>
                {
                    // [이곳은 인터넷 통신이 성공한 뒤에 실행됨]
                    Debug.Log("[UI] API 데이터 도착! 설명을 덮어씁니다.");

                    // 1. 현재 정보 업데이트
                    currentInfo.description = apiDescription + "\n\n(출처: 위키백과)";

                    // 2. 만약 이미 설명창이 열려있다면, 글자도 바로 바꿔주기
                    if (bottomSheetPanel.activeSelf)
                    {
                        txtBody.text = currentInfo.description;
                    }
                });
            }
        }
    }

    // [버튼 연결용] '설명' 버튼을 누르면 실행됨
    public void OpenBottomSheet()
    {
        if (currentInfo != null)
        {
            // 저장해둔 정보를 텍스트 UI에 꽂아넣기 (Data Binding)
            txtTitle.text = currentInfo.title;
            txtBody.text = currentInfo.description;

            // 패널 켜기
            bottomSheetPanel.SetActive(true);
            // 버튼은 잠깐 숨기기 토글버튼두,,,
            showDescButton.SetActive(false);
            if (toggleARButton != null) toggleARButton.SetActive(false);
        }
    }

    // [버튼 연결용] 설명창의 'X' 버튼을 누르면 실행됨
    public void CloseBottomSheet()
    {
        bottomSheetPanel.SetActive(false);
        showDescButton.SetActive(true); // 버튼 다시 보여주기
        if (toggleARButton != null) toggleARButton.SetActive(true);
    }

    // 페이드 아웃 코루틴
    IEnumerator FadeOutGuide()
    {
        float duration = 0.5f;
        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            scanGuideGroup.alpha = Mathf.Lerp(1f, 0f, timer / duration);
            yield return null;
        }
        scanGuideGroup.alpha = 0f;
        scanGuideGroup.gameObject.SetActive(false);
    }

    // 초기화용
    public void ResetToScanMode()
    {
        scanGuideGroup.alpha = 1f;
        scanGuideGroup.gameObject.SetActive(true);
        showDescButton.SetActive(false);
        bottomSheetPanel.SetActive(false);

        if (toggleARButton != null) toggleARButton.SetActive(false);
    }
}