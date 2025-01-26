using System;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class AudioManager : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private EventReference music;

    [Header("Snapshots")]
    [SerializeField] private EventReference normalSnapshot;
    [SerializeField] private EventReference pauseSnapshot;

    private FMOD.Studio.EventInstance musicInstance;
    private FMOD.Studio.EventInstance normalSnapshotInstance;
    private FMOD.Studio.EventInstance pauseSnapshotInstance;

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
        normalSnapshotInstance = RuntimeManager.CreateInstance(normalSnapshot);
        pauseSnapshotInstance = RuntimeManager.CreateInstance(pauseSnapshot);
    }

    private void Start()
    {
        musicInstance.start();
        normalSnapshotInstance.start();
    }

    private void OnDestroy()
    {
        DestroyEvent(musicInstance);
        DestroyEvent(normalSnapshotInstance);
        DestroyEvent(pauseSnapshotInstance);
    }

    public void PauseMenuOpened()
    {
        normalSnapshotInstance.stop(STOP_MODE.ALLOWFADEOUT);
        pauseSnapshotInstance.start();
    }

    public void EndMusic()
    {
        musicInstance.setParameterByName("End_Music", 1);
    }
    
    public void PauseMenuClosed()
    {
        pauseSnapshotInstance.stop(STOP_MODE.ALLOWFADEOUT);
        normalSnapshotInstance.start();
    }

    private void DestroyEvent(EventInstance eventInstance)
    {
        eventInstance.stop(STOP_MODE.ALLOWFADEOUT);
        eventInstance.release();
    }
}
