using UnityEngine;
using UnityEngine.Rendering;

public class MaskTextureProvider : MonoBehaviour
{
    public MaskCameraController maskCameraController;

    public RTHandle MaskRTHandle => maskCameraController.maskRTHandle;
}
