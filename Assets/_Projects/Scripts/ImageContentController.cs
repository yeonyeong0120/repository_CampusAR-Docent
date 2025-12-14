using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;

public class ImageContentController : MonoBehaviour
{
    [Header("각 작품별 오브젝트/이펙트 모음 연결")]
    public GameObject effectStarryNight;
    public GameObject effectScream;
    public GameObject effectMonaLisa;

    private ARTrackedImage trackedImage;
    private string currentActiveName = "";


    void Awake()
    {
        trackedImage = GetComponent<ARTrackedImage>();
    }

    void Start()
    {
        DisableAll();

        // UIManager야, 니가 들고 있는 'toggleARButton' 좀 빌려줘!
        if (UIManager.Instance != null && UIManager.Instance.toggleARButton != null)
        {
            // 버튼 컴포넌트를 가져옵니다.
            Button btn = UIManager.Instance.toggleARButton.GetComponent<Button>();

            if (btn != null)
            {
                // 만약 이전에 연결된 게 있다면 지우고 (중복 방지)
                btn.onClick.RemoveAllListeners();

                // 내 기능(ToggleContent)을 연결해라!
                btn.onClick.AddListener(ToggleContent);

                Debug.Log("[AR] 토글 버튼이 성공적으로 연결되었습니다!");
            }
        }

    }

    void Update()
    {
        //Debug.Log("업데이트 돌아가는 중..."); // 디버그용 추가

        if (trackedImage == null)
        {
            trackedImage = GetComponent<ARTrackedImage>();
            return;
        }

        if (trackedImage.referenceImage == null) return;

        UpdateContent();
    }

    void UpdateContent()
    {
        string newName = trackedImage.referenceImage.name;

        // 이름이 비어있거나 Null이면? -> "아직 로딩 중이구나" 하고 바로 되돌아가기!
        // 이 코드가 없어서 아까 에러가 난 겁니다.
        if (string.IsNullOrEmpty(newName)) return;

        // 아까랑 똑같은 거면 무시 (최적화)
        if (currentActiveName == newName) return;

        DisableAll();
        currentActiveName = newName;

        // 디버그 추가,,,,
        Debug.Log($"[AR] 이미지 변경 감지! -> {newName}");


        // ui 매니저한테 알림
        if (UIManager.Instance != null)
        {
            UIManager.Instance.OnImageRecognized(newName);
        }

        if (newName.Contains("Starry"))
        {
            if (effectStarryNight != null) effectStarryNight.SetActive(true);
        }
        else if (newName.Contains("Scream"))
        {
            if (effectScream != null) effectScream.SetActive(true);
        }
        else if (newName.Contains("Mona"))
        {
            if (effectMonaLisa != null) effectMonaLisa.SetActive(true);
        }

        // [추가] 스탬프 매니저에게 "나 이거 찾았어!" 라고 보고
        if (StampManager.Instance != null)
        {
            StampManager.Instance.CollectStamp(newName);
        }

    } // updateContent 끝

    // close 버튼
    // close 버튼 (기능을 On/Off 토글로 변경)
    public void ToggleContent()
    {
        // [추가] 현재 StarryNight 이펙트가 켜져 있는지 확인하여 상태를 전환합니다.
        // (effectStarryNight.activeSelf로 현재 콘텐츠의 켜짐/꺼짐 상태를 확인합니다.)
        if (effectStarryNight != null && effectStarryNight.activeSelf)
        {
            // 현재 켜져 있으면: 모두 끕니다 (OFF)
            DisableAll();
        }
        else
        {
            // 현재 꺼져 있으면: 이미지 이름에 맞는 콘텐츠를 켭니다 (ON)
            currentActiveName = "";
            UpdateContent();
        }

        // [삭제] 기존의 'this.gameObject.SetActive(false);' 코드는 삭제합니다.
        // 이 코드가 AR Content Master 프리팹 자체를 비활성화시켜 콘텐츠가 영영 사라지게 만들었습니다.
    }

    void DisableAll()
    {
        if (effectStarryNight != null) effectStarryNight.SetActive(false);
        if (effectScream != null) effectScream.SetActive(false);
        if (effectMonaLisa != null) effectMonaLisa.SetActive(false);
    }
}