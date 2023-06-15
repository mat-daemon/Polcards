using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location : MonoBehaviour
{
    public string name;
    public int year;
    public string[] abilities;
    //NatSym_ -dodaje symbol _ w tej lokacji ka¿demu graczowi
    //NatPlay_ -dodaje wartoœæ _ do si³y ka¿dej zagrywanej karty do tej lokacji
    //FirstZero - pierwsza zagrana karta przez ka¿dego z graczy w tej lokacji ma si³ê 0
    //SemMore_,_ - na koniec semestru gracz który ma wiêcej symboli _ otrzymuje + _ si³y

    private List<GameObject> playerCards = new List<GameObject>();
    private List<GameObject> opponentCards = new List<GameObject>();
    private List<string> fireAtEndPlayer = new List<string>();
    private List<string> fireAtEndOponent = new List<string>();
    private int playerPoints = 0;
    private int opponentPoints = 0;
    
    private int locationSymbol = -1;
    private int additionalStrength = 0;
    private bool zeroOpponentCard = false;
    private bool zeroPlayerCard = false;
    private int boostSymbolEndTurn = -1;
    private int boostForSymbolEndTurn = 0;

    public void addCard(GameObject c, bool forOpponent)
    {
        //var cards = 
        var card = c.GetComponent<Card>();

        //play sound
        var sound = c.GetComponent<AudioSource>();
        var clip = sound.clip;
        sound.PlayOneShot(clip);
        //
        foreach (var ability in card.abilities)
        {
            //if (ability.StartsWith("SemOp") && !forOpponent)
            //{
            //    fireAtEndPlayer.Add(ability);
            //}
            //else if (ability.StartsWith("Sem") && !forOpponent)
            //{
            //    fireAtEndPlayer.Add(ability);
            //}
            //// added for oponent
            //else if (ability.StartsWith("SemOp") && forOpponent)
            //{
            //    fireAtEndOponent.Add(ability);
            //}
            //else if (ability.StartsWith("Sem") && forOpponent)
            //{
            //    fireAtEndOponent.Add(ability);
            //}
            ////
            if (ability.StartsWith("NatOp") && !forOpponent)
            {
                string[] details = ability.Substring(5).Split(",");
                int symbol = int.Parse(details[0]);
                int wage = int.Parse(details[1]);

                int pointsForSymbols = countPointsForSymbols(symbol, wage, opponentCards, opponentCards, details.Length == 3);

                if (details.Length == 3)
                {
                    opponentPoints += pointsForSymbols;
                }
                else 
                {
                    //playerPoints += pointsForSymbols;
                    card.power += pointsForSymbols;
                }

                
            }
            else if (ability.StartsWith("Nat") && !forOpponent)
            {
                string[] details = ability.Substring(3).Split(",");
                int symbol = int.Parse(details[0]);
                int wage = int.Parse(details[1]);
                int pointsForSymbols = countPointsForSymbols(symbol, wage, playerCards, opponentCards, details.Length == 3);

                if (details.Length == 3)
                {
                    opponentPoints += pointsForSymbols;
                }
                else
                {
                    //playerPoints += pointsForSymbols;
                    card.power += pointsForSymbols;
                }
            }
            //aded for oponent move
            else if (ability.StartsWith("NatOp") && forOpponent)
            {
                string[] details = ability.Substring(5).Split(",");
                int symbol = int.Parse(details[0]);
                int wage = int.Parse(details[1]);

                int pointsForSymbols = countPointsForSymbols(symbol, wage, playerCards, playerCards, details.Length == 3);

                if (details.Length == 3)
                {
                    playerPoints += pointsForSymbols;
                }
                else
                {
                    //opponentPoints += pointsForSymbols;
                    card.power += pointsForSymbols;
                }
               
            }
            else if (ability.StartsWith("Nat") && forOpponent)
            {
                string[] details = ability.Substring(3).Split(",");
                int symbol = int.Parse(details[0]);
                int wage = int.Parse(details[1]);
                int pointsForSymbols = countPointsForSymbols(symbol, wage, opponentCards, playerCards, details.Length == 3);

                if (details.Length == 3)
                {
                    playerPoints += pointsForSymbols;
                }
                else
                {
                    //opponentPoints += pointsForSymbols;
                    card.power += pointsForSymbols;
                }
            }
            //
        }

        if (forOpponent)
        {
            if (zeroOpponentCard)
            {
                zeroOpponentCard = false;
                c.GetComponent<Card>().power = 0;
            }
            c.GetComponent<Card>().power += additionalStrength;

            opponentPoints += c.GetComponent<Card>().power;
            opponentCards.Add(c);
        }
        else
        {
            if (zeroPlayerCard)
            {
                zeroPlayerCard = false;
                c.GetComponent<Card>().power = 0;
            }
            c.GetComponent<Card>().power += additionalStrength;

            playerPoints += c.GetComponent<Card>().power;
            playerCards.Add(c);
        }
    }

    //private int countPointsForSymbols(int symbol, int wage, List<GameObject> cards, List<GameObject> oponentCards, bool oposite=false)
    //{
    //    int points = 0;
    //    int symbolCounter = 0;
    //    foreach (var cardInLocation in cards)
    //    {
    //        foreach (int cardSymbol in cardInLocation.GetComponent<Card>().symbols)
    //        {
    //            if (cardSymbol == symbol) {
    //                symbolCounter++;
    //                //aded to change strength of card
    //                if(!oposite)
    //                {
    //                    cardInLocation.GetComponent<Card>().power += wage;
    //                }                    
    //            }
    //        }
    //    }
    //    // Additional symbol added
    //    if (symbol == locationSymbol) symbolCounter++;

    //    if (oposite)
    //    {
    //        foreach (var cardInLocation in oponentCards)
    //        {
    //            foreach (int cardSymbol in cardInLocation.GetComponent<Card>().symbols)
    //            {
    //                if (cardSymbol == symbol)
    //                {
    //                    //aded to change strength of oposite team card
    //                    cardInLocation.GetComponent<Card>().power += wage;
                        
    //                }
    //            }
    //        }
    //    }

    //    points += (symbolCounter * wage);
    //    return points;
    //}

    private int countPointsForSymbols(int symbol, int wage, List<GameObject> cards, List<GameObject> oponentCards, bool oposite = false)
    {
        int points = 0;
        int symbolCounter = 0;
        foreach (var cardInLocation in cards)
        {
            foreach (int cardSymbol in cardInLocation.GetComponent<Card>().symbols)
            {
                if (cardSymbol == symbol)
                {
                    symbolCounter++;
                }
            }
        }
        // Additional symbol added
        if (symbol == locationSymbol) symbolCounter++;

        if (oposite)
        {
            foreach (var cardInLocation in oponentCards)
            {
                foreach (int cardSymbol in cardInLocation.GetComponent<Card>().symbols)
                {
                    if (cardSymbol == symbol)
                    {
                        //aded to change strength of oposite team card
                        cardInLocation.GetComponent<Card>().power += wage;

                    }
                }
            }
        }

        points += (symbolCounter * wage);
        return points;
    }

    public void fireEndTurn()
    {
        foreach (var card in playerCards)
        {
            foreach (var ability in card.GetComponent<Card>().abilities)
            {
                if (ability.StartsWith("SemOp"))
                {
                    string[] details = ability.Substring(5).Split(",");
                    int symbol = int.Parse(details[0]);
                    int wage = int.Parse(details[1]);

                    int pointsForSymbols = countPointsForSymbols(symbol, wage, opponentCards, opponentCards, details.Length == 3);

                    if (details.Length == 3)
                    {
                        opponentPoints += pointsForSymbols;
                    }
                    else
                    {
                        playerPoints += pointsForSymbols;
                        card.GetComponent<Card>().power += pointsForSymbols;
                    }
                }
                else if (ability.StartsWith("Sem"))
                {
                    string[] details = ability.Substring(3).Split(",");
                    int symbol = int.Parse(details[0]);
                    int wage = int.Parse(details[1]);
                    int pointsForSymbols = countPointsForSymbols(symbol, wage, playerCards, opponentCards, details.Length == 3);

                    if (details.Length == 3)
                    {
                        opponentPoints += pointsForSymbols;
                    }
                    else
                    {
                        playerPoints += pointsForSymbols;
                        card.GetComponent<Card>().power += pointsForSymbols;
                    }
                }
            }
        }
        foreach (var card in opponentCards)
        {
            foreach (var ability in card.GetComponent<Card>().abilities)
            {
                if (ability.StartsWith("SemOp"))
                {
                    string[] details = ability.Substring(5).Split(",");
                    int symbol = int.Parse(details[0]);
                    int wage = int.Parse(details[1]);

                    int pointsForSymbols = countPointsForSymbols(symbol, wage, playerCards, playerCards, details.Length == 3);

                    if (details.Length == 3)
                    {
                        playerPoints += pointsForSymbols;
                    }
                    else
                    {
                        opponentPoints += pointsForSymbols;
                        card.GetComponent<Card>().power += pointsForSymbols;
                    }
                }
                else if (ability.StartsWith("Sem"))
                {
                    string[] details = ability.Substring(3).Split(",");
                    int symbol = int.Parse(details[0]);
                    int wage = int.Parse(details[1]);
                    int pointsForSymbols = countPointsForSymbols(symbol, wage, opponentCards, playerCards, details.Length == 3);

                    if (details.Length == 3)
                    {
                        playerPoints += pointsForSymbols;
                    }
                    else
                    {
                        opponentPoints += pointsForSymbols;
                        card.GetComponent<Card>().power += pointsForSymbols;
                    }
                }
            }
        }

        if (boostSymbolEndTurn != -1)
        {
            int countOpponentSymbols = countPointsForSymbols(boostSymbolEndTurn, 1, opponentCards, playerCards);
            int countPlayerSymbols = countPointsForSymbols(boostSymbolEndTurn, 1, playerCards, opponentCards);

            if (countOpponentSymbols > countPlayerSymbols) opponentPoints += boostForSymbolEndTurn;
            else if (countOpponentSymbols < countPlayerSymbols) playerPoints += boostForSymbolEndTurn;
        }

        //fireAtEndPlayer.Clear();
    }

    public int getPlayerPoints()
    {
        return playerPoints;
    }

    public int getOpponentPoints()
    {
        return opponentPoints;
    }

    public int getPlayerSize()
    {
        return playerCards.Count;
    }

    public int getOpponentSize()
    {
        return opponentCards.Count;
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (var ability in abilities)
        {
            if (ability.StartsWith("NatSym"))
            {
                int symbolAdded = int.Parse(ability.Substring(6));
                locationSymbol = symbolAdded;

            }
            else if (ability.StartsWith("NatPlay"))
            {
                int strength = int.Parse(ability.Substring(7));
                additionalStrength = strength;
            }
            else if (ability.StartsWith("FirstZero"))
            {
                zeroOpponentCard = true;
                zeroPlayerCard = true;
            }
            else if (ability.StartsWith("SemMore"))
            {
                string[] strength = ability.Substring(7).Split(",");
                boostSymbolEndTurn = int.Parse(strength[0]);
                boostForSymbolEndTurn = 2;
                //boostForSymbolEndTurn = int.Parse(strength[1]);
            }

        }

    }
}
