using UnityEngine;

public class ARToggleManager : MonoBehaviour
{
    [Header("각 작품의 이펙트 그룹을 연결하세요")]
    public GameObject starryNightGroup; // 별이 빛나는 밤 이펙트 (소리 포함)
    public GameObject munchGroup;       // 뭉크 이펙트 (소리 포함)
    public GameObject monaLisaGroup;    // 모나리자 이펙트 (썬글라스 포함)

    // 현재 보고 있는 작품의 이펙트를 저장할 변수
    private GameObject currentEffectGroup;

    void Start()
    {
        // 시작할 때는 아무것도 선택 안 함 (또는 기본값 설정)
        currentEffectGroup = null;
    }

    // [1] 작품이 인식되거나 화면이 넘어갈 때 이 함수들을 호출해주세요.
    public void SetContent_StarryNight()
    {
        currentEffectGroup = starryNightGroup;
        Debug.Log("현재 작품: 별이 빛나는 밤");
    }

    public void SetContent_Munch()
    {
        currentEffectGroup = munchGroup;
        Debug.Log("현재 작품: 뭉크");
    }

    public void SetContent_MonaLisa()
    {
        currentEffectGroup = monaLisaGroup;
        Debug.Log("현재 작품: 모나리자");
    }

    // [2] 토글 버튼(Btn_ToggleAR)에 연결할 함수입니다.
    public void ToggleCurrentEffect()
    {
        if (currentEffectGroup == null)
        {
            Debug.LogWarning("현재 선택된 작품이 없습니다!");
            return;
        }

        // 현재 켜져있으면 끄고, 꺼져있으면 켭니다 (반전)
        bool isActive = currentEffectGroup.activeSelf;
        currentEffectGroup.SetActive(!isActive);
    }
}