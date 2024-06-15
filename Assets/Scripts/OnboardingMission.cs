using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


public class OnboardingMission : MonoBehaviour
{

    //public GameObject bar;
    //public Text loadingText;
    //public bool backGroundImageAndLoop;
    //public float LoopTime;
    //public GameObject[] backgroundImages;
    //[Range(0,1f)]public float vignetteEfectVolue; // Must be a value between 0 and 1
    AsyncOperation asyncOp;
    //Image vignetteEfect;


    enum TutorialState { None, MoveForward, Completed }

    public float timeToComplete = 0.8f; // Time in seconds to hold W key

    private TutorialState currentState = TutorialState.None;
    private float timer = 0f;
    private bool holdingForward = false;
    public GameObject TutorialMessageObj;
    public Text TutorialMessage;

    void Start()
    {
        TutorialMessage.text = "Welcome to EOF";
        TutorialMessageObj.SetActive(true);
    }



    void Update()
    {
        switch (currentState)
        {
            case TutorialState.None:
                currentState = TutorialState.MoveForward;
                break;

            case TutorialState.MoveForward:
                TutorialMessage.text = "Press W to go forward";
                HandleForwardInput();
                if (holdingForward)
                {
                    timer += Time.deltaTime;
                    if (timer >= timeToComplete)
                    {
                        NextState();
                        timer = 0f;
                        break;
                    }
                }
               
                break;

            case TutorialState.Completed:
                TutorialMessage.text = "Misson completed Loafing new World...";


                break;
        }
        void HandleForwardInput()
        {
            if (Input.GetKey(KeyCode.W))
            {
                holdingForward = true;
            }
            else
            {
                holdingForward = false;
            }
        }




        void NextState()
        {
            currentState = TutorialState.Completed;
            timer = 0f;
        }

    

    }







    IEnumerator WaitForMissionToFinish(int sceneNo)
    {
        yield return new WaitForSeconds(8);
        StartCoroutine(Load(sceneNo));
    }





    // Activate the scene 
    IEnumerator Load(int sceneNo)
    {
        asyncOp = SceneManager.LoadSceneAsync(sceneNo);
        asyncOp.allowSceneActivation = false;

        // Continue until the installation is completed
        while (asyncOp.isDone == false)
        {

            if (asyncOp.progress == 0.9f)
            {

                asyncOp.allowSceneActivation = true;
            }
            yield return null;
        }
    }

}
