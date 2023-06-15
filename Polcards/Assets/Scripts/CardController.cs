using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// help from: https://gamedevbeginner.com/how-to-move-an-object-with-the-mouse-in-unity-in-2d/

public class CardController : MonoBehaviour
{
    /*
     * The CardController controls screen card's movement, i.e.
     * clicking on cards
     * moving cards
     * displaying location frame
     * adding to hand
     */
    public GameObject GameController;

    GameObject card;
    Vector3 offset;

    Renderer CurrentCardFrame = null;
    bool isCardFrameActivated = false;

    Vector2 handPosition;

    void Start()
    {
        
    }

    void Update()
    {
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            Collider2D targetObject = Physics2D.OverlapPoint(mousePosition);
            if (targetObject) Debug.Log(targetObject.tag);
            if (targetObject && targetObject.tag=="card")
            {
                card = targetObject.transform.gameObject;
                offset = card.transform.position - mousePosition;
                handPosition = card.transform.position;
            }
            else if(targetObject && targetObject.tag == "cardToChoose")
            {
                //zmieniione aby dodaæ specjalne
                if(targetObject.GetComponent<Card>().name == "Bazdanna" || targetObject.GetComponent<Card>().name == "Hurbeza")
                {
                    GameController.GetComponent<GameController>().addSpecialCardToHand(targetObject.gameObject);
                }
                else
                {
                    GameController.GetComponent<GameController>().addCardToHand(targetObject.gameObject);
                }
                
            }
        }
        if (card)
        {
            card.transform.position = mousePosition + offset;


            Collider2D[] results = Physics2D.OverlapPointAll(mousePosition);

            isCardFrameActivated = false;
            foreach (Collider2D col in results)
            {
                Renderer ren = col.gameObject.GetComponent<Renderer>();
                if (ren && ren.tag == "locationCardsFrame")
                {
                    CurrentCardFrame = ren;
                    isCardFrameActivated = true;
                    break;
                }
            }

            if (isCardFrameActivated) CurrentCardFrame.enabled = true;
            else if(CurrentCardFrame) CurrentCardFrame.enabled = false;

        }
        if (Input.GetMouseButtonUp(0) && card)
        {
            bool cardBinded = false;

            Collider2D[] results = Physics2D.OverlapPointAll(mousePosition);
            foreach (Collider2D col in results)
            {
                Renderer ren = col.gameObject.GetComponent<Renderer>();
                if (ren && ren.tag=="locationCardsFrame")
                {
                    bool movePossible = ren.transform.parent.GetComponent<CardSlotController>().Bind(card);

                    if (movePossible)
                    {
                        cardBinded = true;
                    }

                    ren.enabled = false;
                }
            }

            if (!cardBinded) card.transform.position = handPosition;

            card = null;
        }
    }
}

