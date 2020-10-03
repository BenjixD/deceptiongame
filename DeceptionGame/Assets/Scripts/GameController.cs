using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public MapController mapController;
    public FogOfWarGenerator fogOfWarGenerator;
    public PlayerController mainPlayer{get; set;}
    public GameUIController uiController;

    public int tmp_NumStartingCards = 5; // TODO: Put somewhere else

    // Start is called before the first frame update
    void Awake()
    {
        GameManager.Instance.controller = this;
        mapController.Initialize();
        this.uiController.Initialize();
    }

    void Start() {
        // tmp - TODO: Put somewhere else
        foreach (PlayerController player in this.mapController.players) {
            List<Card> cards = GameManager.Instance.models.cardPool.GetCardsFromPool(tmp_NumStartingCards);
            player.cardController.AddCardsToHand(cards);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
