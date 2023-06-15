using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//using System;

public class GameController : MonoBehaviour
{
    /*
     * The purpose of GameController script is to control the game course
     * It initialises player's cards and controls opponent's moves
     */
    public GameObject[] locationObjects;
    public GameObject hand;
    public GameObject cardChoice;
    public GameObject cardChoiceText;
    public GameObject gameOverSheet;
    public GameObject AIScript;
    public TMP_Text endSpeech;

    public GameObject cardPrefab;
    public Slider clockSlider;
    public GameObject AllCardsPrefab;
    public GameObject AllLocationsPrefab;
    public ParticleSystem afterTurnEffect;

    private int initialCardsNumber = 4;
    private int initialCardsNumberToChoose = 6;
    private int maxCardsInSlot = 4;
    private int noTurns = 2;
    private int currentTurn = 1;
    private float offset = 0.1f;
    private float cardInLocationScaler = 0.5f;
    private int cardToChooseOrder = 2;
    private int cardOrder = 0;

    /*private Dictionary<int, Location> playerLocations = new Dictionary<int, Location>();
    private Dictionary<int, Location> opponentLocations = new Dictionary<int, Location>();*/
    public Dictionary<int, Location> Locations = new Dictionary<int, Location>();
    private List<GameObject> AllCards = new List<GameObject>();

    public List<GameObject> OpponentCards = new List<GameObject>();

    public List<GameObject> AllLocations = new List<GameObject>();
    public List<GameObject> cardsInHand;
    private List<GameObject> cardsToChoose;
    private GameObject[] opponentCardDeck;
    private TMP_Text[] points;
    private TMP_Text[] opponentPoints;
    private bool secondTurn = false;

    void Start()
    {
        // Initialize locations that keep cards for storing points
        Random.InitState(System.DateTime.Now.Millisecond);

        int locationsNr = locationObjects.Length;
        opponentCardDeck = new GameObject[locationsNr];
        points = new TMP_Text[locationsNr];
        opponentPoints = new TMP_Text[locationsNr];

        //Create list of all locations
        for (int i = 0; i < AllLocationsPrefab.transform.childCount; i++)
        {
            AllLocations.Add(AllLocationsPrefab.transform.GetChild(i).gameObject);

        }

        // Creating locations structure
        for (int i=0; i<locationsNr; i++)
        {
            // UI purpose
            for (int j = 0; j < locationObjects[i].transform.childCount; j++)
            {
           
                if (locationObjects[i].transform.GetChild(j).name == "CardSlotOpponent")
                {
                    opponentCardDeck[i] = locationObjects[i].transform.GetChild(j).transform.gameObject;
                }
                else if(locationObjects[i].transform.GetChild(j).name == "Canvas")
                {
                    Debug.Log(locationObjects[i].transform.GetChild(j).GetChild(0).name);
                    points[i] = locationObjects[i].transform.GetChild(j).GetChild(0).transform.gameObject.GetComponent<TMP_Text>();
                    opponentPoints[i] = locationObjects[i].transform.GetChild(j).GetChild(1).transform.gameObject.GetComponent<TMP_Text>();
                }
                else if(locationObjects[i].transform.GetChild(j).name == "LocationInfo")
                {
                    int whichLocation = Random.Range(1, 4);
                    int whichYear = i + 1;
                    string locationName = "rok" + whichYear.ToString() + "_" + whichLocation.ToString();
                    GameObject choosenLocation = AllLocations[0];
                    
                    foreach (var someLocation in AllLocations)
                    {
                        if (someLocation.name.Equals(locationName))
                        {
                            choosenLocation = someLocation;
                            break;
                        }
                    }

                    var locationCard = Instantiate(choosenLocation, new Vector3(0, 0), Quaternion.identity);
                    Locations[i] = locationCard.GetComponent<Location>();
                    locationCard.transform.position = locationObjects[i].transform.GetChild(j).transform.position;
                }
            }
        }
        
        
        cardsInHand = new List<GameObject>();
        cardsToChoose = new List<GameObject>();

        //Create list of all cards
        cardPrefab = AllCardsPrefab.transform.GetChild(0).gameObject;
        
        for (int i = 0; i < AllCardsPrefab.transform.childCount; i++)
        {
            AllCards.Add(AllCardsPrefab.transform.GetChild(i).gameObject);
        }


        //startTurn();
        startGame();
    }
    void startGame()
    {
        cardChoice.SetActive(true);
        cardChoiceText.GetComponent<TextMeshProUGUI>().text = "Wybierz stronê konfliktu";
        Vector2 initialPosition = cardChoice.transform.position;
        var cardWidth = cardPrefab.GetComponent<Renderer>().bounds.size.x;

        initialPosition.x -= 0.5f *(2*cardWidth + offset);
        for (int i = 0; i < 2; i++)
        {
            var x_position = initialPosition.x + i * (2*cardWidth + offset);
            var y_position = initialPosition.y - 2*offset;

            var cardInstance = Instantiate(AllCards[4+i*17], new Vector3(x_position, y_position), Quaternion.identity);
            cardInstance.AddComponent<BoxCollider2D>();
            cardInstance.transform.position = new Vector3(cardInstance.transform.position.x, cardInstance.transform.position.y, -3f);
            AllCards.RemoveAt(4 + i * 17);
            cardInstance.GetComponent<SpriteRenderer>().sortingOrder = cardToChooseOrder;
            cardInstance.tag = "cardToChoose";
            cardsToChoose.Add(cardInstance);
            //cardInstance.transform.localScale += cardToChooseScaller;    
        }

        //startTurn();
    }

