using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private string gameSceneName = "Main Scene";
    
    
    public void OnNewGameButtonClicked() => SaveManager.Instance.TransitionScene(gameSceneName, true);
    
    public void OnLoadButtonClicked() => SaveManager.Instance.TransitionScene(gameSceneName, false);
    
    public void OnExitButtonClicked()
    {
        // This part only runs inside the Unity Editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif

        // This part runs in the actual built game
        Application.Quit();
    }
}