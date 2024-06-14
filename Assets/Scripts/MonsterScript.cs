using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterScript : MonoBehaviour
{
   public enum MonsterType
    {
        shuffle,
        dizzy,
        alert
    }

    public enum MonsterState
    {
        Idle,
        Walking,
        Eating
    }
    public MonsterType monsterstyle;
    private Animator anim;
    public float yAdjustment = 0.0f;
    public MonsterState choosestate;
    public bool randomState = false;
    private int newState = 0;
    private int currentState;
    public float randomTiming = 5f;
    private NavMeshAgent agent;
    private GameObject[] targets;
    private float[] walkspeed = { 0.15f, 1.0f, 0.75f, 0.8f };
    private float distancetotarget;
    private int currentTarget = 0;
    private AnimatorStateInfo animInfo;
    private float distanceToPlayer;
    private GameObject player;
    public float zombieAlertRange = 20f;
    private bool awareofplayer = false;
    private bool adding = true;
    private AudioSource chasemusicPlayer;
    public float attackDistance = 2.5f;
    private float rotateSpeed = 2.5f;


    void Start()
    {
        anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        targets = GameObject.FindGameObjectsWithTag("target");
        player = GameObject.Find("FPSController");
        chasemusicPlayer = GameObject.Find("ChaseMusic").GetComponent<AudioSource>();

        anim.SetLayerWeight(((int)monsterstyle+1), 1);
        if(monsterstyle == MonsterType.shuffle)
        {
            transform.position=new Vector3(transform.position.x,transform.position.y+yAdjustment,transform.position.z);
        }
        anim.SetTrigger(choosestate.ToString());

        currentState = (int)choosestate;

        if(randomState ==true)
        {
            InvokeRepeating("SetAnimState", randomTiming, randomTiming);
        }

        agent.destination = targets[0].transform.position;
        agent.speed = walkspeed[(int)monsterstyle];
        
    }

    // Update is called once per frame
    void Update()
    {
        if (monsterstyle == MonsterType.shuffle)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + yAdjustment, transform.position.z);
        }
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        if(distanceToPlayer<=attackDistance)
        {
            agent.isStopped = true;
            anim.SetBool("Attacking", true);

            Vector3 pos=(player.transform.position-transform.position).normalized;

            Quaternion posRotation =Quaternion.LookRotation(new Vector3(pos.x,0,pos.z));

            transform.rotation = Quaternion.Slerp(transform.rotation,posRotation,rotateSpeed*Time.deltaTime);

        }
        else
        {
            anim.SetBool("Attacking", false);
            if (SaveScript.monsterChasing.Count > 0)
            {
                if (chasemusicPlayer.volume < 0.4f)
                {
                    if (chasemusicPlayer.isPlaying == false)
                    {
                        chasemusicPlayer.Play();
                    }
                    chasemusicPlayer.volume += 0.5f * Time.deltaTime;

                }
            }
            if (SaveScript.monsterChasing.Count == 0)
            {
                if (chasemusicPlayer.volume > 0.0f)
                {
                    chasemusicPlayer.volume -= 0.5f * Time.deltaTime;

                }
                if (chasemusicPlayer.volume == 0.0f)
                {
                    chasemusicPlayer.Stop();
                }
            }


            distancetotarget = Vector3.Distance(transform.position, targets[currentTarget].transform.position);


            if (distanceToPlayer < zombieAlertRange && choosestate == MonsterState.Walking)
            {
                agent.speed = 3.0f;
                agent.destination = player.transform.position;
                awareofplayer = true;
                if (adding == true)
                {
                    if (SaveScript.monsterChasing.Contains(this.gameObject))
                    {
                        adding = false;
                        return;
                    }
                    else
                    {
                        SaveScript.monsterChasing.Add(this.gameObject);
                        adding = false;
                    }
                }
            }

            if (distanceToPlayer > zombieAlertRange)
            {
                agent.speed = walkspeed[(int)monsterstyle];

                awareofplayer = false;
                if (SaveScript.monsterChasing.Contains(this.gameObject))
                {
                    SaveScript.monsterChasing.Remove(this.gameObject);
                    adding = true;
                }
            }


            animInfo = anim.GetCurrentAnimatorStateInfo((int)monsterstyle);

            if (animInfo.IsTag("motion"))
            {
                if (anim.IsInTransition((int)monsterstyle))
                {
                    agent.isStopped = true;
                }
            }
            if (choosestate == MonsterState.Walking)
            {

                if (distancetotarget < 1.5f)
                {
                    if (currentTarget < targets.Length - 1)
                    {
                        currentTarget++;

                    }
                    else
                    {
                        currentTarget = 0;
                        agent.destination = targets[currentTarget].transform.position;
                    }

                }
            }

        }

       

    }

    void SetAnimState()
    {

        if (awareofplayer == false)
        {


            newState = Random.Range(0, 3);
            if (newState != currentState)
            {
                choosestate = (MonsterState)newState;
                currentState = (int)choosestate;
                anim.SetTrigger(choosestate.ToString());
            }
        }

    }

    public void WalkOff()
    {
        agent.isStopped = true;
        
    }

    public void WalkOn()
    {
        agent.isStopped=false;
        agent.destination = targets[currentTarget].transform.position;
    }

  
}
