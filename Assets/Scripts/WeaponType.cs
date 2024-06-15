using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponType : MonoBehaviour
{
    // Start is called before the first frame update
    public enum typeOfWeapon
    {
        knife,
        cleaver,
        bat,
        pistol,
        shotgun,
        SparyCan,
        Bottle
    }

    public typeOfWeapon chooseWeapon;


}
