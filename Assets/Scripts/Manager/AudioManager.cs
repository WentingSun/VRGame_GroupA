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
            if (GameManager.Instance.CurrentPlayerHealth == 1)
            {
                //这时候算是玩家死亡, 不用播放受伤音效, 如果没有死亡音效, 这if判断删了便是
            }
            else
            {
                //播放受伤音效
            }
        }
    }

    private void receivedGameEvent(GameEvent receivedEvent)
    {
        if (receivedEvent == GameEvent.ThreeComboHit)
        {
            //达到三连击的时候的音效在这播放
        }
        else if (receivedEvent == GameEvent.ProtectShellBreak)
        {
            //护盾被破坏时的音效在这添加
        }
    }
}
