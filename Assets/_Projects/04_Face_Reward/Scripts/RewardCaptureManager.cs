using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class RewardCaptureManager : MonoBehaviour
{
    [Header("UI to Hide")]
    public GameObject uiCanvas; // 촬영 시 숨길 UI 전체 (버튼 등)

    [Header("Preview UI")]
    public GameObject previewPanel; // 찍은 사진 보여줄 패널
    public RawImage previewImage;   // 찍은 사진이 들어갈 이미지

    // 캡처 버튼에 연결할 함수
    public void OnCaptureButtonClick()
    {
        StartCoroutine(CaptureRoutine());
    }

    IEnumerator CaptureRoutine()
    {
        // 1. UI 숨기기 (버튼이 사진에 나오면 안 되니까)
        uiCanvas.SetActive(false);

        // 2. 화면 렌더링이 끝날 때까지 대기 (필수)
        yield return new WaitForEndOfFrame();

        // 3. 화면 전체를 텍스처로 변환
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
        texture.Apply();

        // 4. UI 다시 켜기
        uiCanvas.SetActive(true);

        // 5. 프리뷰 패널에 띄워서 확인시켜주기 (선택사항이지만 UX상 필수)
        ShowPreview(texture);

        // 6. 갤러리 저장 (Native Gallery 플러그인 없이 파일로 저장)
        // 주의: 실제 상용 앱은 Native Gallery 플러그인이 필요하지만, 3주 프로젝트엔 이걸로 퉁쳐라.
        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string filename = $"AR_Reward_{timestamp}.png";
        string path = Path.Combine(Application.persistentDataPath, filename);

        File.WriteAllBytes(path, texture.EncodeToPNG());
        Debug.Log("Saved to: " + path); // PC에서는 콘솔 확인, 모바일은 로그캣 확인
    }

    void ShowPreview(Texture2D texture)
    {
        if (previewPanel != null && previewImage != null)
        {
            previewPanel.SetActive(true);
            previewImage.texture = texture;
        }
    }

    public void ClosePreview()
    {
        if (previewPanel != null) previewPanel.SetActive(false);
    }
}