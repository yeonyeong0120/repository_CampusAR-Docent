// 예시 코드 (C# 스크립트) - ARNavigation* Hierarchy에 있는 어떤 오브젝트에 붙여도 됩니다.

using UnityEngine;

public class Hide : MonoBehaviour
{
    void Start()
    {
        // 씬 내의 모든 XR Map 부모 오브젝트를 찾습니다.
        var xrMaps = GameObject.FindObjectsOfType<Transform>();

        foreach (var mapTransform in xrMaps)
        {
            // XR Map 부모 객체의 자식들 중에서 '-vis'가 붙은 오브젝트를 찾아 비활성화합니다.
            if (mapTransform.parent != null && mapTransform.parent.name.Contains("XR Map"))
            {
                // 이름이 'vis'로 끝나는 오브젝트만 시각화라고 가정하고 처리
                if (mapTransform.name.EndsWith("-vis"))
                {
                    // 시각화 전용 자식 객체를 비활성화합니다.
                    mapTransform.gameObject.SetActive(false);
                    // 또는 mapTransform.GetComponent<MeshRenderer>().enabled = false; (렌더러만 끌 수도 있습니다)
                }
            }
        }
    }
}
