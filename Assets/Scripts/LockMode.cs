using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class LockMode : MonoBehaviour
{
    private PostProcessVolume vol;
    public PostProcessProfile standard;
    public PostProcessProfile nightvision;
    public PostProcessProfile inventory;

    public GameObject nightvisionOverlay;
    public GameObject flashlightoverlay;

    public GameObject Terramondo1;
    public GameObject floodedgrounds;
    public GameObject inventoryMenu;

    private Light flashlight=null;

    private bool nightvisionEnabled=false;
    private bool flashlightoverlayEnabled=false;
    private bool mondo2=false;
 
    // Start is called before the first frame update
    void Start()
    {

        vol=GetComponent<PostProcessVolume>();
        flashlight=GameObject.Find("FlashLight").GetComponent<Light>();
        flashlight.enabled = false;
        inventoryMenu.SetActive(false);
        nightvisionOverlay.SetActive(false);
        vol.profile = standard;
        Terramondo1 = GameObject.Find("Terrain");
        floodedgrounds = GameObject.Find("FloodedGrounds");
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.N)) 
        {
            if (SaveScript.inventoryOpen == false)
            {
                if (nightvisionEnabled == false)
                {
                    vol.profile = nightvision;
                    nightvisionOverlay.GetComponent<NightVisionScript>().StartDrain();
                    nightvisionOverlay.SetActive(true);
                    nightvisionEnabled = true;
                    NightVisionOff();
                }
                else if (nightvisionEnabled == true)
                {
                    vol.profile = standard;
                    nightvisionOverlay.SetActive(false);
                    nightvisionOverlay.GetComponent<NightVisionScript>().StopDrain();
                    this.gameObject.GetComponent<Camera>().fieldOfView = 60;
                    nightvisionEnabled = false;

                }
            }
           

        }

        if (nightvisionEnabled == true)
        {
            NightVisionOff();
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            if(SaveScript.inventoryOpen == false)
            { 
            if (flashlight.enabled== false)
            {
                flashlight.enabled = true;
            }
            else if (flashlight.enabled == true)
            {
                flashlight.enabled = false;

            }
            }

        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            if (SaveScript.inventoryOpen == false)
            {
                inventoryMenu.SetActive (true);
                vol.profile = inventory;
                
                if (flashlight.enabled == true)
                {
                    flashlight.enabled = false;

                }
                if (nightvisionEnabled == true)
                {

                    vol.profile = standard;
                    nightvisionOverlay.SetActive(false);
                    nightvisionOverlay.GetComponent<NightVisionScript>().StopDrain();
                    this.gameObject.GetComponent<Camera>().fieldOfView = 60;
                    nightvisionEnabled = false;

                }

            }
            else if (SaveScript.inventoryOpen == true)
            {

                vol.profile = standard;
                inventoryMenu.SetActive(false);

            }


        }



    }



    private void NightVisionOff()
    {
        if(nightvisionOverlay.GetComponent<NightVisionScript>().batteypower<=0)
        {
            vol.profile = standard;
            nightvisionOverlay.SetActive(false);
            this.gameObject.GetComponent<Camera>().fieldOfView = 60;
            nightvisionEnabled = false;

        }
    }
}
