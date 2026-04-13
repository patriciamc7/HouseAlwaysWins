using UnityEngine;
using UnityEngine.UI;

public class LocalizedText : MonoBehaviour
{
    [SerializeField] private string _key;
    private Text _text;

    void Awake()
    {
        _text = GetComponent<Text>();
    }

    void OnEnable()
    {
        _text = GetComponent<Text>();
        LocalizationManager.OnLanguageChanged += UpdateText;
        UpdateText();
    }

    void OnDisable()
    {
        LocalizationManager.OnLanguageChanged -= UpdateText;
    }

    private void UpdateText()
    {
        if (LocalizationManager.Instance != null )
            _text.text = LocalizationManager.Instance.Get(_key);
    }
}