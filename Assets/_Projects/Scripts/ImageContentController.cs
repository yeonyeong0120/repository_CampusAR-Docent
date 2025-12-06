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

        // 2. 일단 모든 이펙트를 다 끕니다 (초기화)
        if (effectStarryNight != null) effectStarryNight.SetActive(false);
        if (effectScream != null) effectScream.SetActive(false);
        if (effectMonaLisa != null) effectMonaLisa.SetActive(false);

        // 3. 이름표를 확인해서 맞는 것만 켭니다 (Switch)
        if (imageName == "StarryNight") // 라이브러리 이름과 토씨 하나 틀리지 않고 같아야 함!
        {
            if (effectStarryNight != null) effectStarryNight.SetActive(true);
        }
        else if (imageName == "Scream")
        {
            if (effectScream != null) effectScream.SetActive(true);
        }

        // 디버깅용 로그 (나중에 지우세요)
        Debug.Log($"인식된 이미지: {imageName}");
    }
}