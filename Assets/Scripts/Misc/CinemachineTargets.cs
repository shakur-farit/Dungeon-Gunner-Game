using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineTargetGroup))]
public class CinemachineTargets : MonoBehaviour
{
    private CinemachineTargetGroup targetGroup;

    [SerializeField] private Transform cursorTarget;

    private void Awake()
    {
        targetGroup = GetComponent<CinemachineTargetGroup>();
    }

    private void Start()
    {
        SetTargetGroup();
    }
    private void Update()
    {
        cursorTarget.position = HelperUtilities.GetMouseWorldPosition();
    }

    private void SetTargetGroup()
    {
        CinemachineTargetGroup.Target groupTarget_player = new CinemachineTargetGroup.Target
        {
            weight = 1f,
            radius = 2.5f,
            target = GameManager.Instance.GetPlayer.transform
        };

        CinemachineTargetGroup.Target groupTarget_cursor = new CinemachineTargetGroup.Target
        {
            weight = 1f,
            radius = 1f,
            target = cursorTarget
        };

        CinemachineTargetGroup.Target[] targetArray = new CinemachineTargetGroup.Target[]
        {
            groupTarget_player,
            groupTarget_cursor
        };

        targetGroup.m_Targets = targetArray;
    }
}
