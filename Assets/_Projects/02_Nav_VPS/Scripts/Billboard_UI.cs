using UnityEngine;

public class Billboard_UI : MonoBehaviour
{
    private Transform camTransform;

    void Start()
    {
        // 메인 카메라 찾기
        camTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        // 안내판이 카메라를 바라보게 회전
        // (단, UI는 뒤집혀 보일 수 있으므로 180도 돌려줍니다)
        transform.LookAt(transform.position + camTransform.rotation * Vector3.forward,
                         camTransform.rotation * Vector3.up);
    }
}