using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class LockMode : MonoBehaviour
{
    private PostProcessVolume vol;
    public PostProcessProfile standard;
    public PostProcessProfile nightvision;
    public GameObject nightvisionOverlay;

    private bool nightvisionEnabled=false;
    // Start is called before the first frame update
    void Start()
    {
        vol=GetComponent<PostProcessVolume>();
        nightvisionOverlay.SetActive(false);
        vol.profile = standard;
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.N)) 
        {
            if (nightvisionEnabled == false)
            {
                vol.profile = nightvision;
                nightvisionOverlay.SetActive (true); 
                nightvisionEnabled = true;
            }
            else if (nightvisionEnabled == true) 
            {
                vol.profile = standard;
                nightvisionOverlay.SetActive(false);
                this.gameObject.GetComponent<Camera>().fieldOfView = 60;
                nightvisionEnabled = false;

            }
           

        }
    }
}
