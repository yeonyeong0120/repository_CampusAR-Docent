using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
// NativeGallery를 쓰기 때문에 System.IO는 사용하지 않음

public class InstaFilterController : MonoBehaviour
{
    [Header("--- AR 연결 ---")]
    public ARFaceManager faceManager;
    public List<GameObject> maskPrefabs;

    [Header("--- UI 연결 ---")]
    public GameObject uiToHide;

    private int currentIdx = -1;

    void Start()
    {
        if (maskPrefabs.Count > 0) SwapFilter(0);
    }

    public void SwapFilter(int index)
    {
        if (index < 0 || index >= maskPrefabs.Count) return;
        if (currentIdx == index) return;

        currentIdx = index;

        faceManager.facePrefab = maskPrefabs[index];

        foreach (ARFace face in faceManager.trackables)
        {
            if (face.gameObject != null) Destroy(face.gameObject);
        }
    }

    public void TakePicture()
    {
        StartCoroutine(CaptureProcess());
    }

    IEnumerator CaptureProcess()
    {
        // 1. UI 숨기기
        uiToHide.SetActive(false);
        yield return new WaitForEndOfFrame();

        // 2. 화면 캡처
        Texture2D texture = ScreenCapture.CaptureScreenshotAsTexture();

        // 3. 갤러리 저장 (Native Gallery 플러그인 사용)
        // 수정됨: 변수(=)에 담지 않고 바로 실행합니다.
        NativeGallery.SaveImageToGallery(texture, "AR_Photos", "MyFace_{0}.png", (success, path) =>
        {
            Debug.Log(success ? "사진 저장 성공: " + path : "사진 저장 실패");
        });

        // 메모리 청소
        Destroy(texture);

        // 4. UI 다시 켜기
        uiToHide.SetActive(true);
    }
}