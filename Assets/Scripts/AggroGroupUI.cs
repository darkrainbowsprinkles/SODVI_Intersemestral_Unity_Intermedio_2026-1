using TMPro;
using UnityEngine;

public class AggroGroupUI : MonoBehaviour
{
    [SerializeField] TMP_Text groupCountText;
    AggroGroup aggroGroup;

    void Awake()
    {
        aggroGroup = FindFirstObjectByType<AggroGroup>();
    }

    void OnEnable()
    {
        aggroGroup.OnChange += RefreshUI;
    }

    void Start()
    {
        RefreshUI();
    }

    void OnDisable()
    {
        aggroGroup.OnChange += RefreshUI;
    }

    void RefreshUI()
    {
        int aliveCount = aggroGroup.GetAliveCount();
        int totalCount = aggroGroup.GetTotalCount();
        groupCountText.text = $"{aliveCount}/{totalCount}";
    }
}
