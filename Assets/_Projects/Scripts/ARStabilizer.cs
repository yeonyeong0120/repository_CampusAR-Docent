using UnityEngine;

public class ARStabilizer : MonoBehaviour
{
    [Header("설정")]
    [Tooltip("숫자가 작을수록 더 부드럽지만 반응이 느려집니다. (추천: 5 ~ 10)")]
    public float smoothSpeed = 8.0f;

    private Transform targetParent; // 원래의 부모 (떨리는 AR 마커)
    private bool isDetached = false;

    void Awake()
    {
        // 1. 태어날 때 나의 원래 부모님(AR 마커)을 기억해둡니다.
        targetParent = transform.parent;
    }

    void OnEnable()
    {
        // 2. 켜질 때마다 부모님 위치로 순간이동! (안 그러면 멀리서 날아오는 게 보임)
        if (targetParent != null)
        {
            transform.position = targetParent.position;
            transform.rotation = targetParent.rotation;
        }
    }

    void Start()
    {
        // 3. 시작하자마자 부모님 품에서 독립합니다! (그래야 부모가 떨 때 같이 안 떰)
        if (targetParent != null)
        {
            transform.SetParent(null); // 부모 관계 끊기 (World Space로 이동)
            isDetached = true;
        }
    }

    void Update()
    {
        // 부모님이 사라지면(인식 실패 등) 나도 사라져야 함 (안전장치)
        if (targetParent == null)
        {
            Destroy(gameObject);
            return;
        }

        // 부모님이 꺼져있으면 나도 꺼져야 함 (ImageContentController와의 호환성)
        if (gameObject.activeSelf != targetParent.gameObject.activeSelf)
        {
            // 이 부분은 ImageContentController가 SetActive를 제어하므로
            // 굳이 여기서 건드리지 않아도 되지만, 혹시 몰라 남겨둡니다.
        }

        // ★ 핵심 기술: 부모님 위치로 "부드럽게(Lerp)" 따라갑니다.
        // 위치 보정
        transform.position = Vector3.Lerp(transform.position, targetParent.position, Time.deltaTime * smoothSpeed);

        // 회전 보정
        transform.rotation = Quaternion.Slerp(transform.rotation, targetParent.rotation, Time.deltaTime * smoothSpeed);
    }

    // 이 오브젝트가 파괴될 때 할 일
    void OnDestroy()
    {
        // 혹시라도 독립된 상태로 남을까 봐 처리
        if (isDetached && targetParent != null)
        {
            // 사실 AR 세션이 끝나면 다 같이 사라지니 크게 걱정 안 해도 됩니다.
        }
    }
}