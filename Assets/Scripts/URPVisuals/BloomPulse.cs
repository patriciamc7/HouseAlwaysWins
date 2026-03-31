using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BloomPulse : MonoBehaviour
{
    private Volume volume;
    private Bloom bloom;

    void Start()
    {
        volume = GetComponent<Volume>();
        volume.profile.TryGet(out bloom);
    }

    void Update()
    {
        if (bloom == null) return;
        bloom.intensity.value = 1f + Mathf.PerlinNoise(Time.time * 0.25f, 0f) * 14f;
    }
}