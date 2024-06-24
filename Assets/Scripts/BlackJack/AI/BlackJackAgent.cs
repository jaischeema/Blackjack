using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace BlackJack.AI
{
    public class BlackJackAgent : Agent
    {
        private SceneManager _sceneManager;

        public override void Initialize()
        {
            _sceneManager = GetComponent<SceneManager>();
        }

        public override void OnEpisodeBegin()
        {
            Debug.Log("Episode started");
            base.OnEpisodeBegin();
            _sceneManager.Reset();
        }

        public override void CollectObservations(VectorSensor sensor)
        {
            base.CollectObservations(sensor);

            sensor.AddObservation(_sceneManager.Game.CurrentPlayerIndex);

            _sceneManager.Game.PlayerHands.ForEach(a => { a.ForEach(card => sensor.AddObservation(card.globalIndex)); });

            _sceneManager.Game.VisibleDealerCards.ForEach(a => { sensor.AddObservation(a.globalIndex); });
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
            base.OnActionReceived(actions);
            var discreteActions = actions.DiscreteActions;
            Debug.Log("Received action: " + discreteActions[0]);
            var action = _sceneManager.Game.GetActionFromIndex(discreteActions[0]);
            _sceneManager.Game.Update(action);

            if (!_sceneManager.Game.IsFinished) return;

            var increment = _sceneManager.Game.Reward(0);
            AddReward(increment);
            Debug.Log("Episode finished with reward: " + increment);

            EndEpisode();
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            base.Heuristic(new ActionBuffers(ActionSpec.MakeDiscrete(_sceneManager.Game.GetPossibleActions().Count)));
        }
    }
}