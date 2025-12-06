using UnityEngine;
using UnityEngine.XR.ARFoundation; // AR 기능 필수

public class ImageContentController : MonoBehaviour
{
    [Header("여기에 각 작품별 오브젝트를 연결하세요")]
    public GameObject effectStarryNight; // 별이 빛나는 밤 (UFO)
    public GameObject effectScream;      // 절규 (나중에 추가)
    public GameObject effectMonaLisa;    // 모나리자 (나중에 추가)

    private ARTrackedImage trackedImage;

    void Awake()
    {
        // 내 몸에 붙어있는 ARTrackedImage 컴포넌트를 찾아옴
        trackedImage = GetComponent<ARTrackedImage>();
    }

    void OnEnable()
    {
        // 이 프리팹이 생성되거나 켜질 때 실행됨
        UpdateContent();
    }

    void UpdateContent()
    {
        // 1. 현재 인식된 이미지의 이름을 알아냄 (Reference Image Library에 등록한 이름!)
        string imageName = trackedImage.referenceImage.name;

        // 디버깅용: 도대체 무슨 이름으로 인식되는지 로그로 찍어봅니다.
        Debug.Log($"[AR] 인식된 이미지 이름: '{imageName}'");

        // 일단 다 끕니다
        if (effectStarryNight != null) effectStarryNight.SetActive(false);
        if (effectScream != null) effectScream.SetActive(false);
        if (effectMonaLisa != null) effectMonaLisa.SetActive(false);

        // [수정된 부분] == (완전일치) 대신 Contains (포함)를 사용합니다!
        // "StarryNight", "the-starry-night", "StarryNight " 뭐든 다 통과됩니다.
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

    }
}