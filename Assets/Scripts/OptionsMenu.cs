using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.Audio;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] GameObject VisualPanel;
    [SerializeField] GameObject SoundsPanel;
    [SerializeField] GameObject DifficultyPanel;
    [SerializeField] GameObject SavePanel;
    [SerializeField] GameObject ControlPanel;
    [SerializeField] GameObject BTMPanel;
    public Slider LightSlider;
    public Toggle FogToggle;
    public Toggle AntiOff;
    public Toggle AntiFXXA;
    public Toggle AntiSMAA;
    public Toggle AntiTAA;
    private int AntiState = 4;
    private PostProcessVolume postProcessVolume;
    private PostProcessLayer postProcessLayer;
    public Slider AmbienceLevel;
    public Slider SFXlevel;
    public AudioMixer AmbienceMixer;
    public AudioMixer SFXMixer;

    void Start()
    {
        
        VisualPanel.gameObject.SetActive(true);
        SoundsPanel.gameObject.SetActive(false);
        DifficultyPanel.gameObject.SetActive(false);
        ControlPanel.gameObject.SetActive(false);
        BTMPanel.gameObject.SetActive(false);
        SavePanel.gameObject.SetActive(false);
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LightValue()
    {
        RenderSettings.ambientIntensity=LightSlider.value;
    }

    public void FogValue()
    {
        if (FogToggle.isOn)
        {
            RenderSettings.fog = true;
            RenderSettings.fogMode = FogMode.Exponential;
            RenderSettings.fogDensity = 0.15f;
            RenderSettings.fogColor = new Color32(0x6F, 0x6F, 0x6F, 0xFF);
        }
        else
        {
            RenderSettings.fog = false;
        }
    }

    public void Visuals()
    {
        VisualPanel.gameObject.SetActive(true);
        SoundsPanel.gameObject.SetActive(false);
        DifficultyPanel.gameObject.SetActive(false);
        ControlPanel.gameObject.SetActive(false);
        BTMPanel.gameObject.SetActive(false);
        SavePanel.gameObject.SetActive(false);
    }

    public void Sounds()
    {
        VisualPanel.gameObject.SetActive(false);
        SoundsPanel.gameObject.SetActive(true);
        DifficultyPanel.gameObject.SetActive(false);
        ControlPanel.gameObject.SetActive(false);
        BTMPanel.gameObject.SetActive(false);
        SavePanel.gameObject.SetActive(false);
    }

    public void Difficulty()
    {
        VisualPanel.gameObject.SetActive(false);
        SoundsPanel.gameObject.SetActive(false);
        DifficultyPanel.gameObject.SetActive(true);
        ControlPanel.gameObject.SetActive(false);
        BTMPanel.gameObject.SetActive(false);
        SavePanel.gameObject.SetActive(false);
    }

    public void Controls()
    {
        VisualPanel.gameObject.SetActive(false);
        SoundsPanel.gameObject.SetActive(false);
        DifficultyPanel.gameObject.SetActive(false);
        ControlPanel.gameObject.SetActive(true);
        BTMPanel.gameObject.SetActive(false);
        SavePanel.gameObject.SetActive(false);
    }

    public void BTM()
    {
        VisualPanel.gameObject.SetActive(false);
        SoundsPanel.gameObject.SetActive(false);
        DifficultyPanel.gameObject.SetActive(false);
        ControlPanel.gameObject.SetActive(false);
        BTMPanel.gameObject.SetActive(true);
        SavePanel.gameObject.SetActive(false);
    }

    public void Save()
    {
        VisualPanel.gameObject.SetActive(false);
        SoundsPanel.gameObject.SetActive(false);
        DifficultyPanel.gameObject.SetActive(false);
        ControlPanel.gameObject.SetActive(false);
        BTMPanel.gameObject.SetActive(false);
        SavePanel.gameObject.SetActive(true);
    }

    public void AntiAliasingOff()
    {
        postProcessLayer = Camera.main.GetComponent<PostProcessLayer>();
        if (AntiState != 1)
        {
            if (AntiOff.isOn == true)
            {
                postProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.None;
                AntiFXXA.isOn = false;
                AntiTAA.isOn = false;
                AntiSMAA.isOn = false;
                AntiState = 1;

            }
        }
    }

    public void AntiAliasingFXAA()
    {
        postProcessLayer = Camera.main.GetComponent<PostProcessLayer>();
        if (AntiState != 2)
        {
            if (AntiFXXA.isOn == true)
            {
                postProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.FastApproximateAntialiasing;
                AntiOff.isOn = false;
                AntiTAA.isOn = false;
                AntiSMAA.isOn = false;
                AntiState = 2;

            }
        }
    }

    public void AntiAliasingSMAA()
    {
        postProcessLayer = Camera.main.GetComponent<PostProcessLayer>();
        if (AntiState != 3)
        {
            if (AntiSMAA.isOn == true)
            {
                postProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
                AntiOff.isOn = false;
                AntiTAA.isOn = false;
                AntiFXXA.isOn = false;
                AntiState = 3;

            }
        }
    }

    public void AntiAliasingTAA()
    {
        postProcessLayer = Camera.main.GetComponent<PostProcessLayer>();
        if (AntiState != 4)
        {
            if (AntiTAA.isOn == true)
            {
                postProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.SubpixelMorphologicalAntialiasing;
                AntiOff.isOn = false;
                AntiSMAA.isOn = false;
                AntiFXXA.isOn = false;
                AntiState = 4;

            }
        }
    }

    public void AmbienceVolume()
    {
        AmbienceMixer.SetFloat("Volume", AmbienceLevel.value);
      
    }

    public void SFXVolume()
    {
        
        SFXMixer.SetFloat("Volume", SFXlevel.value);
    }

    private float ConvertToDecibel(float linear)
    {
        if (linear <= 0)
            return -80; // Mute
        return Mathf.Log10(linear) * 20;
    }

    private void ResetPostProcessingSettings()
    {
        postProcessLayer = Camera.main.GetComponent<PostProcessLayer>();
        postProcessLayer.antialiasingMode = PostProcessLayer.Antialiasing.None;
        AntiState = 4;
    }

    public void returntomainmenu()
    {
        ResetPostProcessingSettings();

        FirstPersonController.pausePanelSwitchedOn = false;
        FirstPersonController.inventorySwitchedOn = false;
        
  
        SaveScript.OptionOpen = false;
        SaveScript.reload = true;
  

        SceneManager.LoadScene(0);
    }
}
