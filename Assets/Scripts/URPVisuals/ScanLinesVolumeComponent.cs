using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[System.Serializable, VolumeComponentMenu("Custom/ScanLines")]
public class ScanLinesVolumeComponent : VolumeComponent
{
    public BoolParameter enabled = new BoolParameter(false);
    public FloatParameter intensity = new FloatParameter(0.03f);
    public FloatParameter lineCount = new FloatParameter(300f);
}