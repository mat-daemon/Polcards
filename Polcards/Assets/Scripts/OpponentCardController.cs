using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentCardController : MonoBehaviour
{
    private int currentSlot = 0;

    public List<Card> carsInLocation = new List<Card>();

    private int no_tiles = 4;
    private float scaler = 0.5f;

    [SerializeField] private GameObject _tilePrefab;

    public GameObject cardSlot;
    private GameObject[] _tiles;

    public ParticleSystem bindEffect;

    void Start()
    {
        GenerateGrid();
    }

    void Update()
    {

    }


    void GenerateGrid()
    {
        // Initialize tiles for cards in a location

        Vector2 cardSlotPosition = cardSlot.transform.position;
        cardSlotPosition.x -= (1.5f * _tilePrefab.GetComponent<Renderer>().bounds.size.x * scaler);
        cardSlotPosition.y += (0.5f * _tilePrefab.GetComponent<Renderer>().bounds.size.y * scaler);
        var y_position = cardSlotPosition.y;

        _tiles = new GameObject[no_tiles];


        for (int tile_nr = 0; tile_nr < no_tiles; tile_nr++)
        {
            float x_position = cardSlotPosition.x + ((tile_nr) * _tilePrefab.GetComponent<Renderer>().bounds.size.x * scaler);

            var spawnedTile = Instantiate(_tilePrefab, new Vector3(x_position, y_position), Quaternion.identity);
            spawnedTile.transform.localScale = spawnedTile.transform.localScale * scaler;
            //spawnedTile.tag = "cardSlot";

            spawnedTile.transform.SetParent(cardSlot.transform);
            _tiles[tile_nr] = spawnedTile;

        }


    }

    public void Bind(GameObject card)
    {
        card.transform.position = _tiles[currentSlot].transform.position;
        card.tag = "Untagged";
        currentSlot++;

        //dodanie karty do pamiêci lokacji
        carsInLocation.Add(card.GetComponent<Card>());

        Instantiate(bindEffect, card.transform.position, Quaternion.identity);
    }
}
