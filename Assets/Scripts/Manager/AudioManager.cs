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
}
