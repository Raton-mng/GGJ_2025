using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class AudioManager : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private EventReference music;

    private FMOD.Studio.EventInstance musicInstance;

    public static AudioManager Instance;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        musicInstance = RuntimeManager.CreateInstance(music);
    }

    private void Start()
    {
        musicInstance.start();
    }

    private void OnDestroy()
    {
        DestroyEvent(musicInstance);
    }

    public void PauseMenuOpened()
    {
        musicInstance.setParameterByName("Pause_Menu", 1);
    }

    public void EndMusic()
    {
        musicInstance.setParameterByName("End_Music", 1);
    }

    public void SetShopParameter(int shopOpened)
    {
        musicInstance.setParameterByName("shop", shopOpened);
    }
    
    public void PauseMenuClosed()
    {
        musicInstance.setParameterByName("Pause_Menu", 0);
    }

    private void DestroyEvent(EventInstance eventInstance)
    {
        eventInstance.stop(STOP_MODE.ALLOWFADEOUT);
        eventInstance.release();
    }
}
