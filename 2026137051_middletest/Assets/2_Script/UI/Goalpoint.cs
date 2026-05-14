using UnityEngine;
using UnityEngine.SceneManagement;

public class Goalpoint : MonoBehaviour
{
    public string nextLevel;
    public void MoveToNextLevel()
    {
        SceneManager.LoadScene(nextLevel);
    }


}
