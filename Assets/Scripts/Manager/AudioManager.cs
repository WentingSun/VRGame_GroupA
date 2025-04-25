using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [Range(0f, 1f)]
    [SerializeField] private float audioVolumn;
    [SerializeField] private AudioSource ManagerAudioSource; //default AudioSource



    void Start()
    {
        GameManager.OnGameStateChange += onGameStateChange;
        GameManager.OnPlayerStateChange += onPlayerStateChange;
        GameManager.OnGameEventSent += receivedGameEvent;
         
    }

    public void PlayAudioClip(AudioClip clip, bool isLoop = false)
    {
        ManagerAudioSource.volume = audioVolumn;
        ManagerAudioSource.clip = clip;
        ManagerAudioSource.loop = isLoop;
        ManagerAudioSource.Play();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        GameManager.OnGameStateChange -= onGameStateChange;
        GameManager.OnPlayerStateChange -= onPlayerStateChange;
        GameManager.OnGameEventSent -= receivedGameEvent;
    }

    private void onGameStateChange(GameState gameState)
    {
        if (gameState == GameState.GamePause)
        {

        }
        else if (gameState == GameState.GameStart)
        {

        }
    }

    private void onPlayerStateChange(PlayerState playerState)
    {
        if (playerState == PlayerState.TakingDamage)
        {
            if (GameManager.Instance.CurrentPlayerHealth == 0)
            {
                //这时候算是玩家死亡, 不用播放受伤音效, 如果没有死亡音效, 这if判断删了便是
                PlayAudio(Death);
            }
            else
            {
                //播放受伤音效
                PlayAudio(Hurt);
            }
        }
    }

    private void receivedGameEvent(GameEvent receivedEvent)
    {
        if (receivedEvent == GameEvent.ThreeComboHit)
        {
            //达到三连击的时候的音效在这播放
            PlayAudio(Hit3);
        }
        else if (receivedEvent == GameEvent.ProtectShellBreak)
        {
            //护盾被破坏时的音效在这添加
            PlayAudio(Shield_Break);
        }
    }

    public static string Death = "death"; // 死亡
    public static string Hurt = "hurt"; // 受伤
    public static string Hit = "hit"; // 攻击命中音效
    public static string Hit3 = "hit3"; // 3连击命中音效
    public static string Shield_Break = "shield_break3"; // 护盾破坏
    public static string Ball_Appear = "ball_appear"; // 小球出现
    public static string Ball_Hit = "ball_hit"; // 小球命中
    public static string Ball_Bomb = "ball_bomb"; // 小球暴炸
    public static string Recursion="recursion"; // 复活


    // 播放音效
    public void PlayAudio(string name, bool isLoop = false)
    {
        var ac=Resources.Load<AudioClip>("Audio/" + name);
        if (ac == null)
        {
            return;
        }
        AudioSource source = GetAudioSourceCache();
        source.clip = ac;
        source.loop = isLoop;
        source.Play();

        if (!isLoop)
        {
            StartCoroutine(OnPlayEnd(source));
        }
    }

    private IEnumerator OnPlayEnd(AudioSource source)
    {
        var ac=source.clip;
        yield return new WaitForSeconds(ac.length);
        AddAudioSourceCache(source);
    }

    private GameObject _audioRoot;
    private Queue<AudioSource> _audioSourceCache = new Queue<AudioSource>();
    private AudioSource GetAudioSourceCache()
    {
        if (_audioRoot == null)
        {
            _audioRoot = new GameObject("AudioRoot");
        }
        if (_audioSourceCache.Count > 0)
        {
            return _audioSourceCache.Dequeue();
        }
        var go = new GameObject("Audio");
        go.transform.SetParent(_audioRoot.transform);
        var s = go.AddComponent<AudioSource>();
        return s;
    }
    private void AddAudioSourceCache(AudioSource audioSource)
    {
        _audioSourceCache.Enqueue(audioSource);
    }
}
