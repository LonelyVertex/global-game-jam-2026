using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private MaskSelectionPanel maskSelectionPanel;
    [Header("Prefabs")]
    [SerializeField] private GameObject maskProjectileManagerPrefab;

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ShowMaskSelection()
    {
        maskSelectionPanel.GenerateOptions();
        maskSelectionPanel.gameObject.SetActive(true);

        Time.timeScale = 0;
    }

    public void EquipMask(MaskInfo maskInfo)
    {
        var playerTransform = PlayerStats.Instance.transform;

        var managerObject = Instantiate(maskProjectileManagerPrefab, playerTransform);
        managerObject.GetComponent<MaskProjectileManager>().SetMaskInfo(maskInfo);

        maskSelectionPanel.gameObject.SetActive(false);
        Time.timeScale = 1;
    }
}
