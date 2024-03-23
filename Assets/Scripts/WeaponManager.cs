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
        Bottle
    }

    public weaponsSelect chosenWeapon;
    //private int weaponId = 0;
    public GameObject[] weapons;
    private Animator anim;
    private AudioSource audioplayer;
    public AudioClip[] weaponSounds;
    private int currentWeaponID;

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
            if (SaveScript.inventoryOpen==false)
            {
                anim.SetTrigger("Attack");
                audioplayer.clip = weaponSounds[SaveScript.weaponID];
                audioplayer.Play();

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

    IEnumerator WeaponReset()
    {
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("WeaponChanged", false);
    }
}
