using BlackJack.Models;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace BlackJack.Components
{
    public class Card: MonoBehaviour
    {
        private Models.Card _card = new(Suite.Clubs, Rank.Five);
        private bool _isTurnedOver = true;

        public int GlobalIndex = 0;
        public Models.Card Model
        {
            set
            {
                _card = value;
                UpdateSprite();
            }

            get => _card;
        }

        private Image _image;
        public SpriteAtlas spriteAtlas;

        private void Awake()
        {
            _image = GetComponent<Image>();
            _card = new Models.Card((byte)GlobalIndex);
            UpdateSprite();
        }

        void Start()
        {
            UpdateSprite();
        }

        private void UpdateSprite()
        {
            _image.sprite = spriteAtlas.GetSprite(_isTurnedOver ? Model.GetSpriteName() : "card_back");
        }

        public void TurnOver()
        {
            _isTurnedOver = true;
            UpdateSprite();
        }
    }
}