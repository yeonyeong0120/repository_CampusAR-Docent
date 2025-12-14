using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // 씬 이동 필수
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class StampManager : MonoBehaviour
{
    public static StampManager Instance;

    [Header("1. 스탬프 UI")]
    public Image[] stampIcons; // 아이콘 3개 (순서: 0:Starry, 1:Scream, 2:Mona)
    public Sprite[] activeSprites; // 컬러 이미지 3장 (Inspector에서 넣기)
    public TextMeshProUGUI txtMissionCount; // "0 / 3" 텍스트
    public GameObject stampBoardPanel; // 깜빡거릴 판
    public Image stampBoardOutline;    // 색깔 바꿀 테두리
    public Button btnStampBoard; // 보드판 버튼

    [Header("2. 티켓 UI")]
    public RectTransform ticketRect; // 움직일 티켓 (Img_Ticket)
    public Button btnTicket;         // 티켓 버튼
    public float slideSpeed = 2.0f;  // [요청] 슬라이드 속도 제어

    [Header("3. 피날레 UI")]
    public GameObject finalOverlayPanel; // 검은 화면
    public Button btnGoReward;           // 씬 이동 버튼
    public Button btnCloseFinal;  // 판넬 닫기버튼

    // 내부 변수
    private int currentCount = 0;
    private HashSet<string> collectedArtworks = new HashSet<string>(); // 중복 방지용
    private bool isAllClear = false;
    private bool isTicketDropped = false;  // 티켓 내려왔는지 확인용

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        // 초기화
        finalOverlayPanel.SetActive(false);

        // 티켓 숨기기 (위로 올려서 Mask에 가려지게 함)
        // Y값을 200 정도로 올려서 안보이게 세팅
        ticketRect.anchoredPosition = new Vector2(0, 200);
        if (btnStampBoard != null)
        {
            btnStampBoard.interactable = false; // 아직 스탬프 다 안 모았으니까 클릭 금지
            btnStampBoard.onClick.AddListener(OnStampBoardClicked);
        }

        // 버튼 연결
        btnTicket.onClick.AddListener(OnTicketClicked);
        btnGoReward.onClick.AddListener(GoToRewardScene);
        if (btnCloseFinal != null) btnCloseFinal.onClick.AddListener(CloseFinalReward);  //닫기

        UpdateUI();
    }

    // [외부 호출] ImageContentController에서 이 함수를 부를 겁니다!
    public void CollectStamp(string artworkID)
    {
        if (isAllClear) return; // 이미 끝났으면 무시
        if (collectedArtworks.Contains(artworkID)) return; // 이미 찍은 거면 무시

        // 1. 도장 쾅!
        collectedArtworks.Add(artworkID);
        currentCount++;

        // 2. UI 업데이트 (아이콘 색칠)
        int index = -1;
        if (artworkID.Contains("Starry")) index = 0;
        else if (artworkID.Contains("Scream")) index = 1;
        else if (artworkID.Contains("Mona")) index = 2;

        if (index != -1 && index < stampIcons.Length)
        {
            // 컬러 이미지로 교체
            stampIcons[index].sprite = activeSprites[index];
            // 효과음 재생하면 좋음 (PlaySound)
        }

        UpdateUI();

        // 3. 다 모았는지 체크
        if (currentCount >= 3)
        {
            StartCoroutine(AllClearRoutine());
        }
    }

    void UpdateUI()
    {
        txtMissionCount.text = $"Artwork {currentCount} / 3";
    }

    // [연출] 올클리어 루틴
    IEnumerator AllClearRoutine()
    {
        isAllClear = true;
        Debug.Log("ALL CLEAR! 보상 루틴 시작");
        // 보드판 버튼 활성화! (이제 누를 수 있음)
        if (btnStampBoard != null) btnStampBoard.interactable = true;

        // 황금색 깜빡임 효과 (무한 반복 or 일정 시간)
        // 여기선 사용자가 누를 때까지 계속 황금색으로 빛나게 합시다.
        if (stampBoardOutline != null)
        {
            Color goldColor;
            ColorUtility.TryParseHtmlString("#C5A059", out goldColor);
            stampBoardOutline.color = goldColor;
        }

        yield return null;
    }

    void OnStampBoardClicked()
    {
        if (isTicketDropped) return; // 이미 내려왔으면 무시

        StartCoroutine(DropTicketRoutine());
    }

    IEnumerator DropTicketRoutine()
    {
        isTicketDropped = true;

        // 더 이상 보드판 못 누르게 (이미 티켓 나왔으니까)
        if (btnStampBoard != null) btnStampBoard.interactable = false;

        // 티켓 슬라이드 다운
        Vector2 startPos = ticketRect.anchoredPosition;
        // [수정] 더 많이 내려오게 좌표 수정 (-450)
        Vector2 targetPos = new Vector2(0, -450);

        float slideTimer = 0f;
        while (slideTimer < 1f)
        {
            slideTimer += Time.deltaTime * slideSpeed;
            ticketRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, slideTimer);
            yield return null;
        }

        // 티켓 클릭 활성화
        btnTicket.interactable = true;
    }

    // [이벤트] 티켓 클릭 시
    void OnTicketClicked()
    {
        // 검은 화면 덮기
        finalOverlayPanel.SetActive(true);
    }

    void CloseFinalReward()
    {
        finalOverlayPanel.SetActive(false);
    }

    // [이벤트] 선물 받기 클릭 시 -> 03씬으로 이동
    void GoToRewardScene()
    {
        // 씬 이름 정확히 확인하세요!
        SceneManager.LoadScene("03_Reward");
    }
}