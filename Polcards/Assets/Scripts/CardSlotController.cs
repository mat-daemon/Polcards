using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSlotController : MonoBehaviour
{
    public int locationNr;
    public GameObject cardSlot;
    public GameObject CardsFrame;
    public GameObject GameController;
    public ParticleSystem bindEffect;


    public List<Card> carsInLocation = new List<Card>();

    private int currentSlot = 0; // slots 0,1,2,3
    private int no_tiles = 4;
    private float scaler = 0.5f;

    [SerializeField] private GameObject _tilePrefab;
    private GameObject[] _tiles;

    void Start()
    {
        GameController = GameObject.Find("GameController");
        GenerateDeck();
        GenerateGrid();
    }

    void Update()
    {
        
    }

    void GenerateDeck()
    {
        Vector2 positionForCardFrame = cardSlot.transform.position;

        positionForCardFrame.y -= (_tilePrefab.GetComponent<Renderer>().bounds.size.y * scaler/ 2.0f);
        
        var cardsFrame = Instantiate(CardsFrame, new Vector3(0, 0), Quaternion.identity);
        cardsFrame.transform.localScale = cardsFrame.transform.localScale*scaler;
        cardsFrame.transform.position = positionForCardFrame;
        cardsFrame.transform.SetParent(cardSlot.transform);
    }

    void GenerateGrid()
    {
        // Initialize tiles for cards in a location

        Vector2 cardSlotPosition = cardSlot.transform.position;
        cardSlotPosition.x -= (1.5f * _tilePrefab.GetComponent<Renderer>().bounds.size.x * scaler);
        cardSlotPosition.y -= (0.5f * _tilePrefab.GetComponent<Renderer>().bounds.size.y * scaler);
        var y_position = cardSlotPosition.y;

        _tiles = new GameObject[no_tiles];


        for (int tile_nr = 0; tile_nr < no_tiles; tile_nr++)
        {
            float x_position = cardSlotPosition.x + ( (tile_nr) * _tilePrefab.GetComponent<Renderer>().bounds.size.x * scaler);

            var spawnedTile = Instantiate(_tilePrefab, new Vector3(x_position, y_position), Quaternion.identity);
            spawnedTile.transform.localScale = spawnedTile.transform.localScale*scaler;
            //spawnedTile.tag = "cardSlot";

            spawnedTile.transform.SetParent(cardSlot.transform);
            _tiles[tile_nr] = spawnedTile;

        }
        
    }

    public bool Bind(GameObject card)
    {
        bool movePossible = GameController.GetComponent<GameController>().playerMove(card, locationNr);

        if (movePossible)
        {
            card.transform.position = _tiles[currentSlot].transform.position;
            card.tag = "Untagged";
            currentSlot++;

            //dodanie karty do pamiêci lokacji
            carsInLocation.Add(card.GetComponent<Card>());

            GameController.SendMessage("removeCardFromHand", card);

            Instantiate(bindEffect, card.transform.position, Quaternion.identity);
            GameController.SendMessage("opponentMove");
        }

        return movePossible;
    }

}
