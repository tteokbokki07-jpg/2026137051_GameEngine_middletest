using TMPro;
using UnityEngine;

public class ScoreText : MonoBehaviour
{
    public TextMeshProUGUI stage1;
    public TextMeshProUGUI stage2;
    public TextMeshProUGUI stage3;
    public TextMeshProUGUI stage4;

    void Start()
    {
        stage1.text = "Stage1  :  " + HighScore.Load(1).ToString() + "점";
        stage2.text = "Stage2  :  " + HighScore.Load(2).ToString() + "점";
        stage3.text = "Stage3  :  " + HighScore.Load(3).ToString() + "점";
        stage4.text = "Stage4  :  " + HighScore.Load(4).ToString() + "점";
    }
}
