using UnityEngine;
using UnityEngine.SceneManagement;

public class UImanager : MonoBehaviour
{
    public void GameStart()
    {
        SceneManager.LoadScene("Making_1");
    }
    public void Quit()
    {
        Application.Quit();
    }






}
