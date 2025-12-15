using UnityEngine;

public class SpotlightMover : MonoBehaviour
{
    [Header("설정")]
    public float moveRange = 100f; // 좌우로 움직일 거리 (픽셀 단위)
    public float speed = 1.0f;     // 움직이는 속도

    private Vector3 startPos;

    void Start()
    {
        // 시작할 때의 위치를 기준점으로 잡습니다.
        startPos = transform.localPosition;
    }

    void Update()
    {
        // PingPong이나 Sin을 써서 부드럽게 왔다갔다 하게 만듭니다.
        // Mathf.Sin은 -1 ~ 1 사이를 오르락내리락하는 파도 함수입니다.
        float x = Mathf.Sin(Time.time * speed) * moveRange;

        // 원래 위치 + 계산된 X값만큼 이동
        transform.localPosition = new Vector3(startPos.x + x, startPos.y, startPos.z);
    }
}