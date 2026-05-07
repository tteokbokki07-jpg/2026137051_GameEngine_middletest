using UnityEngine;
using System.Linq;
using TMPro;
using Unity.VisualScripting;

public class RankPage : MonoBehaviour
{
    [SerializeField] Transform contentRoot;
    [SerializeField] GameObject rowPrefeb;
    public int RankStage = 1;
    public TextMeshProUGUI   RankStageText;

    StageResultList allData;

    private void Awake()
    {
        allData = StageResultSaver.LoadRank();
        RefreshRankList();
    }
    void RefreshRankList()
    {
        foreach (Transform child in contentRoot)
        {
            Destroy(child.gameObject);
        }
        //랭크 데이터 정렬
        var sortedData = allData.results.Where(r => r.stage == RankStage).OrderByDescending(x => x.score).ToList();
        //var sortedData = allData.results.Where(r => r.stage == 1).OrderByDescending(x => x.score).ToList();
        //랭크 데이터 생성
        for (int i = 0; i < sortedData.Count; i++)
        {
            GameObject row = Instantiate(rowPrefeb, contentRoot);
            TMP_Text rankText = row.GetComponentInChildren<TMP_Text>();
            rankText.text = $"{i + 1}. {sortedData[i].playerName} - {sortedData[i].stage}";
        }
    }
    void Update()
    {
        RankStageText.text = $"랭킹 : {RankStage}스테이지";
    }

    public void RankvalueChangeUp()
    {
        if (RankStage < 4)
        {
            RankStage++;
        }

        RefreshRankList();
    }

    public void RankvalueChangeDown()
    {
        if (RankStage > 1)
        {
            RankStage--;
        }

        RefreshRankList();
    }
}
