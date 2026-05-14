using UnityEngine;

public class TitleManager : MonoBehaviour
{
    void Start()
    {
        SoundManager.instance.PlayBGM(SoundManager.instance.titleBGM);
    }
}
