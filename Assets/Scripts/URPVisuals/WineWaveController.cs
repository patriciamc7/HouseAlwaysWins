using UnityEngine;

/// <summary>
/// Al hacer click:
///   1. Lanza la onda circular (como antes).
///   2. Dispara un impulso de _IdleStrength que sube de golpe y decae
///      suavemente de vuelta al valor base — simula el líquido agitado.
/// </summary>
[RequireComponent(typeof(Renderer))]
public class WineWaveController : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Dejar vacío → usa el primer material del Renderer.")]
    public Material wineMaterial;

    [Header("Wave Settings")]
    [Range(0.3f, 5f)] public float waveDuration = 2.0f;
    [Range(0.1f, 3f)] public float waveSpeed = 0.5f;
    [Range(5f, 50f)] public float waveFrequency = 22f;
    [Range(0f, 0.2f)] public float waveAmplitude = 0.06f;
    [Range(1f, 10f)] public float waveFalloff = 2.5f;
    [Range(1f, 4f)] public float waveBrightness = 2.0f;

    [Header("Idle Liquid")]
    [Tooltip("Valor base de agitación en reposo.")]
    [Range(0f, 0.1f)] public float idleStrengthBase = 0.012f;
    [Tooltip("Valor máximo al que sube el idle en el momento del click.")]
    [Range(0f, 0.3f)] public float idleStrengthPeak = 0.12f;
    [Tooltip("Cuántos segundos tarda en volver al valor base (decay exponencial).")]
    [Range(0.5f, 6f)] public float idleDecayDuration = 2.5f;
    [Range(0f, 0.1f)] public float idleSpeed = 0.6f;

    [Header("Debug")]
    public bool debugMode = true;
    public bool spaceToTest = true;

    // ── IDs shader ────────────────────────────────────────────────────────
    static readonly int ID_ClickPosLocal = Shader.PropertyToID("_ClickPosLocal");
    static readonly int ID_WaveTime = Shader.PropertyToID("_WaveTime");
    static readonly int ID_WaveSpeed = Shader.PropertyToID("_WaveSpeed");
    static readonly int ID_WaveFrequency = Shader.PropertyToID("_WaveFrequency");
    static readonly int ID_WaveAmplitude = Shader.PropertyToID("_WaveAmplitude");
    static readonly int ID_WaveDuration = Shader.PropertyToID("_WaveDuration");
    static readonly int ID_WaveFalloff = Shader.PropertyToID("_WaveFalloff");
    static readonly int ID_WaveBrightness = Shader.PropertyToID("_WaveBrightness");
    static readonly int ID_IdleStrength = Shader.PropertyToID("_IdleStrength");
    static readonly int ID_IdleSpeed = Shader.PropertyToID("_IdleSpeed");

    Camera _cam;
    Renderer _rend;

    // Estado del impulso de idle
    float _idleCurrent;
    float _idleImpulse;
    float _impulseStartTime;
    bool _impulseActive;

    // ──────────────────────────────────────────────────────────────────────
    void Awake()
    {
        _cam = Camera.main;
        _rend = GetComponent<Renderer>();

        if (wineMaterial == null)
            wineMaterial = _rend.material;

        _idleCurrent = idleStrengthBase;

        SyncParams();
        wineMaterial.SetFloat(ID_WaveTime, -999f);
        wineMaterial.SetFloat(ID_IdleStrength, idleStrengthBase);
        wineMaterial.SetFloat(ID_IdleSpeed, idleSpeed);

        if (debugMode)
            Debug.Log("[WineWave] Listo con impulso de idle en click.");
    }

    // ──────────────────────────────────────────────────────────────────────
    void Update()
    {
        // ── Animar idle strength de vuelta al base ─────────────────────
        if (_impulseActive)
        {
            float elapsed = Time.time - _impulseStartTime;
            float t = Mathf.Clamp01(elapsed / idleDecayDuration);

            // Ease-out: baja rápido al principio, suave al final
            float decay = 1f - Mathf.Pow(t, 0.35f);

            _idleCurrent = idleStrengthBase + _idleImpulse * decay;
            wineMaterial.SetFloat(ID_IdleStrength, _idleCurrent);

            if (t >= 1f)
            {
                _impulseActive = false;
                _idleCurrent = idleStrengthBase;
                wineMaterial.SetFloat(ID_IdleStrength, idleStrengthBase);
            }
        }

        // ── Input ──────────────────────────────────────────────────────
        if (spaceToTest && Input.GetKeyDown(KeyCode.Space))
        {
            TriggerWaveAtLocalPos(Vector3.zero);
            if (debugMode) Debug.Log("[WineWave] TEST centro (Space)");
            return;
        }

        if (Input.GetMouseButtonDown(0))
            HandleClick(Input.mousePosition);

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            HandleClick(Input.GetTouch(0).position);
    }

    // ──────────────────────────────────────────────────────────────────────
    void HandleClick(Vector2 screenPos)
    {
        Ray ray = _cam.ScreenPointToRay(screenPos);

        if (!Physics.Raycast(ray, out RaycastHit hit))
        {
            if (debugMode) Debug.Log("[WineWave] Raycast no golpeó nada.");
            return;
        }

        if (hit.transform != transform)
        {
            if (debugMode) Debug.Log($"[WineWave] Golpeó otro objeto: {hit.transform.name}");
            return;
        }

        Vector3 localHit = transform.InverseTransformPoint(hit.point);

        if (debugMode)
            Debug.Log($"[WineWave] localHit={localHit}  t={Time.time:F3}");

        TriggerWaveAtLocalPos(localHit);
    }

    // ──────────────────────────────────────────────────────────────────────
    public void TriggerWaveAtLocalPos(Vector3 localPos)
    {
        SyncParams();

        // Solo impulso de idle — sin onda
        float currentExcess = _idleCurrent - idleStrengthBase;
        _idleImpulse = (idleStrengthPeak - idleStrengthBase) + currentExcess;
        _impulseStartTime = Time.time;
        _impulseActive = true;

        if (debugMode)
            Debug.Log($"[WineWave] Impulso → peak={idleStrengthPeak:F4}  decay={idleDecayDuration}s");
    }

    public void TriggerWaveAtCenter() => TriggerWaveAtLocalPos(Vector3.zero);

    // ──────────────────────────────────────────────────────────────────────
    void SyncParams()
    {
        wineMaterial.SetFloat(ID_WaveSpeed, waveSpeed);
        wineMaterial.SetFloat(ID_WaveFrequency, waveFrequency);
        wineMaterial.SetFloat(ID_WaveAmplitude, waveAmplitude);
        wineMaterial.SetFloat(ID_WaveDuration, waveDuration);
        wineMaterial.SetFloat(ID_WaveFalloff, waveFalloff);
        wineMaterial.SetFloat(ID_WaveBrightness, waveBrightness);
        wineMaterial.SetFloat(ID_IdleSpeed, idleSpeed);
    }

    void OnDestroy()
    {
        if (wineMaterial != null && wineMaterial != _rend.sharedMaterial)
            Destroy(wineMaterial);
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (Application.isPlaying && wineMaterial != null) SyncParams();
    }
#endif
}
