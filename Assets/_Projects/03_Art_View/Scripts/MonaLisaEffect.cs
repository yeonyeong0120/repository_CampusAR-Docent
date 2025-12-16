using UnityEngine;

public class MonaLisaEffect : MonoBehaviour
{
    [Header("설정")]
    public GameObject sunglasses;
    public float targetZ = -0.1f; // 목표 위치
    public float speed = 2.0f;    // 이동 속도 (빠르게 하려면 5.0f로 바꾸세요)

    private bool isMoving = false;

    // 액자가 켜지자마자(AR 인식되자마자) 바로 실행!
    void OnEnable()
    {
        isMoving = true;
    }

    void Update()
    {
        if (isMoving && sunglasses != null)
        {
            Vector3 currentPos = sunglasses.transform.localPosition;

            // Z축 이동 (앞뒤)
            float newZ = Mathf.MoveTowards(currentPos.z, targetZ, Time.deltaTime * speed);

            sunglasses.transform.localPosition = new Vector3(currentPos.x, currentPos.y, newZ);
        }
    }
}