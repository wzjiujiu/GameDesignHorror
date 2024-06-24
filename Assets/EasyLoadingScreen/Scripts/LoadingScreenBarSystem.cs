using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class LoadingScreenBarSystem : MonoBehaviour {

    //public GameObject bar;
    //public Text loadingText;
    //public bool backGroundImageAndLoop;
    //public float LoopTime;
    //public GameObject[] backgroundImages;
    //[Range(0,1f)]public float vignetteEfectVolue; // Must be a value between 0 and 1
    AsyncOperation async;
    //Image vignetteEfect;
    public GameObject LoadingVideo;
    public GameObject MenuVideo;
    public GameObject Play;
    public GameObject PanelImage;
    public GameObject Exit;
    public GameObject Credits;

    public VideoClip loading_eyesVideoClip;

    private UnityEngine.SceneManagement.Scene scene;

    private bool isloaded=false;


    public void ExitGame()
    {
        Application.Quit();
    }

    void Start()
    {
        string sceneName = SceneManager.GetSceneByBuildIndex(1).name;
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
           scene = SceneManager.GetSceneAt(i);
            if (scene.name == sceneName)
            {
                isloaded = true;
            }
        }

        if(isloaded)
        {
            Debug.Log("YES");
        }
        else
        {
            Debug.Log("NO");
        }
        Cursor.visible = true;
    }


    public void loadingScreen (int sceneNo)
    {

        MenuVideo = GameObject.Find("MenuVideo");
        Play = GameObject.Find("Play");
        PanelImage = GameObject.Find("Panelmage");
        Exit = GameObject.Find("Exit");
        Credits = GameObject.Find("Credits");
        MenuVideo.SetActive(false);
        Play.SetActive(false);
        PanelImage.SetActive(false);
        Exit.SetActive(false);
        Credits.SetActive(false);
        LoadingVideo.gameObject.SetActive(true);
        StartCoroutine(WaitForVideoToFinish(sceneNo));

    }

 

    IEnumerator WaitForVideoToFinish(int sceneNo)
    {
        yield return new WaitForSeconds(8);
        StartCoroutine(Loading(sceneNo));
    }


 


    // Activate the scene 
    IEnumerator Loading (int sceneNo)
    {
        async = SceneManager.LoadSceneAsync(sceneNo);
        async.allowSceneActivation = false;

        // Continue until the installation is completed
        while (async.isDone == false)
        {

            if (async.progress == 0.9f)
            {
        
                async.allowSceneActivation = true;
            }
            yield return null;
        }
    }

}
