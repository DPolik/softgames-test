using System.Collections;
using UnityEngine;

namespace Task1Scripts
{
    public class Main : MonoBehaviour
    {
        [SerializeField] private CardStack[] cardStacks;
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private int numberOfCards = 144;
        [SerializeField] private float moveTime = .5f; // Animation duration
        [SerializeField] private float waitTime = 1f; // Time for next card to pop
        private int _currentStackIndex;

        private void Start()
        {
            _currentStackIndex = Random.Range(0, cardStacks.Length);
            cardStacks[_currentStackIndex].InitializeStack(numberOfCards, cardPrefab);
            StartCoroutine(MoveCards());
        }

        private IEnumerator MoveCards()
        {
            var nextStackIndex = _currentStackIndex + 1;
            if (nextStackIndex >= cardStacks.Length)
            {
                nextStackIndex = 0;
            }
        
            while (true)
            {
                yield return new WaitForSeconds(waitTime);
            
                if (cardStacks[_currentStackIndex].IsEmpty)
                {
                    cardStacks[nextStackIndex].SlideTopCard();
                    _currentStackIndex = nextStackIndex;
                    nextStackIndex = _currentStackIndex + 1;
                    if (nextStackIndex >= cardStacks.Length)
                    {
                        nextStackIndex = 0;
                    }

                    if (moveTime > waitTime) // wait until the animation is done
                    {
                        yield return new WaitForSeconds(moveTime - waitTime);
                    }
                }
            
                var topCard = cardStacks[_currentStackIndex].PopCard();
                StartCoroutine(AnimateMove(topCard, cardStacks[nextStackIndex])); 
            }
        }

        private IEnumerator AnimateMove(GameObject card, CardStack targetStack)
        {
            card.transform.parent = targetStack.transform;
            var startPos = card.transform.position;
            // end position will be the stack position (with z adjusted) plus the offset if there are cards already
            var endPos = targetStack.transform.position - Vector3.forward * ((targetStack.transform.childCount-1) * 0.1f);
            if (!targetStack.IsEmpty)
            {
                endPos += targetStack.TopCardDisplacement;
            }
            
            targetStack.PushCard(card);

            var elapsedTime = 0f;
            while (elapsedTime < moveTime)
            {
                elapsedTime += Time.deltaTime;
                card.transform.position = Vector3.Lerp(startPos, endPos, elapsedTime / moveTime);
                yield return null;
            }

            card.transform.position = endPos;
            targetStack.CheckVisibility(card);
        }
    }
}
