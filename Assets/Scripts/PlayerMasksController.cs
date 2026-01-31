using UnityEngine;

public class PlayerMasksController : MonoBehaviour
{
    public static PlayerMasksController Instance { get; private set; }

    [SerializeField] private Transform masksRoot;
    [SerializeField] private float maskPlacementIncrement;

    private float _zPosition;

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

    public void EquipMask(MaskInfo maskInfo)
    {
        var tr = Instantiate(maskInfo.playerMaskPrefab, masksRoot).transform;
        tr.localPosition = new Vector3(0.0f, 0.0f, _zPosition);

        _zPosition += maskPlacementIncrement;
    }
}
