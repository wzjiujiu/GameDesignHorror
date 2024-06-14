using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreathingScript : MonoBehaviour


{
    private AudioSource audioPlayer;
    private bool heavyBreathing=false;
    // Start is called before the first frame update
    void Start()
    {
        audioPlayer = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if(SaveScript.mental<20 && heavyBreathing==false)
        {
            heavyBreathing = true;
            audioPlayer.Play();
        }
        if (SaveScript.mental > 19) 
        { 
            heavyBreathing = false;
            audioPlayer.Stop();
        }
    }
}
