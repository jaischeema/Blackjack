using BlackJack.Models;

using UnityEngine;

public class TestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var game = new Game(2);
        print(game);
        game.Start();
        print(game);
        game.Update(Game.PlayerAction.Hit);
        game.Update(Game.PlayerAction.Stand);
        print(game);
    }

    private void print(Game game)
    {
        game.PlayerHands.ForEach(a =>
        {
            Debug.Log("Player cards ------------------------------------");
            a.ForEach(card => Debug.Log(card));
        });

        Debug.Log("Dealer cards ------------------------------------");
        game.VisibleDealerCards.ForEach(a => Debug.Log(a));
        Debug.Log("Current player=" + game.CurrentPlayerIndex + " Is Finished= " + game.IsFinished);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
