﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardUIView : MonoBehaviour
{
    public Transform cardUIParent;
    [SerializeField] private UICardImage cardPrefab;

    private List<UICardImage> uiCardImages = new List<UICardImage>(); 

    private bool isShowing = false;
    public void PressPlayCardsButton() {
        isShowing = !isShowing;
        // TODO:
        this.cardUIParent.gameObject.SetActive(isShowing);

        if (isShowing) {
            PlayerController mainPlayer = GameManager.Instance.controller.mainPlayer;
            List<Card> cards = mainPlayer.cardController.GetCardsInHand();
            foreach (Card card in cards) {
                UICardImage newCard = Instantiate<UICardImage>(this.cardPrefab, cardUIParent);
                newCard.Initialize(card);
                this.uiCardImages.Add(newCard);
            }
        } else {
            this.DestroyCardList();
        }
    }

    private void DestroyCardList() {
        foreach (UICardImage image in this.uiCardImages) {
            Destroy(image.gameObject);
        }

        uiCardImages.Clear();

    }

}
