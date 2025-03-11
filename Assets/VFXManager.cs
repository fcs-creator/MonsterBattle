using UnityEngine;
using UnityEngine.Rendering;

public enum VFX
{
    HitS,
    HitM,
    HitL,
    Guard,
    Dead,
}

public class VFXManager : MonoBehaviour
{
    public GameObject hitSPrefab;
    public GameObject hitMPrefab;
    public GameObject hitLPrefab;
    public GameObject guardPrefab;
    public GameObject deadPrefab;

    public static VFXManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // シーン間で保持する場合は以下を有効化
        // DontDestroyOnLoad(gameObject);
    }

    public void Play(VFX vfx, Vector3 position, Quaternion rotation)
    {
        switch (vfx)
        {
            case VFX.HitS:
                PlayVFX(hitSPrefab, position, GetRandomRotation(), Parameters.VFX_HIT_S_SCALE);
                AudioManager.Instance.PlaySE(SE.HitStrikeS);
                break;
            case VFX.HitM:
                PlayVFX(hitMPrefab, position, GetRandomRotation(), Parameters.VFX_HIT_M_SCALE);
                AudioManager.Instance.PlaySE(SE.HitStrikeM);
                break;
            case VFX.HitL:
                PlayVFX(hitLPrefab, position, GetRandomRotation(), Parameters.VFX_HIT_L_SCALE);
                AudioManager.Instance.PlaySE(SE.HitStrikeL);
                break;
            case VFX.Guard:
                PlayVFX(guardPrefab, position, rotation, Parameters.VFX_GUARD_SCALE);
                AudioManager.Instance.PlaySE(SE.Guard);
                break;
            case VFX.Dead:
                PlayVFX(deadPrefab, position, GetRandomRotation(), Parameters.VFX_DEAD_SCALE);
                AudioManager.Instance.PlaySE(SE.Dead);
                break;
            default:
                Debug.LogWarning("指定したVFXが存在しません！");
                break;
        }
    }


    void PlayVFX(GameObject vfxPrefab, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        GameObject vfx = Instantiate(vfxPrefab, position, rotation);
        vfx.transform.localScale = scale;
        ParticleSystem particleSystem = vfx.GetComponent<ParticleSystem>();

        if (particleSystem != null)
        {
            particleSystem.Play();
            Destroy(vfx, particleSystem.main.duration);
        }
        else
        {
            Debug.LogWarning("指定したVFXにParticleSystemが含まれていません！");
            Destroy(vfx, 2f); // パーティクルがない場合のデフォルト削除タイミング
        }
    }

    Quaternion GetRandomRotation()
    {
        return Quaternion.Euler(0, 0, Random.Range(0, 360));
    }
}
