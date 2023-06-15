using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationClass
{
    private List<GameObject> cards = new List<GameObject>();

    public void addCard(GameObject c)
    {
        cards.Add(c);
    }

    public int getPoints()
    {
        int strength = 0;
        
        foreach(var card in cards){
            strength++;
        }

        return strength;
    }

    public int getSize()
    {
        return cards.Count;
    }

}
