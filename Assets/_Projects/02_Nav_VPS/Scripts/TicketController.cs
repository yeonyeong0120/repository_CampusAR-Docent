using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TicketController : MonoBehaviour
{
    [Header("----[ UI 설정 ]----")]
    public GameObject ticketUI;      // 티켓 이미지 오브젝트
    public Button ticketButton;      // 티켓 이미지에 붙은 버튼

    [Header("----[ 목표 위치 설정 (직접 연결) ]----")]
    public Transform targetObject;   // ? 여기에 'Entrance' 오브젝트를 넣으세요!
    public float arrivalDistance = 3.0f; // 도착 거리 (3미터)

    [Header("----[ 이동할 씬 ]----")]
    public string nextSceneName = "Scene3";

    private Transform mainCamera;

    void Start()
    {
        // 카메라 찾기
        if (Camera.main != null) mainCamera = Camera.main.transform;

        // 버튼 클릭 연결
        if (ticketButton != null)
        {
            ticketButton.onClick.RemoveListener(OnTicketClicked); // 중복 방지
            ticketButton.onClick.AddListener(OnTicketClicked);
        }

        // 시작 시 티켓 숨기기
        if (ticketUI != null) ticketUI.SetActive(false);
    }

    void Update()
    {
        // 카메라나 목표물이 없으면 아무것도 안 함
        if (mainCamera == null || targetObject == null) return;

        // 매니저를 통하지 않고, 직접 거리 계산! (에러 원천 봉쇄)
        float dist = Vector3.Distance(mainCamera.position, targetObject.position);

        if (dist <= arrivalDistance)
        {
            // 거리 안에 들어오면 티켓 보여줌
            if (!ticketUI.activeSelf) ticketUI.SetActive(true);
        }
        else
        {
            // 멀어지면 숨김
            if (ticketUI.activeSelf) ticketUI.SetActive(false);
        }
    }

    // 클릭 시 실행
    void OnTicketClicked()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}