using UnityEngine;

public class GuideController : MonoBehaviour
{
    [Header("----[ UI 설정 ]----")]
    public GameObject guideUI;       // 아까 만든 GuidePanel 넣기

    [Header("----[ 목표 및 거리 설정 ]----")]
    public Transform targetObject;   // 'Entrance' 오브젝트 넣기
    public float showDistance = 5.0f; // 5미터 안으로 오면 보여주기 (조절 가능)

    private Transform mainCamera;

    void Start()
    {
        if (Camera.main != null) mainCamera = Camera.main.transform;

        // 시작할 때 안내문구 숨기기
        if (guideUI != null) guideUI.SetActive(false);
    }

    void Update()
    {
        if (mainCamera == null || targetObject == null) return;

        // 거리 계산 (내 위치 vs 목적지)
        float dist = Vector3.Distance(mainCamera.position, targetObject.position);

        // 설정한 거리보다 가까우면 켜고, 멀어지면 끄기
        if (dist <= showDistance)
        {
            if (!guideUI.activeSelf) guideUI.SetActive(true);
        }
        else
        {
            if (guideUI.activeSelf) guideUI.SetActive(false);
        }
    }
}