using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UImanager : MonoBehaviour
{
    public GameObject SettingPanal;
    public GameObject StagePanal;
    public bool isPaused = false;
    public TextMeshProUGUI ClearItemCount;

    public PlayerController player;

    [System.Obsolete]
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isPaused == false)
        {
            Time.timeScale = 0;
            SettingPanal.SetActive(true);
            Debug.Log("ESC");
            isPaused = true;
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isPaused == true)
        {
            Time.timeScale = 1;
            SettingPanal.SetActive(false);
            StagePanal.SetActive(false);
            Debug.Log("ESC_down");
            isPaused = false;
        }

        // ClearItemCount 업데이트 (널 체크)
        if (ClearItemCount != null && player != null)
        {
            ClearItemCount.text = $"클리어 아이템 수집 : {player.itemMission}/{player.missionCount}";
        }
    }
    public void GamePause()
    {
        Time.timeScale = 1;
        SettingPanal.SetActive(false);
        isPaused = false;
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void Title()
    {
        SceneManager.LoadScene("Title");
        Time.timeScale = 1;
        isPaused = false;
    }
    public void GameStart()
    {
        SceneManager.LoadScene("Making_1");
        Time.timeScale = 1;
        isPaused = false;
    }

    public void Stage1()
    {
        SceneManager.LoadScene("Making_1");
        Time.timeScale = 1;
        isPaused = false;
    }
    public void Stage2()
    {
        SceneManager.LoadScene("Making_2");
        Time.timeScale = 1;
        isPaused = false;
    }
    public void Stage3()
    {
        SceneManager.LoadScene("Making_3");
        Time.timeScale = 1;
        isPaused = false;
    }
    public void Stage4()
    {
        SceneManager.LoadScene("Making_4");
        Time.timeScale = 1;
        isPaused = false;
    }
    public void Stage5()
    {
        SceneManager.LoadScene("Making_5");
        Time.timeScale = 1;
        isPaused = false;
    }
}
