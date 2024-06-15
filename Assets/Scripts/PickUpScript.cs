using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using DialogueEditor;

public class PickUpScript : MonoBehaviour
{
    // Start is called before the first frame update

    private RaycastHit hit;
    public LayerMask excludeLayers;
    private int objId;
    public Image mainImage;
    public Sprite[] weaponIcons;
    public Sprite[] itemIcons;
    public Sprite[] ammoIcons;
    public Text mainTitle;
    public string[] weaponTitles;
    public string[] itemTitles;
    public string[] ammoTitles;
    public GameObject pickupPanel;
    public GameObject doorMessageObj;
    public Text doorMessage;
    public AudioClip[] pickupSounds;
    public RectTransform DialoguePanel;
    public TMPro.TextMeshProUGUI DialogueText;

    public float pickupDisplayDIstance = 3;
    private AudioSource audioPlayer;
    void Start()
    {
        pickupPanel.SetActive(false);
        audioPlayer=GetComponent<AudioSource>();
        doorMessageObj.SetActive(false);
        DialoguePanel.gameObject.SetActive(false);


    }

    // Update is called once per frame
    void Update()
    {
        if (SaveScript.itemAmts[9] == 1)
        {
            if (SaveScript.doorObject != null)
            {
                if ((int)SaveScript.doorObject.GetComponent<DoorType>().chooseDoor == 1)
                {
                    if (SaveScript.doorObject.GetComponent<DoorType>().locked == true)
                    {
                        SaveScript.doorObject.GetComponent<DoorType>().locked = false;

                    }
                }
            }

        }

        if (SaveScript.itemAmts[10] == 1)
        {
            if (SaveScript.doorObject != null)
            {
                if ((int)SaveScript.doorObject.GetComponent<DoorType>().chooseDoor == 2)
                {
                    if (SaveScript.doorObject.GetComponent<DoorType>().locked == true)
                    {
                        SaveScript.doorObject.GetComponent<DoorType>().locked = false;

                    }
                }
            }

        }
        if (Physics.SphereCast(transform.position,0.5f,transform.forward,out hit,30, ~excludeLayers))
        {
            if (Vector3.Distance(transform.position, hit.transform.position) < pickupDisplayDIstance)
            {
                if (hit.transform.gameObject.CompareTag("weapon"))
                {
                    pickupPanel.SetActive(true);
                    objId = (int)hit.transform.gameObject.GetComponent<WeaponType>().chooseWeapon;
                    mainImage.sprite = weaponIcons[objId];
                    mainTitle.text = weaponTitles[objId];

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        SaveScript.weaponAmts[objId]++;
                        Debug.Log(SaveScript.weaponAmts[objId]);
                        audioPlayer.clip = pickupSounds[3];
                        audioPlayer.Play();
                        SaveScript.change=true;
                        Destroy(hit.transform.gameObject, 0.2f);
                    }
                }
                else if (hit.transform.gameObject.CompareTag("item"))
                {
                    pickupPanel.SetActive(true);
                    objId = (int)hit.transform.gameObject.GetComponent<ItemsType>().chooseItem;
                    mainImage.sprite = itemIcons[objId];
                    mainTitle.text = itemTitles[objId];

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        SaveScript.itemAmts[objId]++;
                        audioPlayer.clip = pickupSounds[3];
                        audioPlayer.Play();
                        SaveScript.change = true;
                        Destroy(hit.transform.gameObject, 0.2f);
                    }
                }
                else if (hit.transform.gameObject.CompareTag("ammo"))
                {
                    pickupPanel.SetActive(true);
                    objId = (int)hit.transform.gameObject.GetComponent<AmmoType>().chooseAmmo;
                    mainImage.sprite = ammoIcons[objId];
                    mainTitle.text = ammoTitles[objId];

                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        SaveScript.ammoAmts[objId]++;
                        audioPlayer.clip = pickupSounds[3];
                        audioPlayer.Play();
                        SaveScript.change = true;
                        Destroy(hit.transform.gameObject, 0.2f);
                    }
                }
                else if (hit.transform.gameObject.CompareTag("door"))
                {
                    SaveScript.doorObject = hit.transform.gameObject;
                    Debug.Log(SaveScript.doorObject);
                    objId = (int)hit.transform.gameObject.GetComponent<DoorType>().chooseDoor;
                    doorMessageObj.SetActive(true);
                    doorMessage.text=hit.transform.gameObject.GetComponent<DoorType>().message;
                    DialogueText.text = "There is a door maybe is there something inside..let'see";
                    DialoguePanel.gameObject.SetActive(true);
                   
                    if (hit.transform.gameObject.GetComponent<DoorType>().locked == true)
                    {
                        
                        hit.transform.gameObject.GetComponent<DoorType>().message = "Find the key to open " + hit.transform.gameObject.GetComponent<DoorType>().chooseDoor  ;

                        
                    }
                    if (hit.transform.gameObject.GetComponent<DoorType>().locked == false)
                    {

                        hit.transform.gameObject.GetComponent<DoorType>().message = "Press E to close the door ";
                    }


                    if (Input.GetKeyDown(KeyCode.E) && hit.transform.gameObject.GetComponent<DoorType>().locked==false)
                    {
                        audioPlayer.clip = pickupSounds[objId];
                        audioPlayer.Play();
                        if (hit.transform.gameObject.GetComponent<DoorType>().opened==false)
                        {
                            hit.transform.gameObject.GetComponent<DoorType>().message = "Press E to close the door";
                            hit.transform.gameObject.GetComponent<DoorType>().opened =true;
                            hit.transform.gameObject.GetComponent<Animator>().SetTrigger("Open");

                        }

                        else if (hit.transform.gameObject.GetComponent<DoorType>().opened == true)
                        {
                            hit.transform.gameObject.GetComponent<DoorType>().message = "Press E to open the door";
                            hit.transform.gameObject.GetComponent<DoorType>().opened = false;
                            hit.transform.gameObject.GetComponent<Animator>().SetTrigger("Close");

                        }

                        
                        
                    }
                }
                else if (hit.transform.gameObject.CompareTag("corridorDor"))
                {
                    SaveScript.doorObject = hit.transform.gameObject;
                    Debug.Log(SaveScript.doorObject);
                    objId = (int)hit.transform.gameObject.GetComponent<DoorType>().chooseDoor;
                    doorMessageObj.SetActive(true);
                    doorMessage.text = hit.transform.gameObject.GetComponent<DoorType>().message;
                    DialogueText.text = "There is a door maybe is there something inside..let'see";
                    DialoguePanel.gameObject.SetActive(true);

                    if (hit.transform.gameObject.GetComponent<DoorType>().locked == true)
                    {

                        hit.transform.gameObject.GetComponent<DoorType>().message = "Find the key to open " + hit.transform.gameObject.GetComponent<DoorType>().chooseDoor;


                    }
                    if (hit.transform.gameObject.GetComponent<DoorType>().locked == false)
                    {

                        hit.transform.gameObject.GetComponent<DoorType>().message = "Press E to close the door ";
                    }


                    if (Input.GetKeyDown(KeyCode.E) && hit.transform.gameObject.GetComponent<DoorType>().locked == false)
                    {
                        audioPlayer.clip = pickupSounds[objId];
                        audioPlayer.Play();
                        if (hit.transform.gameObject.GetComponent<DoorType>().opened == false)
                        {
                            hit.transform.gameObject.GetComponent<DoorType>().message = "Press E to close the door";
                            hit.transform.gameObject.GetComponent<DoorType>().opened = true;
                            hit.transform.gameObject.GetComponent<Animator>().SetTrigger("Open");

                        }

                        else if (hit.transform.gameObject.GetComponent<DoorType>().opened == true)
                        {
                            hit.transform.gameObject.GetComponent<DoorType>().message = "Press E to open the door";
                            hit.transform.gameObject.GetComponent<DoorType>().opened = false;
                            hit.transform.gameObject.GetComponent<Animator>().SetTrigger("Close");

                        }



                    }
                }
            }
            else
            {
                pickupPanel.SetActive(false);
                doorMessageObj.SetActive(false);
                SaveScript.doorObject = null;
                DialoguePanel.gameObject.SetActive(false);
                
            }
            
            
        }
        
    }
}
