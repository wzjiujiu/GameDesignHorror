using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    // Start is called before the first frame update

    private bool canDamage = false;
    private Collider col;
    private Animator bloodEffect;
    public int damageAmt = 3;
    private AudioSource hitSound;

    void Start()
    {
        col= GetComponent<Collider>();
        bloodEffect=GameObject.Find("Blood").GetComponent<Animator>();
        hitSound=GetComponent<AudioSource>();

        
    }

    // Update is called once per frame
    void Update()
    {
        if(col.enabled==false)
        {
            canDamage = true;

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if(canDamage==true)
            {
                canDamage=false;
                SaveScript.health -= damageAmt;
                SaveScript.mental -= damageAmt;
                bloodEffect.SetTrigger("blood");
                hitSound.Play();
            }
        }
    }
}
