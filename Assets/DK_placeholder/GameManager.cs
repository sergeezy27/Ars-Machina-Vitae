using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private int lives, score, level;
    void Start()
    {
        // Makes sure the GameManager persists when loading different levels
        DontDestroyOnLoad(gameObject);
        NewGame();
    }
    
    private void NewGame() 
    {
        lives = 3;
        score = 0;
        level = 1;
        
        LoadLevel(level);
    }

    private void LoadLevel(int index) // Check the build profile/settings for scene index
    {
        level = index;
        // transition between levels
        Camera camera = Camera.main;
        if(camera != null) {
            camera.cullingMask = 0;
        }

        Invoke(nameof(LoadScene), 1f);
    }

    private void LoadScene()
    {
        SceneManager.LoadScene(level);
    }

    public void LevelComplete()
    {
        score += 1000;
        level++;

        if(level >= SceneManager.sceneCountInBuildSettings) {
            level = 1;
        }
        LoadLevel(level);
    }

    public void LevelFailed()
    {
        lives--;

        if (lives <= 0) {
            NewGame();
        }else {
            LoadLevel(level);
        }
    }
}
