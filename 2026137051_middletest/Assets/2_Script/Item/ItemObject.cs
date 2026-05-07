using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField] ItemSO data;
    public int GetPoint()
    {
        return data.point;
    }

}
