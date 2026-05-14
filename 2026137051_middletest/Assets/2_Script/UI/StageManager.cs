using UnityEngine;

public class StageManager : MonoBehaviour
{
    void Start()
    {
        SoundManager.instance.PlayBGM(SoundManager.instance.stageBGM);
    }

}
