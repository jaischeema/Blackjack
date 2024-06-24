using System.Collections.Generic;
using System.Linq;
using BlackJack.Models;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    public GameObject PlayerHandPrefab;
    public GameObject HandsContainer;
    private Game _game;

    public Game Game => _game;

    private PlayerHand _dealer;
    private List<PlayerHand> _playerHands = new();

    private void Start()
    {
        Reset();
    }

    // Start is called before the first frame update
    public void Reset()
    {
        _game = new Game(3);
        
        foreach(Transform child in HandsContainer.transform)
        {
            Destroy(child.gameObject);
        }
        
        var dealerGameObject = Instantiate(PlayerHandPrefab, Vector3.zero, Quaternion.identity);
        dealerGameObject.transform.SetParent(HandsContainer.transform);
        _dealer = dealerGameObject.GetComponent<PlayerHand>();
        _dealer.PlayerIndex = -1;
        
        _playerHands = _game.PlayerHands.Select((a, index) =>
        {
            var newObj = Instantiate(PlayerHandPrefab, Vector3.zero, Quaternion.identity);
            newObj.transform.SetParent(HandsContainer.transform);
            var playerHand = newObj.GetComponent<PlayerHand>();
            playerHand.PlayerIndex = index;
            return playerHand;
        }).ToList();

        
        _game.Start();
    }

    public void OnHitButton()
    {
        _game.Update(Game.PlayerAction.Hit);
    }

    public void OnStandButton()
    {
        _game.Update(Game.PlayerAction.Stand);
    }

    private void Update()
    {
       _game.VisibleDealerCards.ForEach(a => _dealer.AddCard(a)); 
       foreach (var item in _game.PlayerHands.Select((value, i) => new { i, value }))
       {
           var cards = item.value;
           var playerIndex = item.i;
           var playerHand = _playerHands[playerIndex];
           cards.ForEach(card => playerHand.AddCard(card));
           playerHand.ChangeActive(_game.CurrentPlayerIndex == playerIndex);
       }
    }
}
