using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NightVisionScript : MonoBehaviour
{

    private Image zoombar;
    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        zoombar=GameObject.Find("ZoomBar").GetComponent<Image>();
        cam=GameObject.Find("FirstPersonCharacter").GetComponent<Camera>();
        


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
    }
}
