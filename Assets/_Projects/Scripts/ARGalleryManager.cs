using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.UI;
using System;

[System.Serializable]
public struct ArtInfo
{
    public string imageName;
    public GameObject popupPrefab;
}

public class ARGalleryManager : MonoBehaviour
{
    public ARTrackedImageManager imageManager;
    public List<ArtInfo> artList;
    public float popupDisplayTime = 3f; // ← 팝업 표시 시간 (초)

    private Dictionary<string, GameObject> spawnedPopups = new Dictionary<string, GameObject>();
    private Dictionary<string, Coroutine> popupTimers = new Dictionary<string, Coroutine>(); // ← 타이머 관리

    void OnEnable()
    {
        imageManager.trackedImagesChanged += OnImageChanged;
    }

    void OnDisable()
    {
        imageManager.trackedImagesChanged -= OnImageChanged;
    }

    void OnImageChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        // 1. 새로운 그림을 찾았을 때 (Added)
        foreach (var newImage in eventArgs.added)
        {
            string name = newImage.referenceImage.name;

            foreach (var art in artList)
            {
                if (art.imageName.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    GameObject popup = Instantiate(art.popupPrefab, newImage.transform);
                    popup.transform.localPosition = Vector3.zero;
                    popup.transform.localRotation = Quaternion.identity;
                    popup.transform.localScale = new Vector3(0.001f, 0.001f, 0.001f);

                    popup.SetActive(true);
                    foreach (Transform child in popup.GetComponentsInChildren<Transform>(true))
                    {
                        child.gameObject.SetActive(true);
                    }

                    Canvas popupCanvas = popup.GetComponentInChildren<Canvas>(true);
                    if (popupCanvas != null)
                    {
                        popupCanvas.renderMode = RenderMode.WorldSpace;

                        Camera mainCam = Camera.main;
                        if (mainCam != null)
                        {
                            popupCanvas.worldCamera = mainCam;
                        }

                        RectTransform canvasRect = popupCanvas.GetComponent<RectTransform>();
                        if (canvasRect != null)
                        {
                            canvasRect.localPosition = Vector3.zero;
                            canvasRect.localRotation = Quaternion.identity;
                            canvasRect.localScale = Vector3.one;
                            canvasRect.sizeDelta = new Vector2(500, 600);
                        }

                        Debug.Log("✅ Canvas 설정 완료!");
                    }

                    var images = popup.GetComponentsInChildren<UnityEngine.UI.Image>(true);
                    foreach (var img in images)
                    {
                        img.enabled = true;
                        Color c = img.color;
                        c.a = 1f;
                        img.color = c;
                    }

                    var texts = popup.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true);
                    foreach (var txt in texts)
                    {
                        txt.enabled = true;
                        Color c = txt.color;
                        c.a = 1f;
                        txt.color = c;
                    }

                    spawnedPopups[name] = popup;

                    // ⭐ 3초 타이머 시작
                    if (popupTimers.ContainsKey(name))
                    {
                        StopCoroutine(popupTimers[name]);
                    }
                    popupTimers[name] = StartCoroutine(HidePopupAfterDelay(name, popupDisplayTime));

                    Debug.Log($"🎨 [AR] {name} 인식 성공! {popupDisplayTime}초 후 자동 숨김.");
                }
            }
        }

        // 2. 계속 추적 중인 이미지 (Updated)
        foreach (var updatedImage in eventArgs.updated)
        {
            if (spawnedPopups.ContainsKey(updatedImage.referenceImage.name))
            {
                GameObject popup = spawnedPopups[updatedImage.referenceImage.name];

                // 추적 중이면 팝업 표시
                if (updatedImage.trackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
                {
                    popup.SetActive(true);

                    // ⭐ 다시 보이면 타이머 재시작
                    string name = updatedImage.referenceImage.name;
                    if (popupTimers.ContainsKey(name))
                    {
                        StopCoroutine(popupTimers[name]);
                    }
                    popupTimers[name] = StartCoroutine(HidePopupAfterDelay(name, popupDisplayTime));
                }
                else
                {
                    popup.SetActive(false);
                }
            }
        }

        // 3. 이미지가 추적을 잃었을 때 (Removed)
        foreach (var removedImage in eventArgs.removed)
        {
            string name = removedImage.referenceImage.name;
            if (spawnedPopups.ContainsKey(name))
            {
                // 타이머 중지
                if (popupTimers.ContainsKey(name))
                {
                    StopCoroutine(popupTimers[name]);
                    popupTimers.Remove(name);
                }

                Destroy(spawnedPopups[name]);
                spawnedPopups.Remove(name);
                Debug.Log($"🖼️ [AR] {name} 추적 상실. 설명창 제거.");
            }
        }
    }

    // ⭐ 3초 후 팝업 숨기는 루틴
    IEnumerator HidePopupAfterDelay(string imageName, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (spawnedPopups.ContainsKey(imageName))
        {
            spawnedPopups[imageName].SetActive(false);
            Debug.Log($"⏰ [AR] {imageName} 팝업 자동 숨김 ({delay}초 경과)");
        }

        popupTimers.Remove(imageName);
    }
}