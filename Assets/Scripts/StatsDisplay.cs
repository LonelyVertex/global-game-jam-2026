using System.Text;
using TMPro;
using UnityEngine;

public class StatsDisplay : MonoBehaviour
{
    public TextMeshProUGUI statsText;

    private readonly StringBuilder _sb = new();

    private void OnEnable()
    {
        _sb.Clear();

        var stats = PlayerStats.Instance;
        _sb.AppendLine($"{stats.maxHitpoints + stats.hitpointsBonus}");
        _sb.AppendLine($"{(stats.hitpointsRegen + stats.regenerationBonus):F1}");
        _sb.AppendLine($"{stats.armor:F1}");
        _sb.AppendLine($"{stats.evasion:F1}");
        _sb.AppendLine();
        _sb.AppendLine($"{stats.GetDamangeScaler():F1}");
        _sb.AppendLine($"{stats.attackSpeed}");
        _sb.AppendLine($"0.2");
        _sb.AppendLine();
        _sb.AppendLine($"{(stats.movementSpeed + stats.movementSpeedBonus):F1}");

        statsText.SetText(_sb.ToString());
    }
}
