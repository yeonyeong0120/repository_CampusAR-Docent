using UnityEngine;

public class Stabilizer_MK2 : MonoBehaviour
{
    [Header("1. 떨림 방지 설정 (Stabilization)")]
    [Tooltip("숫자가 클수록 빠릿하게, 작을수록 부드럽게 따라갑니다. (추천: 8 ~ 10)")]
    public float smoothSpeed = 8.0f;

    [Header("2. 둥실둥실 설정 (Floating)")]
    [Tooltip("체크하면 둥실둥실 움직입니다.")]
    public bool enableFloating = true;
    public float floatSpeed = 1.5f; // 움직이는 속도
    public float floatRange = 0.2f; // 움직이는 범위

    [Header("3. 각도 조절 (Rotation Offset)")]
    [Tooltip("UFO가 이상하게 서있다면 여기서 각도를 조절하세요! (예: 90, 0, 0)")]
    public Vector3 rotationOffset = new Vector3(90, 0, 0);

    private Transform targetParent;

    void Awake()
    {
        // 태어날 때 부모님(AR 마커)을 기억합니다.
        targetParent = transform.parent;
    }

    void OnEnable()
    {
        // 켜질 때마다 부모님 위치로 순간이동해서 '짠' 하고 나타납니다.
        if (targetParent != null)
        {
            transform.position = targetParent.position;
            // 켜질 때 설정된 오프셋 각도로 초기화
            transform.rotation = targetParent.rotation * Quaternion.Euler(rotationOffset);
        }
    }

    void Start()
    {
        // 시작하자마자 부모님 품을 떠나 독립합니다. (그래야 부모가 떨 때 같이 안 떰)
        if (targetParent != null)
        {
            transform.SetParent(null);
        }
    }

    void Update()
    {
        // 부모님이 사라지면(인식 실패) 나도 사라집니다.
        if (targetParent == null)
        {
            Destroy(gameObject);
            return;
        }

        // 부모님 활성화 상태 동기화 (필요시 사용)
        if (gameObject.activeSelf != targetParent.gameObject.activeSelf)
        {
            // ImageContentController가 끄면 같이 꺼지게 하려면 여기에 로직 추가
        }

        // --- 1. 위치 계산 ---
        Vector3 targetPos = targetParent.position;

        // 둥실둥실 기능이 켜져있으면
        if (enableFloating)
        {
            // 부모의 오른쪽(Right) 방향으로 왔다 갔다 계산
            float sineWave = Mathf.Sin(Time.time * floatSpeed) * floatRange;
            targetPos += targetParent.right * sineWave;
        }

        // --- 2. 위치 적용 (Lerp) ---
        // 현재 위치에서 목표 위치로 부드럽게 이동
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smoothSpeed);

        // --- 3. 회전 적용 (Slerp + Offset) ---
        // 부모 회전값에 + 우리가 설정한 오프셋(rotationOffset)을 더해서 목표 회전값 계산
        Quaternion targetRot = targetParent.rotation * Quaternion.Euler(rotationOffset);

        // 부드럽게 회전
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * smoothSpeed);
    }
}