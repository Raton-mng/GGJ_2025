using System;
using UnityEngine;
using UnityEngine.UI;

public class VolumeMenu : MonoBehaviour
{
    [SerializeField] private GameObject volumeMenu;
    [SerializeField] private Slider soundtrackSlider;
    [SerializeField] private Slider sfxSlider; 
    
    private FMOD.Studio.Bus soundtrackBus;
    private FMOD.Studio.Bus sfxBus; //  define a volume controler

    private bool sfxIsClicked;
    
    void Start()
    {
        soundtrackBus = FMODUnity.RuntimeManager.GetBus("bus:/Music");
        sfxBus = FMODUnity.RuntimeManager.GetBus("bus:/SFX");
    }
    
    public void ChangeSoundtrackVolume(Single value)
    {
        soundtrackBus.setVolume(value);
    }


    public void ChangeSfxVolume(Single value)
    {
        sfxIsClicked = true;
        sfxBus.setVolume(value);
    }

    private void Update()
    {
        // Check if the left mouse button (0) is released
        if (Input.GetMouseButtonUp(0) && sfxIsClicked)
        {
            sfxIsClicked = false;
            FMODUnity.RuntimeManager.PlayOneShot("event:/TODO"); // Joue un SFX au pif
        }
    }

    public void OnVolumeControlButton()
    {
        volumeMenu.SetActive(true);
    }
    
    public void OnVolumeControlOpen()
    {
        volumeMenu.SetActive(true);
    }
    
    public void OnVolumeControlQuit()
    {
        volumeMenu.SetActive(false);
    }
}
