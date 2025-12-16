using UnityEngine;
using UnityEngine.XR.ARFoundation; // AR 기능 가져오기

public class UpdateStencil : MonoBehaviour
{
    public Material arBackgroundMaterial;       // 우리가 만든 마테리얼
    public AROcclusionManager occlusionManager; // 사람 인식 매니저

    void Update()
    {
        // 매 프레임마다 사람 모양(Stencil) 텍스처가 있는지 확인하고
        if (occlusionManager.humanStencilTexture != null)
        {
            // 마테리얼의 "HumanStencil"이라는 구멍에 넣어줍니다.
            arBackgroundMaterial.SetTexture("HumanStencil", occlusionManager.humanStencilTexture);
        }
    }
}