using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;

namespace Task2Scripts
{
    public class DialogueSystem : MonoBehaviour
    {
        [SerializeField] private string apiUrl = "https://private-624120-softgamesassignment.apiary-mock.com/v2/magicwords";
        [SerializeField] private Transform dialogueContainer;
        [SerializeField] private GameObject dialoguePrefabLeft;
        [SerializeField] private GameObject dialoguePrefabRight;
        private List<string> _emojiList = new List<string>();

        void Start()
        {
            StartCoroutine(FetchDialogue());
        }

        IEnumerator FetchDialogue()
        {
            UnityWebRequest request = UnityWebRequest.Get(apiUrl);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Deserialize JSON directly into the DialogueData class
                DialogueData data = JsonUtility.FromJson<DialogueData>(request.downloadHandler.text);
                StoreEmojiData(data.emojies);
                StartCoroutine(DisplayDialogue(data.dialogue, data.avatars));
            }
            else
            {
                Debug.LogError("Failed to fetch dialogue: " + request.error);
            }
        }

        void StoreEmojiData(List<EmojiData> emojies)
        {
            foreach (var emoji in emojies)
            {
                _emojiList.Add(emoji.name);
            }
        }

        IEnumerator DisplayDialogue(List<DialogueEntry> dialogue, List<AvatarData> avatars)
        {
            foreach (var entry in dialogue)
            {
                var avatar = avatars.Find(a => a.name == entry.name);
                var prefab = dialoguePrefabLeft;
                if (avatar is { position: "right" })
                {
                    prefab = dialoguePrefabRight;
                }
                
                var dialogueEntry = Instantiate(prefab, dialogueContainer).GetComponent<DialogueEntryView>();

                // Process text with emojis
                var processedText = ReplaceEmojis(entry.text);
            
                yield return StartCoroutine(LoadImage(avatar?.url, (avatarSprite) => dialogueEntry.Display(processedText, avatarSprite, entry.name)));
            }
        }

        string ReplaceEmojis(string text)
        {
            var matches = Regex.Matches(text, @"{([^}]+)}");

            foreach (Match match in matches)
            {
                var emojiName = match.Groups[1].Value;
            
                if (_emojiList.Contains(emojiName))
                {
                    text = text.Replace(match.Value, $"<sprite name=\"{emojiName}\" >");
                }
                else
                {
                    text = text.Replace(match.Value, "");
                }
            }
            return text;
        }

        IEnumerator LoadImage(string url, Action<Sprite> callback)
        {
            var request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
                callback?.Invoke(sprite);
            }
            else
            {
                Debug.LogError("Failed to load image: " + request.error + " url: " + url);
                callback?.Invoke(null);
            }
        }
    }

    [System.Serializable]
    public class DialogueData
    {
        public List<DialogueEntry> dialogue;
        public List<EmojiData> emojies;
        public List<AvatarData> avatars;
    }

    [System.Serializable]
    public class DialogueEntry
    {
        public string name;
        public string text;
    }

    [System.Serializable]
    public class EmojiData
    {
        public string name;
        public string url;
    }

    [System.Serializable]
    public class AvatarData
    {
        public string name;
        public string url;
        public string position;
    }
}