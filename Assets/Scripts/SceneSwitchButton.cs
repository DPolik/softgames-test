using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SwitchSceneButton : MonoBehaviour
{
    [SerializeField] private string sceneName;
    private void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(LoadScene);
    }

    private void LoadScene()
    {
        Debug.Log("Loading Scene: " + sceneName);
        if (sceneName == SceneManager.GetActiveScene().name)
        {
            Debug.Log("failed");
            return;
        }
        SceneManager.LoadScene(sceneName);
    }
}
