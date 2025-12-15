using UnityEngine;

public class InfoBoard_Trigger : MonoBehaviour
{
    [Header("연결할 것들")]
    [Tooltip("어디에 가면 나올까요? (Point_Info_A)")]
    public Transform targetLocation;

    [Tooltip("누가 움직이나요? (Main Camera)")]
    public Transform arCamera;

    [Tooltip("띄울 안내판 (Info_Panel_A)")]
    public GameObject infoBoardUI;

    [Header("설정")]
    [Tooltip("몇 미터 내에 있으면 보여줄까요?")]
    public float showRadius = 2.0f; // 2미터

    void Update()
    {
        // 연결 안 됐으면 실행 안 함
        if (targetLocation == null || arCamera == null || infoBoardUI == null) return;

        // 거리 계산
        float distance = Vector3.Distance(arCamera.position, targetLocation.position);

        // 거리가 설정값보다 가까우면?
        if (distance <= showRadius)
        {
            // 아직 안 켜져 있다면 켠다!
            if (infoBoardUI.activeSelf == false)
                infoBoardUI.SetActive(true);
        }
        else
        {
            // 멀어지면 다시 끈다 (자동 사라짐 기능)
            if (infoBoardUI.activeSelf == true)
                infoBoardUI.SetActive(false);
        }
    }
}