using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    public static SceneController Instance;

    [Header("Loading UI")]
    public CanvasGroup loadingCanvasGroup; // ���� ȭ�� ������
    public float fadeDuration = 0.5f;

    private void Awake()
    {
        // �̱��� ���� // �� ������Ʈ�� ���� �ٲ� �ı����� ����
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // �� �̵� �� ��Ƴ���
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // ������ ���� �ε� ȭ���� ����
        loadingCanvasGroup.alpha = 0;
        loadingCanvasGroup.blocksRaycasts = false;
    }

    // �ܺο��� �� �Լ��� ȣ���ؼ� ���� �̵���
    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneRoutine(sceneName));
    }

    private IEnumerator LoadSceneRoutine(string sceneName)
    {
        // 1. ���̵� �ƿ� (���� ȭ���� ��Ÿ��) - "��ǰ ���� ���� ��ȯ ��..." 
        loadingCanvasGroup.blocksRaycasts = true; // ��ġ ����
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            loadingCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }
        loadingCanvasGroup.alpha = 1f;

        // 2. �񵿱� �� �ε� (�޸� ���� �� �ε�)
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false; // �ε� �� �� ������ ���

        while (!op.isDone)
        {
            // �ε��� 90% �̻� �Ǹ� ����
            if (op.progress >= 0.9f)
            {
                op.allowSceneActivation = true;
            }
            yield return null;
        }

        // 3. ���̵� �� (���� ȭ���� �����)
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            loadingCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }
        loadingCanvasGroup.alpha = 0f;
        loadingCanvasGroup.blocksRaycasts = false; // ��ġ ���
    }
}