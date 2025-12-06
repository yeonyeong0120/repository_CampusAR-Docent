using UnityEngine;
using UnityEngine.XR.ARFoundation; // AR 기능 필수

public class ImageContentController : MonoBehaviour
{
    [Header("여기에 각 작품별 오브젝트를 연결하세요")]
    public GameObject effectStarryNight; // 별이 빛나는 밤 (UFO)
    public GameObject effectScream;      // 절규 (나중에 추가)
    public GameObject effectMonaLisa;    // 모나리자 (나중에 추가)

    private ARTrackedImage trackedImage;
    private bool isInitialized = false;

    void Awake()
    {
        // 내 몸에 붙어있는 ARTrackedImage 컴포넌트를 찾아옴
        trackedImage = GetComponent<ARTrackedImage>();
    }

    void Update()
    {
        // 1. 이미 초기화가 끝났으면 더 이상 검사하지 않음 (성능 최적화)
        if (isInitialized) return;

        // 2. ARTrackedImage 컴포넌트가 없거나, 아직 referenceImage 데이터가 안 들어왔으면 대기
        if (trackedImage == null)
        {
            trackedImage = GetComponent<ARTrackedImage>();
            return;
        }

        if (trackedImage.referenceImage == null) return; // 데이터가 준비될 때까지 기다림

        // 3. 드디어 이름(데이터)이 들어왔다면 컨텐츠 갱신 실행!
        UpdateContent();
    }

    void UpdateContent()
    {
        // 등록된 이미지의 이름 가져오기
        string imageName = trackedImage.referenceImage.name;

        // 이름이 비어있으면(Null) 아직 준비 중인 것이니 멈춤! (여기서 에러 방지)
        if (string.IsNullOrEmpty(imageName))
        {
            return;
        }

        Debug.Log($"[AR] 프리팹 생성됨! 인식된 이미지: {imageName}");

        // 혹시 연결 안 된 오브젝트가 있어도 에러 안 나게 체크
        if (effectStarryNight != null) effectStarryNight.SetActive(false);
        if (effectScream != null) effectScream.SetActive(false);
        if (effectMonaLisa != null) effectMonaLisa.SetActive(false);

        // 이름 비교 및 켜기
        if (imageName.Contains("Starry"))
        {
            if (effectStarryNight != null) effectStarryNight.SetActive(true);
        }
        else if (imageName.Contains("Scream"))
        {
            if (effectScream != null) effectScream.SetActive(true);
        }
        else if (imageName.Contains("Mona"))
        {
            if (effectMonaLisa != null) effectMonaLisa.SetActive(true);
        }

        // 여기까지 무사히 왔으면 초기화 완료!
        isInitialized = true;
    }


} // class