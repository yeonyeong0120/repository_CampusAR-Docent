using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class ArtworkInfo
{
    [Header("기본 정보 (필수)")]
    public string id;           // AR 라이브러리 이름 (예: StarryNight)
    public string wikiSearchKeyword; // 위키백과 검색용 (예: 별이 빛나는 밤) - 정확해야 함!

    [Header("화면 표시용 (자유롭게 꾸미세요)")]
    public string title;        // 제목 (예: 별이 빛나는 밤 (The Starry Night))
    public string artist;       // 작가 (예: 빈센트 반 고흐)
    public string year;         // 연도 (예: 1889년)

    [TextArea(3, 10)]
    public string description;  // 기본 설명 (API 실패 시 보여줄 내용)
}

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI 연결: 기본")]
    public CanvasGroup scanGuideGroup;
    public GameObject showDescButton;
    public GameObject toggleARButton;

    [Header("UI 연결: 설명창(Bottom Sheet)")]
    public GameObject bottomSheetPanel;
    public TextMeshProUGUI txtTitle;    // 제목
    public TextMeshProUGUI txtArtist;   // 작가 [NEW]
    public TextMeshProUGUI txtYear;     // 연도 [NEW]
    public TextMeshProUGUI txtBody;     // 본문

    [Header("데이터베이스")]
    public List<ArtworkInfo> artworkDatabase;

    private ArtworkInfo currentInfo;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        bottomSheetPanel.SetActive(false);
        showDescButton.SetActive(false);
        if (toggleARButton != null) toggleARButton.SetActive(false);
    }

    public void OnImageRecognized(string artworkName)
    {
        if (scanGuideGroup.gameObject.activeSelf) StartCoroutine(FadeOutGuide());

        currentInfo = artworkDatabase.Find(x => x.id == artworkName);

        if (currentInfo != null)
        {
            if (!showDescButton.activeSelf) showDescButton.SetActive(true);
            if (toggleARButton != null) toggleARButton.SetActive(true);

            Debug.Log($"[UI] 1차 매칭 완료: {currentInfo.title}");

            // ==========================================================
            // [API 연동] 'wikiSearchKeyword'로 검색합니다!
            // ==========================================================
            if (WikiAPIManager.Instance != null)
            {
                // 화면 표시용 Title(영어포함) 대신, 검색용 Keyword(한글)를 보냄
                WikiAPIManager.Instance.GetDescription(currentInfo.wikiSearchKeyword, (string apiDescription) =>
                {
                    Debug.Log("[UI] API 데이터 도착! 본문만 덮어씁니다.");

                    // Body 설명만 교체 (제목, 작가, 연도는 건드리지 않음)
                    currentInfo.description = apiDescription + "\n\n(출처: 위키백과)";

                    if (bottomSheetPanel.activeSelf)
                    {
                        txtBody.text = currentInfo.description;
                    }
                });
            }
        }
    }

    public void OpenBottomSheet()
    {
        if (currentInfo != null)
        {
            // 1. 제목과 본문은 그대로
            txtTitle.text = currentInfo.title.Replace(" (", "\n(");  // 제목에 엔터 추가
            txtBody.text = currentInfo.description;

            // 작가: "Artist" 라벨은 회색(#AAAAAA), 이름은 골드(#C5A059)
            txtArtist.text = $"<color=#AAAAAA>Artist</color>  <color=#C5A059>{currentInfo.artist}</color>";

            // 연도: "Year" 라벨은 회색, 연도는 골드
            //txtYear.text = $"<color=#AAAAAA>Year</color>  <color=#C5A059>{currentInfo.year}</color>";
            txtYear.text = currentInfo.year;

            bottomSheetPanel.SetActive(true);
            showDescButton.SetActive(false);
            if (toggleARButton != null) toggleARButton.SetActive(false);
        }
    }

    public void CloseBottomSheet()
    {
        bottomSheetPanel.SetActive(false);
        showDescButton.SetActive(true);
        if (toggleARButton != null) toggleARButton.SetActive(true);
    }

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

    public void ResetToScanMode()
    {
        scanGuideGroup.alpha = 1f;
        scanGuideGroup.gameObject.SetActive(true);
        showDescButton.SetActive(false);
        bottomSheetPanel.SetActive(false);
        if (toggleARButton != null) toggleARButton.SetActive(false);
    }
}