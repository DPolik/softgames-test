using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Task1Scripts
{
    public class CardStack : MonoBehaviour
    {
        private List<GameObject> cards = new List<GameObject>();
        public bool IsEmpty => cards.Count == 0;
        public GameObject TopCard => IsEmpty ? null : cards[^1];
        public Vector3 TopCardDisplacement => Vector3.left * 0.5f;

        public void InitializeStack(int numberOfCards, GameObject cardPrefab)
        {
            for (var i = cards.Count-1; i >= 0; i--)
            {
                Destroy(cards[i]);
                cards.RemoveAt(i);
            }

            float zPos = 0;
            while (cards.Count < numberOfCards)
            {
                var card = Instantiate(cardPrefab, transform);
                card.transform.localPosition = new Vector3(0, 0, zPos);
                var cardRenderer = card.GetOrAddComponent<SpriteRenderer>();
                // Change to color of the sprite to distinguish between cards
                cardRenderer.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                // Disable the renderer to save in performance
                cardRenderer.enabled = false;
                cards.Add(card);
                zPos -= 0.5f; // avoid z fighting
            }

            // Enable renderers for the top cards that can be seen
            if (cards.Count > 0)
                cards[^1].GetComponent<SpriteRenderer>().enabled = true;
            if (cards.Count > 1)
                cards[^2].GetComponent<SpriteRenderer>().enabled = true;
        
            SlideTopCard();
        }

        public void SlideTopCard()
        {
            if (cards.Count < 2)
            {
                return;
            }
        
            cards[^1].transform.localPosition += TopCardDisplacement;
        }

        public GameObject PopCard()
        {
            var card = IsEmpty ? null : cards[^1];
            cards.RemoveAt(cards.Count - 1);
            if (cards.Count > 1) // Show the bottom card
            {
                cards[^2].GetComponent<SpriteRenderer>().enabled = true;
            }
            SlideTopCard();
            return card;
        }

        public void PushCard(GameObject card)
        {
            cards.Add(card);
        }

        public void CheckVisibility(GameObject card)
        {
            var cardIndex = cards.IndexOf(card);
            if (cardIndex < 2) // Only need to reset visibility of cards in bottom of deck
            {
                return;
            }
        
            cards[cardIndex-1].transform.localPosition -= TopCardDisplacement;
            cards[cardIndex-2].GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
