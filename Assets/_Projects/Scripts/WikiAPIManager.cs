using UnityEngine;
using UnityEngine.Networking; // 인터넷 통신을 위한 필수 도구
using System.Collections;
using System; // Action(콜백)을 쓰기 위해 필요

public class WikiAPIManager : MonoBehaviour
{
    public static WikiAPIManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    // [1] 위키백과가 보내주는 JSON 데이터 봉투(그릇)를 정의합니다.
    // 위키백과는 title, extract(요약문) 등을 보내줍니다.
    [Serializable]
    public class WikiResponse
    {
        public string title;
        public string extract; // 우리가 필요한 건 바로 이 '요약 설명'입니다!
    }

    // [2] 외부(UIManager)에서 호출할 함수
    // searchTitle: 검색할 제목 (예: "별이 빛나는 밤")
    // onResultFound: 찾으면 실행할 약속(함수)
    public void GetDescription(string searchTitle, Action<string> onResultFound)
    {
        StartCoroutine(GetRequest(searchTitle, onResultFound));
    }

    // [3] 실제 인터넷에 다녀오는 코루틴 (비동기 작업)
    IEnumerator GetRequest(string searchTitle, Action<string> onResultFound)
    {
        // 위키백과 URL 규칙: 띄어쓰기를 밑줄(_)로 바꿔야 함
        string safeTitle = searchTitle.Replace(" ", "_");

        // 한국어 위키백과 요약 API 주소
        string url = $"https://ko.wikipedia.org/api/rest_v1/page/summary/{safeTitle}";

        Debug.Log($"[API] 요청 보냄: {url}");

        // 유니티가 제공하는 웹 요청 도구 생성
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // 전송하고 응답이 올 때까지 기다림 (여기서 멈춤!)
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                // 실패했을 때 (인터넷 끊김, 오타 등)
                Debug.LogError($"[API Error] {webRequest.error}");
                // 실패하면 아무것도 안 함 (그냥 기본 하드코딩 설명 유지)
            }
            else
            {
                // 성공했을 때!
                // 1. 받은 텍스트(JSON)를 확인
                string jsonResult = webRequest.downloadHandler.text;
                Debug.Log($"[API Success] 받음: {jsonResult}");

                // 2. JSON 봉투를 뜯어서 C# 변수로 변환
                try
                {
                    WikiResponse response = JsonUtility.FromJson<WikiResponse>(jsonResult);

                    // 3. 내용물(extract)만 쏙 빼서 UI 매니저에게 전달
                    if (response != null && !string.IsNullOrEmpty(response.extract))
                    {
                        onResultFound(response.extract);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogWarning("JSON 파싱 실패 (데이터 형식이 다른가봐요): " + e.Message);
                }
            }
        }
    }
}