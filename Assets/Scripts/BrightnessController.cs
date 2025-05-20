using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Volume))]
public class BrightnessController : MonoBehaviour
{
    public static BrightnessController Instance;
    
    [SerializeField] private Volume volume;
    private float _defaultValue;

    private ColorAdjustments _colorAdjustments;
    private float _value;

    public float Value
    {
        get => _value;
        set
        {
            _value = value;
            if (_colorAdjustments != null)
            {
                Debug.Log($"Brightness: {value}");
                _colorAdjustments.postExposure.value = value;
            }
        }
    }

    /// <summary>
    /// Make unique persistent instance
    /// </summary>
    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else Destroy(gameObject);
    }

    /// <summary>
    /// Apply the default brightness value
    /// </summary>
    private void Start() => AdjustExposure();

    /// <summary>
    /// Apply the brightness value on scene that have a Volume.
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        volume = FindFirstObjectByType<Volume>();
        AdjustExposure();
    }

    private void AdjustExposure()
    {
        if (volume != null)
        {
            Debug.Log("volume found");
            if (volume.profile.TryGet(out _colorAdjustments))
            {
                Debug.Log("ColorAdjustments found");
                _colorAdjustments.postExposure.value = Value;
            }
            else Debug.LogWarning("ColorAdjustments NOT found");
        } 
        else Debug.LogWarning("volume NOT found");
    }
}