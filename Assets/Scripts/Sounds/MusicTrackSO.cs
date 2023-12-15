using UnityEngine;

[CreateAssetMenu(fileName = "MusicTrack_", menuName = "Scriptable Objects/Sounds/Music Track")]
public class MusicTrackSO : ScriptableObject 
{
    [Header("Music Track Deatils")]
    public string _trackName;
    public AudioClip _trackClip;
    [Range(0f, 1f)]
    public float _trackVolume = 1f;

#if UNITY_EDITOR
    private void OnValidate()
    {
        {
            HelperUtilities.ValidateCheckEmptyString(this, nameof(_trackName), _trackName);
            HelperUtilities.ValidateCheckNullValue(this, nameof(_trackClip), _trackClip);
            HelperUtilities.ValidateCheckPositiveValue(this, nameof(_trackVolume), _trackVolume, true);
        }
    }
#endif
}
