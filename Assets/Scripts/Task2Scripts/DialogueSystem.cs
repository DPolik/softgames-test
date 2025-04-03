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
        private readonly List<string> _emojiList = new List<string>();
        private readonly Dictionary<string, Sprite> _imageCache = new Dictionary<string, Sprite>();
        
        
        private void Start()
        {
            StartCoroutine(FetchDialogue());
        }

        private IEnumerator FetchDialogue()
        {
            var request = UnityWebRequest.Get(apiUrl);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                // Deserialize JSON directly into the DialogueData class
                var data = JsonUtility.FromJson<DialogueData>(request.downloadHandler.text);
                StoreEmojiData(data.emojies);
                StartCoroutine(DisplayDialogue(data.dialogue, data.avatars));
            }
            else
            {
                Debug.LogError("Failed to fetch dialogue: " + request.error);
            }
        }

        private void StoreEmojiData(List<EmojiData> emojies)
        {
            foreach (var emoji in emojies)
            {
                _emojiList.Add(emoji.name);
            }
        }

        private IEnumerator DisplayDialogue(List<DialogueEntry> dialogue, List<AvatarData> avatars)
        {
            foreach (var entry in dialogue)
            {
                var avatar = avatars.Find(a => a.name == entry.name);
                var prefab = dialoguePrefabLeft;
                if (avatar is { position: "right" })
                {
                    prefab = dialoguePrefabRight;
                }
                
                // Process text with emojis
                var processedText = ReplaceEmojis(entry.text);
            
                yield return StartCoroutine(LoadImage(avatar?.url, (avatarSprite) =>
                {   
                    var dialogueEntry = Instantiate(prefab, dialogueContainer).GetComponent<DialogueEntryView>();
                    dialogueEntry.Display(processedText, avatarSprite, entry.name);
                }));
                yield return new WaitForSeconds(2f); // wait between each entry to make it prettier
            }
        }

        private string ReplaceEmojis(string text)
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

        private IEnumerator LoadImage(string url, Action<Sprite> callback)
        {
            if (_imageCache.ContainsKey(url))
            {
                callback?.Invoke(_imageCache[url]);
                yield break;
            }
            
            var request = UnityWebRequestTexture.GetTexture(url);
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
                var sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
                _imageCache[url] = sprite;
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