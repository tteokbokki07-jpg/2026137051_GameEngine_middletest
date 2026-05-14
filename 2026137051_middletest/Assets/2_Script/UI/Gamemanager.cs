using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class Gamemanager : MonoBehaviour
{
    public TMP_InputField inputField;
    public Button gameStartButton;
    void Start()
    {
        gameStartButton.onClick.AddListener(OnGameStartButtonClicked);
    }
    private void OnGameStartButtonClicked()
    {
        string playerName = inputField.text;
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.LogWarning("플레이어 이름을 입력해주세요.");
            return;
        }
        PlayerPrefs.SetString("PlayerName", playerName);
        SceneManager.LoadScene("Making_1");

        Debug.Log("플레이어 이름 저장: " + playerName);
        SceneManager.LoadScene("Making_1");
    }
}
