using UnityEngine;

[CreateAssetMenu(menuName = "Game/Item", fileName = "NewItem")]
public class ItemSO : ScriptableObject
{
    [Header("Score Value")]
    public int point = 10;
}