    void startTurn()
    {
        /*
         * Start turn by creating card choice board, instantiate cards to choose and tag them properly
         */

        cardChoice.SetActive(true);
        cardChoiceText.GetComponent<TextMeshProUGUI>().text = "Wybierz 4 karty";
        Vector2 initialPosition = cardChoice.transform.position;
        var cardWidth = cardPrefab.GetComponent<Renderer>().bounds.size.x;

        initialPosition.x -= 0.5f * ((initialCardsNumberToChoose - 1) * (cardWidth + offset));
        for (int i = 0; i < initialCardsNumberToChoose; i++)
        {
            var x_position = initialPosition.x + i * (cardWidth + offset);
            var y_position = initialPosition.y - 2 * offset; ;

            int cardIndex = Random.Range(0, AllCards.Count);
            var cardInstance = Instantiate(AllCards[cardIndex], new Vector3(x_position, y_position), Quaternion.identity);
            cardInstance.AddComponent<BoxCollider2D>();
            cardInstance.transform.position = new Vector3(cardInstance.transform.position.x, cardInstance.transform.position.y, -3f);
            AllCards.RemoveAt(cardIndex);
            cardInstance.GetComponent<SpriteRenderer>().sortingOrder = cardToChooseOrder;
            cardInstance.tag = "cardToChoose";
            cardsToChoose.Add(cardInstance);
            //cardInstance.transform.localScale += cardToChooseScaller;    
        }
    }

    void Update()
    {
        
    }
    public void addSpecialCardToHand(GameObject cardInstance)
    {
        /*
         * Add a special card from card choice board to hand
         */
        cardsToChoose.Remove(cardInstance);

        //cardInstance.transform.localScale -= cardToChooseScaller;
        cardInstance.GetComponent<SpriteRenderer>().sortingOrder = cardOrder;
        cardInstance.tag = "Untagged";

        cardsInHand.Add(cardInstance);

        reorderCardsInHand();

        if (cardsInHand.Count == 1)
        {
            cardChoice.SetActive(false);
            foreach (var card in cardsInHand) card.tag = "card";

            //dodanie karty specjalnej oponentowi
            for(int i = 0; i < cardsToChoose.Count; i++) 
            {
                
                Destroy(cardsToChoose[i]);
                //cardsToChoose.Remove(cardsToChoose[i]);
            }
            if(cardInstance.GetComponent<Card>().name == "Bazdanna")
            {
                OpponentCards.Add(AllCardsPrefab.transform.GetChild(22).gameObject);//dodanie Hurbezy do rêki oponenta
            }
            else
            {
                OpponentCards.Add(AllCardsPrefab.transform.GetChild(4).gameObject);//dodanie Bazdanny do rêki oponenta
            }
            cardsToChoose.Clear();
            startTurn();
        }
    }

    public void addCardToHand(GameObject cardInstance)
    {
        /*
         * Add a card from card choice board to hand
         */
        cardsToChoose.Remove(cardInstance);

        //cardInstance.transform.localScale -= cardToChooseScaller;
        cardInstance.GetComponent<SpriteRenderer>().sortingOrder = cardOrder;
        cardInstance.tag = "Untagged";
        
        cardsInHand.Add(cardInstance);
        
        reorderCardsInHand();

        if (cardsInHand.Count == 5 || (cardsInHand.Count == 4 && secondTurn))
        {
            cardChoice.SetActive(false);
            foreach (var card in cardsInHand) card.tag = "card";
            foreach (var card in cardsToChoose) Destroy(card);
            cardsToChoose.Clear();
            //dodane dobieranie kart dla oponenta
            AddCardsToOpponent(4);
        }
    }

    void AddCardsToOpponent(int count)
    {
        for (int i = 0; i < count; i++)
        {
            int cardIndex = Random.Range(0, AllCards.Count);
            OpponentCards.Add(AllCards[cardIndex]);
            AllCards.RemoveAt(cardIndex);
        }
    }

