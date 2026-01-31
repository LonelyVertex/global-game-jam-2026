using UnityEngine;

[ExecuteInEditMode]
public class DebugLevelGenerator : MonoBehaviour
{
    public LevelGenerator levelGenerator;

    [Space]
    public bool generate;

    private void Update()
    {
        if (generate)
        {
            StartCoroutine(levelGenerator.Generate());
            generate = false;
        }
    }
}
