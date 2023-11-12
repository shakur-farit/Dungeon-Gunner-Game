using Cinemachine;
using UnityEngine;

[DisallowMultipleComponent]
public class Minimap : MonoBehaviour
{
    [SerializeField] private GameObject minimapPlayer;

    private Transform playerTransfrom;

    private void Start()
    {
        playerTransfrom = GameManager.Instance.GetPlayer.transform;
        CinemachineVirtualCamera cinemachineVirtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        cinemachineVirtualCamera.Follow = playerTransfrom;

        SpriteRenderer spriteRenderer = minimapPlayer.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteRenderer.sprite = GameManager.Instance.GetPlayerMinimaoIcon;
    }

    private void Update()
    {
        if (playerTransfrom != null && minimapPlayer != null)
            minimapPlayer.transform.position = playerTransfrom.position;
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        HelperUtilities.ValidateCheckNullValue(this, nameof(minimapPlayer), minimapPlayer);
    }
#endif
}
