using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic; 

public class GameOverManager : MonoBehaviour
{
    [Header("Ekrany UI")]
    public GameObject gameOverPanel;   
    public GameObject rankingPanel;    

    [Header("Teksty")]
    public TextMeshProUGUI currentScoreText; 
    public TextMeshProUGUI rankingListText;  

    private string saveKey = "TopScores";

    public void TriggerGameOver(float score)
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;

        gameOverPanel.SetActive(true);
        rankingPanel.SetActive(false); 

        int finalScore = Mathf.FloorToInt(score);
        currentScoreText.text = "TWÓJ WYNIK: " + finalScore.ToString();

        UpdateHighScores(finalScore);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowRanking()
    {
        gameOverPanel.SetActive(false);
        rankingPanel.SetActive(true);

        DisplayHighScores();
    }

    public void CloseRanking()
    {
        rankingPanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }


    void UpdateHighScores(int newScore)
    {
        List<int> scores = LoadScores();

        scores.Add(newScore);

        scores.Sort((a, b) => b.CompareTo(a));

        if (scores.Count > 10)
        {
            scores.RemoveRange(10, scores.Count - 10);
        }

        SaveScores(scores);
    }

    void DisplayHighScores()
    {
        List<int> scores = LoadScores();
        rankingListText.text = ""; 

        for (int i = 0; i < scores.Count; i++)
        {
            rankingListText.text += (i + 1).ToString() + ". " + scores[i].ToString() + "\n";
        }
    }

    List<int> LoadScores()
    {
        string data = PlayerPrefs.GetString(saveKey, "");
        List<int> scores = new List<int>();

        if (!string.IsNullOrEmpty(data))
        {
            string[] parts = data.Split(',');
            foreach (string s in parts)
            {
                if (int.TryParse(s, out int result)) scores.Add(result);
            }
        }
        return scores;
    }

    void SaveScores(List<int> scores)
    {
        string data = string.Join(",", scores);
        PlayerPrefs.SetString(saveKey, data);
        PlayerPrefs.Save();
    }
}