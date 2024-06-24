using BlackJack.Models;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace BlackJack.AI
{
    public class BlackJackAgent : Agent
    {
        private Game _game;

        public override void OnEpisodeBegin()
        {
            Debug.Log("Episode started");
            base.OnEpisodeBegin();
            _game = new Game(1);
            _game.Start();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            base.CollectObservations(sensor);

            sensor.AddObservation(_game.CurrentPlayerIndex);

            _game.PlayerHands.ForEach(a => { a.ForEach(card => sensor.AddObservation(card.globalIndex)); });

            _game.VisibleDealerCards.ForEach(a => { sensor.AddObservation(a.globalIndex); });
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            base.OnActionReceived(actions);
            var discreteActions = actions.DiscreteActions;
            Debug.Log("Received action: " + discreteActions[0]);
            var action = _game.GetActionFromIndex(discreteActions[0]);
            _game.Update(action);

            if (!_game.IsFinished) return;

            var increment = _game.Reward(0);
            AddReward(increment);
            Debug.Log("Episode finished with reward: " + increment);

            EndEpisode();
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            Debug.Log("Calling Heuristic");
            base.Heuristic(new ActionBuffers(ActionSpec.MakeDiscrete(_game.GetPossibleActions().Count)));
        }
    }
}