using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class StageResult
{
    public string playerName;
    public int stage;
    public int score;
}
[System.Serializable]
public class StageResultList
{
    public List<StageResult> results = new List<StageResult>(); //StageResult의 집합
}
public static class StageResultSaver
{
    private const string FILE = "stage_results.json"; //파일명
    private const string PLAYER_NAME = "PlayerName"; //PlayerPrefs에 플레이어이름
    private static string filePath = Path.Combine(Application.persistentDataPath, FILE); //데이터저장
    public static void SaveStage(int stage, int score)
    {
        StageResultList list = LoadInternal();
        string playerName = PlayerPrefs.GetString(PLAYER_NAME, ""); //PlayerPrefs로 불러오기
        StageResult entry = new StageResult //StageResult 타입 데이터 생성
        {
            playerName = playerName,
            stage = stage,
            score = score
        };
        list.results.Add(entry); //기존 로드 데이터에 엔트리 추가
        string json = JsonUtility.ToJson(list, true); //json 직렬화
        File.WriteAllText(filePath, json); //filePath에 데이터 저장
    }
    public static StageResultList LoadRank()
    {
        return LoadInternal();
    }
    private static StageResultList LoadInternal()
    {
        if (!File.Exists(filePath)) //filePath에 파일이 없다면 새 파일 생성
            return new StageResultList();
        string json = File.ReadAllText(filePath); //filePath의 데이터 읽기
        StageResultList list = JsonUtility.FromJson<StageResultList>(json); //json에서 stageResultList타입으로 데이터 변환
        if (list == null) //list가 비었으면 새 리스트 생성
            return new StageResultList();
        else //아니라면 반환
            return list;
    }
}
