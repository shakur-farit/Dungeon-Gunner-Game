using UnityEngine;
using UnityEngine.Rendering.Universal;

[DisallowMultipleComponent]
public class LightFlicker : MonoBehaviour
{
    [SerializeField] private float _lightIntensityMin;
    [SerializeField] private float _lightIntensityMax;
    [SerializeField] private float _lightFlickerMin;
    [SerializeField] private float _lightFlickerMax;

    private float _lightFlickerTime;
    private Light2D _light;

    private void Awake()
    {
        _light = GetComponent<Light2D>();
    }

    private void Start()
    {
        _lightFlickerTime = Random.Range(_lightFlickerMin, _lightFlickerMax);
    }

    private void Update()
    {
        if (_light == null)
            return;

        _lightFlickerTime -= Time.deltaTime;

        if(_lightFlickerTime < 0)
        {
            _lightFlickerTime = Random.Range(_lightFlickerMin, _lightFlickerMax);

            RandomiseLightIntensity();
        }
    }

    private void RandomiseLightIntensity()
    {
        _light.intensity = Random.Range(_lightIntensityMin, _lightIntensityMax);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckPositiveRange(this, nameof(_lightFlickerMin), _lightFlickerMin,
            nameof(_lightFlickerMax), _lightFlickerMax, false);

        HelperUtilities.ValidateCheckPositiveRange(this, nameof(_lightIntensityMin), _lightIntensityMin,
            nameof(_lightIntensityMax), _lightIntensityMax, false);
    }
#endif
}
