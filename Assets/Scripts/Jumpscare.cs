using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumpscare : MonoBehaviour
{
    public GameObject scare;
    public bool played = false;
    public bool trig = false;
    public AudioClip scareSound;

    // Start is called before the first frame update
    void Start()
    {
        trig = false;
        scare.GetComponent<Renderer>().enabled = false;
        

    }

    void OnTriggerEnter(Collider other)
    {
        AudioSource audio = GetComponent<AudioSource>();
        trig = true;
        audio.PlayOneShot(scareSound);
    }

    // Update is called once per frame
    void Update()
    {
        if (trig)
        {
            scare.GetComponent<Renderer>().enabled = true;
            if (!played)
            {
                played = true;
               
                Debug.Log("i am here");
                
            }
        
            StartCoroutine(RemoveOverTime());
         
        }
    }

    System.Collections.IEnumerator RemoveOverTime()
    {
        yield return new WaitForSeconds(2f);
        scare.GetComponent<Renderer>().enabled = false;
        Destroy(gameObject);
    }

   
}
