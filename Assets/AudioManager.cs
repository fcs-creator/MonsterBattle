using System.Collections.Generic;
using UnityEngine;

public enum BGM
{
    Battle,
    Result,
}

public enum SE
{
    None = -1,
    HitStrikeS,
    HitStrikeM,
    HitStrikeL,
    Parry,
    Stan,
    Guard,
    Dead,
    Jump,
    Land,
    WeaponShot,
    WeaponDrawing,
    CollideBody,
}

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource seSource;

    //BGM
    [SerializeField] AudioClip battle;
    [SerializeField] AudioClip result;

    //SE
    [SerializeField] AudioClip hitStrikeS;
    [SerializeField] AudioClip hitStrikeM;
    [SerializeField] AudioClip hitStrikeL;
    [SerializeField] AudioClip parry;
    [SerializeField] AudioClip stan;
    [SerializeField] AudioClip guard;
    [SerializeField] AudioClip dead;
    [SerializeField] AudioClip jump;
    [SerializeField] AudioClip land;
    [SerializeField] AudioClip weaponShot;
    [SerializeField] AudioClip weaponDrawing;
    [SerializeField] AudioClip collideBody;
    

    // AudioClipを管理するDictionary
    Dictionary<BGM, AudioClip> bgm;
    Dictionary<SE, AudioClip> se;

    private void Awake()
    {
        // シングルトンの設定
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        bgm = new Dictionary<BGM, AudioClip>()
        {
            { BGM.Battle, battle },
            { BGM.Result, result },
        };

        se = new Dictionary<SE, AudioClip>()
        {
            { SE.HitStrikeS, hitStrikeS },
            { SE.HitStrikeM, hitStrikeM },
            { SE.HitStrikeL , hitStrikeL },
            { SE.Parry, parry },
            { SE.Stan, stan },
            { SE.Guard, guard },
            { SE.Dead, dead },
            { SE.Land, land },
            { SE.Jump, jump },
            { SE.WeaponShot, weaponShot },
            { SE.WeaponDrawing, weaponDrawing },
            { SE.CollideBody, collideBody }
        };

        DontDestroyOnLoad(gameObject);
    }

    // BGMを再生するメソッド
    public void PlayBGM(BGM id)
    {
        if (bgmSource.clip == bgm[id]) return;
        bgmSource.clip = bgm[id];
        bgmSource.Play();
    }

    // BGMの停止
    public void StopBGM()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }
    }

    // 効果音を再生するメソッド
    public void PlaySE(SE id)
    {
        if (id == SE.None) return;

        seSource.PlayOneShot(se[id]);
    }
}
