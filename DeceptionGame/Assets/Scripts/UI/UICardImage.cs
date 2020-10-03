using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UICardImage : MonoBehaviour
{

    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;

    public Card card{get; set;}

    public void Initialize(Card card) {
        this.titleText.SetText(card.cardName);
        this.descriptionText.SetText(card.cardDescription);
        this.card = card;
    }

    public void PlayCard() {
        this.card.PlayCard();
    }
}
