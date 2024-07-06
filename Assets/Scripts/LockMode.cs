using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using System;
using UnityEngine.SceneManagement;

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
    public GameObject optionMenu;

    private static System.Random random = new System.Random();
    private Light flashlight=null;

    private bool nightvisionEnabled=false;
    private bool flashlightoverlayEnabled=false;
    private bool mondo2=false;
    public GameObject point;
    public PostProcessProfile[] bliklist;
    Vignette vignette;
    private Animator animator;
    private bool hasClicked = false;
    private bool hasClicked1 = false;


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
        animator = GetComponent<Animator>();


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

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("i clicked escape");
            if (SaveScript.OptionOpen == false)
            {
                optionMenu.SetActive(true);
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
            else if (SaveScript.OptionOpen == true)
            {

                vol.profile = standard;
                optionMenu.SetActive(false);


            }

        }

        if ( SaveScript.reload == true)
        {

            vol.profile = standard;
            optionMenu.SetActive(false);
            SaveScript.reload = false;


        }



        if (SaveScript.inventoryOpen == true||SaveScript.OptionOpen==true)
        {
            Cursor.visible = true;
            point.SetActive(false);
        }
        else
        {
            Cursor.visible = false;
            point.SetActive(true);
            
        }


         if (Input.GetKeyDown(KeyCode.B) && !hasClicked)
        {
            // Trigger animation
            int randomNumber = random.Next(0, 4);
            vol.profile = bliklist[randomNumber];
            animator.SetTrigger("Blink");
            hasClicked = true; // Prevent multiple clicks
        }

        if (Input.GetKeyDown(KeyCode.P) )
        {
            // Trigger animation
            animator.SetTrigger("BlinkMouse_1");
            SaveScript.eyesclosed = true;
        }

        if (Input.GetKeyUp(KeyCode.P))
        {
            // Trigger animation
            int randomNumber = random.Next(0, 4);
            vol.profile = bliklist[randomNumber];
            animator.SetTrigger("BlinkMouse_2");
            SaveScript.eyesclosed = false;
          
        }



        // Check if the Blink animation has finished playing
        if (hasClicked  && !animator.GetCurrentAnimatorStateInfo(0).IsName("Blink") &&!animator.GetCurrentAnimatorStateInfo(0).IsName("BlinkMouse_1") && !animator.GetCurrentAnimatorStateInfo(0).IsName("BlinkMouse_1"))
        {
            hasClicked = false; // Reset hasClicked flag
            
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
