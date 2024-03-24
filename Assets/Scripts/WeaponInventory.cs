using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInventory : MonoBehaviour
{
    // Start is called before the first frame update

    public Sprite[] bigIcons;
    public Image bigIcon;
    public string[] titles;
    public Text title;

    public string[] descriptions;
    public Text description;

    private AudioSource audioPlayer;
    public AudioClip click, select;
    public GameObject BIGICON;
    private int choosenWeaponNumber;
    public Button[] weaponButtons;



    void Start()
    {
        audioPlayer = GetComponent<AudioSource>();
        bigIcon.sprite = bigIcons[0];
        title.text = titles[0];
        description.text = descriptions[0];

    }

    // Update is called once per frame
    void Update()
    {

        
    }

    private void OnEnable()
    {
        int i;
        for (i=0;i<weaponButtons.Length;i++)
        {
            if (SaveScript.weaponPickedUp[i] ==false)
            {
                weaponButtons[i].image.color = new Color(1, 1, 1, 0.06f);
                weaponButtons[i].image.raycastTarget = false;

            }
            if (SaveScript.weaponPickedUp[i] == true)
            {
                weaponButtons[i].image.color = new Color(1, 1, 1, 1);
                weaponButtons[i].image.raycastTarget = true;

            }
        }
    }

    public void ChooseWeapon(int weaponNumber)
    {
        if (weaponNumber == 4)
        {
            BIGICON.transform.localPosition = new Vector3(-150.6f, 207.7f, 0f);
        }
        else if (weaponNumber == 3) 
        {
            BIGICON.transform.localPosition = new Vector3(-81f, 207.7f, 0f);
        }
        else
        {
            BIGICON.transform.localPosition = new Vector3(-25.5f, 207.7f, 0f);
        }
        bigIcon.sprite = bigIcons[weaponNumber];
        title.text = titles[weaponNumber];
        description.text = descriptions[weaponNumber];  
        audioPlayer.clip = click;
        audioPlayer.Play();
        choosenWeaponNumber = weaponNumber;

    }
    public void AssignWeapon()
    {
        SaveScript.weaponID = choosenWeaponNumber;
        audioPlayer.clip = select;
        audioPlayer.Play();
    }
}
