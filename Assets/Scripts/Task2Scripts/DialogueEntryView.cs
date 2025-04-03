using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Task2Scripts
{
    public class DialogueEntryView : MonoBehaviour
    {
        [SerializeField] private TMP_Text dialogueText;
        [SerializeField] private TMP_Text avatarText;
        [SerializeField] private Image avatarImage;

        public void Display(string text, Sprite avatarSprite, string avatarName)
        {
            dialogueText.text = text;
            avatarText.text = avatarName;
            avatarImage.sprite = avatarSprite;
        }
    }
}
