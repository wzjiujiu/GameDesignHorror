using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SprayScript : MonoBehaviour
{
    public Image sprayFill;
    public float sprayAmount = 1.0f;
    public float drainTime = 0.2f;

    // Start is called before the first frame update
    void OnEnable()
    {
        sprayFill.fillAmount = sprayAmount;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            sprayAmount-=drainTime*Time.deltaTime;
            sprayFill.fillAmount = sprayAmount;
        }
    }
}
