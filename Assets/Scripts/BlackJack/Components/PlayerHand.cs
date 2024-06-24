using System;
using System.Collections.Generic;
using BlackJack.Models;
using TMPro;
using UnityEngine;

public class PlayerHand : MonoBehaviour
{
    public GameObject CardPrefab;
    public TextMeshProUGUI PlayerLabel;
    public GameObject CardsPanel;
    public int PlayerIndex;
    private bool _isActive;

    private readonly List<Card> _currentCards = new();
    
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerIndex == -1)
        {
            PlayerLabel.text = "Dealer";
        }
        else
        {
            PlayerLabel.text = "Player " + PlayerIndex;
        }
    }

    private void Update()
    {
        var currentLeastTotal = Game.SumOfValues(_currentCards, true);
        var currentHighestTotal = Game.SumOfValues(_currentCards, false);
        var result = currentHighestTotal.ToString();
        if (currentLeastTotal != currentHighestTotal)
        {
            result = currentHighestTotal > 21 ? currentLeastTotal.ToString() : $"{currentLeastTotal} or {currentHighestTotal}";
        }
        
        PlayerLabel.text = PlayerIndex == -1 ? $"Dealer ({result})" : $"Player {PlayerIndex} ({result})";
    }

    public void ChangeActive(bool isActive)
    {
        _isActive = isActive;
        PlayerLabel.color = _isActive ? Color.green : Color.white;
    }

    public void AddCard(Card card)
    {
        if(_currentCards.Contains(card)) return;
        _currentCards.Add(card);
        
        var newObject = Instantiate(CardPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        newObject.transform.SetParent(CardsPanel.transform);
        var cardObject = newObject.GetComponent<BlackJack.Components.Card>();
        cardObject.Model = card;
    }
}
