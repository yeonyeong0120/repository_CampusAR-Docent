using UnityEngine;

public class ShakeEffect : MonoBehaviour
{
    // 떠는 강도 (숫자가 클수록 더 미친 듯이 떱니다)
    // 기획하신 0.01f도 좋고, 더 세게 하려면 0.02f나 0.03f로 올려보세요.
    public float shakeAmount = 0.02f;

    private Vector3 originPosition; // 원래 위치를 기억하는 변수

    void Start()
    {
        // 게임 시작할 때 "내 원래 자리는 여기다"라고 기억해둡니다.
        originPosition = transform.localPosition;
    }

    void Update()
    {
        // 매 프레임마다: [원래 위치] + [랜덤한 방향의 떨림]을 더해서 위치를 잡습니다.
        // (이렇게 해야 멀리 날아가지 않고 제자리에서 떱니다!)
        transform.localPosition = originPosition + Random.insideUnitSphere * shakeAmount;
    }
}