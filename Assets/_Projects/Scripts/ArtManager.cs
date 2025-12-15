using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation; // AR 기능 사용을 위해 필수
using UnityEngine.XR.ARSubsystems; // AR 서브시스템 사용

public class ArtManager : MonoBehaviour
{
    // [설명] Inspector에서 연결할 그림(3D 오브젝트)들
    // 헷갈리지 않게 변수 이름도 명확히 해두었습니다.
    [Header("Art Objects (3D Models)")]
    public GameObject objectMona;   // 모나리자 3D 오브젝트 연결
    public GameObject objectScream; // 절규 3D 오브젝트 연결
    public GameObject objectStarry; // 별이 빛나는 밤 3D 오브젝트 연결

    // [설명] 현재 추적 중인 이미지 정보를 담을 변수
    private ARTrackedImage trackedImage;

    // [테스트용] AR이 안 될 때 강제로 켜볼 변수 (Inspector에서 체크하면 켜짐)
    [Header("Test Mode (AR 안될 때 사용)")]
    public bool showMonaTest;
    public bool showScreamTest;
    public bool showStarryTest;

    void Awake()
    {
        // 이 스크립트가 붙은 객체(Gallery 프리팹)에서 ARTrackedImage 컴포넌트를 찾아옵니다.
        trackedImage = GetComponent<ARTrackedImage>();
    }

    void Update()
    {
        // 1. AR로 정상 작동 중일 때 (카메라가 이미지를 인식했을 때)
        if (trackedImage != null && trackedImage.trackingState == TrackingState.Tracking)
        {
            // 인식된 이미지의 이름을 가져옵니다. (예: "MonaLisa")
            string imageName = trackedImage.referenceImage.name;
            UpdateArt(imageName);
        }
        // 2. AR이 안 돼서 수동으로 테스트할 때 (Inspector 체크박스로 켜기)
        else
        {
            CheckTestMode();
        }
    }

    // [기능] 이름표(Reference Image Name)를 확인해서 맞는 오브젝트만 켜는 함수
    void UpdateArt(string name)
    {
        // 1. 일단 모든 3D 오브젝트를 다 끕니다 (초기화)
        objectMona.SetActive(false);
        objectScream.SetActive(false);
        objectStarry.SetActive(false);

        // 2. 이름이 일치하는 녀석만 콕 집어서 켭니다.
        // (알려주신 라이브러리 이름: MonaLisa, Scream, StarryNight)
        if (name == "MonaLisa")
        {
            objectMona.SetActive(true);
        }
        else if (name == "Scream")
        {
            objectScream.SetActive(true);
        }
        else if (name == "StarryNight")
        {
            objectStarry.SetActive(true);
        }
    }

    // [테스트 기능] 개발 중에 마우스로 강제로 켜보는 함수
    void CheckTestMode()
    {
        if (showMonaTest)
        {
            // 강제로 "MonaLisa"가 인식된 척 함수를 실행합니다.
            UpdateArt("MonaLisa");
            ResetTestoggles(); // 하나 켜지면 체크박스 다시 끄기
        }
        else if (showScreamTest)
        {
            UpdateArt("Scream");
            ResetTestoggles();
        }
        else if (showStarryTest)
        {
            UpdateArt("StarryNight");
            ResetTestoggles();
        }
    }

    // 체크박스가 계속 켜져 있지 않게 자동으로 꺼주는 함수
    void ResetTestoggles()
    {
        showMonaTest = false;
        showScreamTest = false;
        showStarryTest = false;
    }
}