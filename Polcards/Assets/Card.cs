using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Card : MonoBehaviour
{
    public string name;
    public int power;
    public int[] symbols; // 0-okulary, 1-klepsydra, 2-kapelusz, 3-kostki
    public string[] abilities;

    //Sem_,_(,Op) - na koniec tury (u oponenta)
    //Nat_,_(,Op) - natychmiast (u oponenta)
    //SemOp_,_(,Op) - na koniec tury za symbole oponenta (u oponenta)
    //NatOp_,_(,Op) - natychmiast za symbole oponenta (u oponenta)

    private Color lowerColor = new Color(90, 0, 0);
    private TextMeshPro text;
    private Renderer renderer;
    private Renderer CardRenderer;
    private int previousPower;
    void Awake()
    {
        text = GetComponentInChildren<TextMeshPro>();
        if (text != null)
        {
            text.text = power.ToString();
            renderer = text.GetComponent<Renderer>();
            CardRenderer = GetComponent<Renderer>();
            renderer.sortingOrder = CardRenderer.sortingOrder;
            previousPower = power;
        }
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (text != null)
        {

            if(power > previousPower)
            {
                text.color = Color.green;
            }

            if(power < previousPower)
            {
                text.color = lowerColor;
            }
            
            text.text = power.ToString();
            renderer.sortingOrder = CardRenderer.sortingOrder;
            previousPower = power;
        }
            
    }
}
