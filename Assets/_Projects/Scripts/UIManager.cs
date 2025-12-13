using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance; // 어디서든 부를 수 있게 싱글톤

    [Header("UI 연결")]
    public GameObject scanGuidePanel; // 스캔 가이드 전체 패널
    public GameObject showDescButton; // 왼쪽 아래 설명 버튼

    private void Awake()
    {
        Instance = this;
    }

    // 이미지가 인식되면 호출될 함수
    public void OnImageRecognized(string artworkName)
    {
        // 1. 스캔 가이드가 켜져 있다면 끈다 (Fade Out 효과는 나중에, 일단 끄기)
        if (scanGuidePanel.activeSelf)
        {
            scanGuidePanel.SetActive(false);
        }

        // 2. 설명 보기 버튼을 켠다
        if (!showDescButton.activeSelf)
        {
            showDescButton.SetActive(true);
        }

        // 3. (나중 구현) artworkName을 받아서 Bottom Sheet 내용을 미리 바꿔둔다.
        // ex) BottomSheet.SetText(artworkName);
    }

    // 다시 스캔 모드로 돌아갈 때 (닫기 버튼 등)
    public void ResetToScanMode()
    {
        scanGuidePanel.SetActive(true);
        showDescButton.SetActive(false);
        // + 열려있는 Bottom Sheet도 닫아야 함
    }
}