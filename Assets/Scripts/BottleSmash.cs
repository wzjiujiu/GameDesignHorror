using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottleSmash : MonoBehaviour
{
    private AudioSource audioplayer;
    private Rigidbody rb;
    private bool playSound = false;
    public GameObject bottleParent;
    void Start()
    {
        audioplayer = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    private void OnCollisionEnter(Collision collision)
    {
        if (playSound==false) {
            playSound = true;
            audioplayer.Play();
            rb.isKinematic = true;
            Destroy(bottleParent, 3);
        }
    }

}
