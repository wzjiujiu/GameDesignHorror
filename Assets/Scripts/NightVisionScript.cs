using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NightVisionScript : MonoBehaviour
{

    private Image zoombar;
    private Image batteryChunks;
    private Camera cam;

    public float batteypower = 1.0f;
    public float draintime = 2;
    // Start is called before the first frame update
    void Start()
    {
        zoombar=GameObject.Find("ZoomBar").GetComponent<Image>();
        batteryChunks = GameObject.Find("BatteryChunks").GetComponent<Image>();
        cam=GameObject.Find("FirstPersonCharacter").GetComponent<Camera>();
        InvokeRepeating("BatteryDrain", draintime, draintime);
        


    }

    private void OnEnable()
    {
        if (zoombar != null)
        {
            zoombar.fillAmount = 0.6f;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if(cam.fieldOfView>10)
            {
                cam.fieldOfView -= 5;
                zoombar.fillAmount = cam.fieldOfView / 100;
            }

        
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (cam.fieldOfView < 60)
            {
                cam.fieldOfView += 5;
                zoombar.fillAmount = cam.fieldOfView / 100;
            }


        }
        batteryChunks.fillAmount = batteypower;
    }

    private void BatteryDrain()
    {
        if (batteypower > 0.0f)
        {
            batteypower -= 0.25f;
        }
    }
}
