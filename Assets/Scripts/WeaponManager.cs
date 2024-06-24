using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public enum weaponsSelect
    {
        knife,
        cleaver,
        bat,
        pistol,
        shotgun,
        SparyCan,
        Bottle,
        BottleWithLighter
    }

    public weaponsSelect chosenWeapon;
    //private int weaponId = 0;
    public GameObject[] weapons;
    private Animator anim;
    private AudioSource audioplayer;
    public AudioClip[] weaponSounds;
    private int currentWeaponID;
    private bool spraySoundOn = false;
    public GameObject sprayPanel;
    public static bool emptyBottleThrow = false;
    public static bool fireBottleThrow = false;
    private bool sprayEmpty = false;
    private bool stopSpray = false;

    // Start is called before the first frame update
    void Start()
    {
        SaveScript.weaponID = (int)chosenWeapon;
        Debug.Log(SaveScript.weaponID);
        anim=GetComponent<Animator>();
        audioplayer = GetComponent<AudioSource>();
        ChangeWeapons();
    }

    // Update is called once per frame
    void Update()
    {
        if(SaveScript.weaponID != currentWeaponID)
        {
            ChangeWeapons();
        }

        if(Input.GetMouseButtonDown(0))
        {
            if (SaveScript.inventoryOpen==false && SaveScript.OptionOpen == false)
            {
                if (SaveScript.currentAmmo[SaveScript.weaponID] > 0)
                {
                    anim.SetTrigger("Attack");
                    audioplayer.clip = weaponSounds[SaveScript.weaponID];
                    audioplayer.Play();
                    if(SaveScript.weaponID == 3||SaveScript.weaponID==4)
                    {
                        SaveScript.currentAmmo[SaveScript.weaponID]--;
                    }
                }
                else
                {
                    if (SaveScript.weaponID == 3 || SaveScript.weaponID == 4)
                    {
                        audioplayer.clip = weaponSounds[8];
                        audioplayer.Play();
                    }

                }

            }
        
        }
        if(Input.GetMouseButton(0) && sprayPanel.GetComponent<SprayScript>().sprayAmount>0.0f)
        {
            sprayEmpty = false;
            stopSpray = false;
            if(SaveScript.weaponID==5 && SaveScript.inventoryOpen == false && SaveScript.OptionOpen== false)
            {
                if (spraySoundOn == false)
                {
                    spraySoundOn = true;
                    anim.SetTrigger("Attack");
                    StartCoroutine(StartSpraySound());
                }

            }
        }
        if (Input.GetMouseButtonUp(0) || sprayPanel.GetComponent<SprayScript>().sprayAmount <= 0.0f)
        {
            if (SaveScript.weaponID == 5 && SaveScript.inventoryOpen == false && stopSpray==false && SaveScript.OptionOpen == false)
            {
                stopSpray = true;
                anim.SetTrigger("Release");
                spraySoundOn=false;
                audioplayer.Stop();
                audioplayer.loop = false;
                

            }
        }
        if(sprayPanel.GetComponent<SprayScript>().sprayAmount <= 0.0f && sprayEmpty==false)
        {
            sprayEmpty = true;
            SaveScript.weaponAmts[5]--;
            if (SaveScript.weaponAmts[6]==0)
            {
                SaveScript.weaponPickedUp[6] = false;
            }
        }

    }

    private void ChangeWeapons()
    {
        foreach(GameObject weapon in weapons)
        {
            weapon.SetActive(false);
        }
        weapons[SaveScript.weaponID].SetActive(true);
        chosenWeapon = (weaponsSelect)SaveScript.weaponID;
        anim.SetInteger("WeaponId", SaveScript.weaponID);
        anim.SetBool("WeaponChanged", true);
        currentWeaponID=SaveScript.weaponID;

        Move();
        StartCoroutine(WeaponReset());
    }

    private void Move()
    {
        switch(chosenWeapon)
        {
            case weaponsSelect.knife:
                transform.localPosition = new Vector3(0.02f, -0.193f, 0.66f);
                break;
            case weaponsSelect.cleaver:
                transform.localPosition = new Vector3(0.02f, -0.193f, 0.66f);
                break;

            case weaponsSelect.bat:
                transform.localPosition = new Vector3(0.02f, -0.193f, 0.66f);
                break;

            case weaponsSelect.pistol:
                transform.localPosition = new Vector3(0.02f, -0.193f, 0.66f);
                break;

            case weaponsSelect.shotgun:
                transform.localPosition = new Vector3(0.02f, -0.193f, 0.5f);
                break;
            case weaponsSelect.SparyCan:
                transform.localPosition = new Vector3(0.02f, -0.193f, 0.66f);
                break;
            case weaponsSelect.Bottle:
                transform.localPosition = new Vector3(0.02f, -0.193f, 0.66f);
                break;
        }
    }

    public void BottleThrowEmpty()
    {
        emptyBottleThrow = true;
    }

    public void BottleThrowFire()
    {
        fireBottleThrow = true;
    }

    public void LoadAnotherBottle()
    {
       if(SaveScript.weaponID==6)
        {
            ChangeWeapons();
        }
    }

    IEnumerator WeaponReset()
    {
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("WeaponChanged", false);
    }

    IEnumerator StartSpraySound()
    {
        yield return new WaitForSeconds(0.3f);
        audioplayer.clip = weaponSounds[SaveScript.weaponID];
        audioplayer.Play();
        audioplayer.loop = true;
    }
}
