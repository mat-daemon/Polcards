using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GUIController : MonoBehaviour
{
    public Slider slider;

    public ParticleSystem steamEffect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void LoadGameScene()
    {
        StartCoroutine(LoadGame());
    }
    public void LoadTutorialScene()
    {
        SceneManager.LoadScene("TutorialScene");
    }
    public void LoadThanksScene()
    {
        SceneManager.LoadScene("ThanksScene");
    }

    IEnumerator LoadGame()
    {
        //for (int i = 0; i <= 100; i++)
        //{
        //    slider.value = i;
            yield return new WaitForSeconds(0.01f);
        //}

        SceneManager.LoadScene("GameScene");
    }

    public void PlaySteamEffect()
    {
        steamEffect.Play();
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}