    void removeCardFromHand(GameObject card)
    {
        bool removed = cardsInHand.Remove(card);
        reorderCardsInHand();
    }


    void reorderCardsInHand()
    {
        int numberOfCards = cardsInHand.Count;

        Vector2 initialPositionInHand = hand.transform.position;
        var cardWidth = cardPrefab.GetComponent<Renderer>().bounds.size.x;

        initialPositionInHand.x -= 0.5f * ((numberOfCards - 1) * (cardWidth + offset));

        for (int i = 0; i < numberOfCards; i++)
        {
            var x_position = initialPositionInHand.x + i * (cardWidth + offset);
            var y_position = initialPositionInHand.y;


            Vector3 handCardPosition = new Vector3(x_position, y_position, -3f);
            cardsInHand[i].transform.position = handCardPosition;
        }
    }

    IEnumerator opponentMove()
    {
        yield return StartCoroutine(moveClockSlider());

        /////////////////////////////////////////////////////////////////////////
        var move = AIScript.GetComponent<AIScript>().GetBestMove();

        int location = move.Item2;
        //while(!(Locations[location].getOpponentSize() < maxCardsInSlot)) location = Random.Range(0, opponentCardDeck.Length);

        Debug.Log(move.Item1);
        int cardIndex = move.Item1;
        var card = Instantiate(OpponentCards[cardIndex], new Vector3(0, 0), Quaternion.identity);
        OpponentCards.RemoveAt(cardIndex);
        //AllCards.RemoveAt(cardIndex);
        ///////////////////////////////////////////////////////////////////////

        card.transform.localScale = card.transform.localScale * cardInLocationScaler;

        card.GetComponent<SpriteRenderer>().sortingOrder = cardOrder;


        opponentCardDeck[location].GetComponent<OpponentCardController>().Bind(card);
        
        Locations[location].addCard(card, true);

        int score = Locations[location].getOpponentPoints();
        int playerCurrentScore = Locations[location].getPlayerPoints();

        opponentPoints[location].text = score.ToString();
        points[location].text = playerCurrentScore.ToString();

        if (cardsInHand.Count == 0)
        {
            currentTurn++;

            yield return StartCoroutine(moveClockSlider(0.005f));

            // End turn, count points
            for (int i = 0; i < locationObjects.Length; i++)
            {
                Locations[i].fireEndTurn();

                int opponentScore = Locations[i].getOpponentPoints();
                int playerScore = Locations[i].getPlayerPoints();

                opponentPoints[i].text = opponentScore.ToString();
                points[i].text = playerScore.ToString();

                //sound
                var sound = Locations[i].GetComponent<AudioSource>();
                var clip = sound.clip;
                sound.PlayOneShot(clip);

                Instantiate(afterTurnEffect, Locations[i].transform.position, Quaternion.identity);
                yield return new WaitForSeconds(1);
            }

            yield return new WaitForSeconds(1);

            if (currentTurn > noTurns) EndGame();
            else
            {
                secondTurn = true;
                startTurn();
            }
        }

        
    }


    public bool playerMove(GameObject card, int location)
    {
        if (Locations[location].getPlayerSize() == maxCardsInSlot) return false;

        card.transform.localScale = card.transform.localScale*cardInLocationScaler;
        Locations[location].addCard(card, false);

        int score = Locations[location].getPlayerPoints();
        int opponentScore = Locations[location].getOpponentPoints();

        points[location].text = score.ToString();
        opponentPoints[location].text = opponentScore.ToString();

        return true;
    }


    IEnumerator moveClockSlider(float interval = 0.05f)
    {
        for (float i = 0f; i <= 1.0f; i+=interval)
        {
            clockSlider.value = i;
            yield return new WaitForSeconds(0.01f);
        }
        clockSlider.value = 0f;
    }

    void EndGame()
    {
        int endScore = 0;
        string endSummary = "Remis";

        for (int i = 0; i < locationObjects.Length; i++)
        {
            if (Locations[i].getPlayerPoints() > Locations[i].getOpponentPoints()) endScore++;
            else if (Locations[i].getPlayerPoints() < Locations[i].getOpponentPoints()) endScore--;
            Debug.Log(endScore);
        }
        if (endScore > 0) endSummary = "Wygra³eœ!";
        else if (endScore < 0) endSummary = "Przegra³eœ!";

        endSpeech.text = ("Koniec gry\n" + endSummary);
        gameOverSheet.SetActive(true);
        Debug.Log("End");
    }

    public void LoadMenu()
    {
        moveClockSlider();
        SceneManager.LoadScene("StartScene");
    }
}


