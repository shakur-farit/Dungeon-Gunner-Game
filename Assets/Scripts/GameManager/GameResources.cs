using UnityEngine;
using UnityEngine.Audio;

public class GameResources : MonoBehaviour
{
    private static GameResources instance;

    public static GameResources Instance
    {
        get
        {
            if (instance == null)
                instance = Resources.Load<GameResources>("GameResources");
            return instance;
        }
    }

    [Header("Dungeon")]
    public RoomNodeTypeListSO roomNodeTypeList;
    [Header("Player")]
    public CurrentPlayerSO currentPlayer;
    [Header("Sounds")]
    public AudioMixerGroup soundMasterMixerGroup;
    public SoundEffectSO doorOpenCloseSoundEffect;
    [Header("Materials")]
    public Material dimmeMaterial;
    public Material litMaterial;
    public Shader variableLitShader;
    [Header("UI")]
    public GameObject ammoIconPrefab;

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(roomNodeTypeList), roomNodeTypeList);
        HelperUtilities.ValidateCheckNullValue(this, nameof(currentPlayer), currentPlayer);
        HelperUtilities.ValidateCheckNullValue(this, nameof(doorOpenCloseSoundEffect), doorOpenCloseSoundEffect);
        HelperUtilities.ValidateCheckNullValue(this, nameof(litMaterial), litMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(dimmeMaterial), dimmeMaterial);
        HelperUtilities.ValidateCheckNullValue(this, nameof(variableLitShader), variableLitShader);
        HelperUtilities.ValidateCheckNullValue(this, nameof(ammoIconPrefab), ammoIconPrefab);
    }
#endif
}
