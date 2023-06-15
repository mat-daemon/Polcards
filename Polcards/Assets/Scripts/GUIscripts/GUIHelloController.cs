using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GUIHelloController : MonoBehaviour
{
    public GameObject GameBanner;
    public Animator[] animators;

    void Start()
    {
        StartCoroutine(SayHello());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator SayHello()
    {
        yield return new WaitForSeconds(2);

        int iteration = 1;
        foreach (var animator in animators)
        {
            string gearParam = "gear" + iteration.ToString();
            animator.SetBool(gearParam, true);

            iteration++;
        }

        yield return  new WaitForSeconds(3);
        SceneManager.LoadScene("StoryScene");
    }
}
