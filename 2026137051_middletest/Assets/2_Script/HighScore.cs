using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HighScore
{
    private const string KEY = "HighScore";
    public static int Load(int stage)
    {
        return PlayerPrefs.GetInt(KEY + "_" + stage, 0);
    }
    public static void TrySet(int stage, int newScore)
    {
        if (newScore <= Load(stage))
            return;

        PlayerPrefs.SetInt(KEY + "_" + stage, newScore);
        PlayerPrefs.Save();
    }
    // 특정 스테이지 최고기록 초기화
    public static void Reset(int stage)
    {
        PlayerPrefs.DeleteKey(KEY + "_" + stage);
        PlayerPrefs.Save();
    }
}
