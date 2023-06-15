using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideButton : MonoBehaviour
{
    public GameObject cardChoice;
    public void Hide()
    {
        if (cardChoice.active)
        {
            cardChoice.SetActive(false);
        }
        else{
            cardChoice.SetActive(true);
        }
        this.gameObject.SetActive(true);

    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
