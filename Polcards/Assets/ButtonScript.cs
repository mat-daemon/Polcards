using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
    private int clickNumber = 0;
    public TextMeshPro text0;
    public TextMeshPro text1;
    public TextMeshPro text2;
    public TextMeshPro text3;
    public TextMeshPro text4;
    public TextMeshPro text4_2;
    public TextMeshPro text5;
    public TextMeshPro text6;
    public TextMeshPro text7;

    public GameObject frame0;
    public GameObject frame1;
    public GameObject image1;
    public GameObject image2;
    public GameObject frame2;

    public void Click()
    {
        clickNumber++;

        if(clickNumber == 1)
        {
            text1.gameObject.SetActive(true);
        }
        if (clickNumber == 2)
        {
            text2.gameObject.SetActive(true);
        }
        if (clickNumber == 3)
        {
            frame0.SetActive(false);
            frame1.SetActive(true);
            frame2.SetActive(true);
            image1.SetActive(true);
            image2.SetActive(true);
            text3.gameObject.SetActive(true);
        }
        if (clickNumber == 4)
        {
            text3.gameObject.SetActive(false);
            text4.gameObject.SetActive(true);
        }
        if (clickNumber == 5)
        {
            text4.gameObject.SetActive(false);
            text4_2.gameObject.SetActive(true);
        }
        if (clickNumber == 6)
        {
            text5.gameObject.SetActive(true);
        }
        if (clickNumber == 7)
        {
            text5.gameObject.SetActive(false);
            text6.gameObject.SetActive(true);
        }
        if (clickNumber == 8)
        {
            text4_2.gameObject.SetActive(false);
            text7.gameObject.SetActive(true);
        }
        if (clickNumber == 9)
        {
            SceneManager.LoadScene("StartScene");
        }
    }

    public void Skip()
    {
        SceneManager.LoadScene("StartScene");
    }
}
