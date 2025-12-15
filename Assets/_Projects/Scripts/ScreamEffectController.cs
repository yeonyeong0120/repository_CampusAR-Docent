using UnityEngine;

public class ScreamEffectController : MonoBehaviour
{
    // 파티클 시스템을 여기에 연결해주세요.
    public ParticleSystem waveParticle;

    // [추가] 1. 오디오 소스 컴포넌트를 연결할 변수
    private AudioSource audioSource;

    // Awake에서 오디오 소스를 찾습니다.
    private void Awake()
    {
        // Obj_Scream에 AudioSource 컴포넌트가 붙어있다고 가정하고 찾습니다.
        audioSource = GetComponent<AudioSource>();

        // (선택) 만약 AudioSource가 없다면 경고 로그를 남깁니다.
        if (audioSource == null)
        {
            Debug.LogWarning("Obj_Scream에 AudioSource 컴포넌트가 없습니다. 비명 소리를 추가할 수 없습니다.");
        }
    }

    // 오브젝트가 켜질 때 (인식되었을 때)
    private void OnEnable()
    {
        if (waveParticle != null)
        {
            waveParticle.Play(); // 파티클 재생 시작!
        }

        // [추가] 2. 오디오도 재생 시작!
        if (audioSource != null)
        {
            // 오디오 클립이 설정되어 있고, 현재 재생 중이 아니라면 재생합니다.
            if (audioSource.clip != null && !audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
    }

    // 오브젝트가 꺼질 때 (인식이 끊기거나 토글로 꺼졌을 때)
    private void OnDisable()
    {
        if (waveParticle != null)
        {
            waveParticle.Stop(); // 파티클 재생 멈춤!
        }

        // [추가] 3. 오디오도 멈춤!
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}