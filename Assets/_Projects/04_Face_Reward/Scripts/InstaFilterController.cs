using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // 이미지 처리를 위해 필수
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;

// [새로 추가됨] 프리팹과 아이콘을 한 쌍으로 묶는 정보
[System.Serializable]
public class FilterItem
{
    public string name;       // 알아보기 쉽게 이름 (옵션)
    public GameObject prefab; // 가면 프리팹
    public Sprite icon;       // 버튼에 보여줄 아이콘 이미지
}

public class InstaFilterController : MonoBehaviour
{
    [Header("--- AR 필수 연결 ---")]
    public ARFaceManager faceManager;
    public ARCameraManager cameraManager;

    [Header("--- UI 연결 ---")]
    public GameObject uiToHide;
    public Transform contentArea;
    public GameObject itemButtonPrefab;

    [Header("--- 데이터 목록 (프리팹 + 아이콘) ---")]
    // [수정됨] 기존 List<GameObject> 대신 새로 만든 FilterItem 리스트 사용
    public List<FilterItem> bgItems = new List<FilterItem>();
    public List<FilterItem> assetItems = new List<FilterItem>();

    private int currentIdx = -1;
    private bool isBgMode = true;

    void Start()
    {
        ShowBackgrounds(); // 시작 시 배경 탭 보여주기
    }

    // --- 탭 버튼 함수 ---
    public void ShowBackgrounds()
    {
        isBgMode = true;
        GenerateButtons(bgItems);
    }

    public void ShowAssets()
    {
        isBgMode = false;
        GenerateButtons(assetItems);
    }

    // --- 버튼 동적 생성 로직 (핵심 수정 부분) ---
    void GenerateButtons(List<FilterItem> targetList)
    {
        // 1. 기존 버튼 삭제
        foreach (Transform child in contentArea)
        {
            Destroy(child.gameObject);
        }

        // 2. 새 버튼 생성 및 아이콘 설정
        for (int i = 0; i < targetList.Count; i++)
        {
            int index = i;
            GameObject newBtn = Instantiate(itemButtonPrefab, contentArea);

            // [핵심] 버튼의 Image 컴포넌트를 찾아 아이콘 스프라이트 교체
            Image btnImage = newBtn.GetComponent<Image>();
            if (btnImage != null && targetList[i].icon != null)
            {
                btnImage.sprite = targetList[i].icon;
            }

            // (옵션) 텍스트가 있다면 지우거나 숨김 처리
            TMP_Text btnText = newBtn.GetComponentInChildren<TMP_Text>();
            if (btnText != null) btnText.text = ""; // 텍스트 비우기

            // 클릭 이벤트 연결
            newBtn.GetComponent<Button>().onClick.AddListener(() => OnItemClicked(index));
        }
    }

    // --- 아이템 클릭 시 실행 ---
    void OnItemClicked(int index)
    {
        // 현재 모드에 맞는 리스트에서 프리팹 가져오기
        GameObject selectedPrefab = isBgMode ? bgItems[index].prefab : assetItems[index].prefab;
        ApplyFilter(selectedPrefab);
    }

    void ApplyFilter(GameObject prefab)
    {
        if (faceManager == null) return;

        faceManager.facePrefab = prefab;

        // 기존 얼굴 삭제 후 즉시 재생성 (필터 교체 효과)
        foreach (ARFace face in faceManager.trackables)
        {
            if (face.gameObject != null) Destroy(face.gameObject);
        }
    }

    // --- 카메라 및 촬영 기능 (기존 동일) ---
    public void SwitchCamera()
    {
        if (cameraManager == null) return;
        cameraManager.requestedFacingDirection =
            (cameraManager.requestedFacingDirection == CameraFacingDirection.User) ?
            CameraFacingDirection.World : CameraFacingDirection.User;
    }

    public void TakePicture() { StartCoroutine(CaptureProcess()); }

    IEnumerator CaptureProcess()
    {
        uiToHide.SetActive(false);
        yield return new WaitForEndOfFrame();
        Texture2D texture = ScreenCapture.CaptureScreenshotAsTexture();
        NativeGallery.SaveImageToGallery(texture, "AR_Photos", "MyFace_{0}.png", (success, path) => { Debug.Log(success ? "저장 성공" : "실패"); });
        Destroy(texture);
        uiToHide.SetActive(true);
    }
}