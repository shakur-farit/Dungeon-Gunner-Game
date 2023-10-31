using Cinemachine;
using UnityEngine;

[RequireComponent(typeof(CinemachineTargetGroup))]
public class CinemachineTargets : MonoBehaviour
{
    private CinemachineTargetGroup targetGroup;

    private void Awake()
    {
        targetGroup = GetComponent<CinemachineTargetGroup>();
    }

    private void Start()
    {
        SetTargetGroup();
    }

    private void SetTargetGroup()
    {
        CinemachineTargetGroup.Target groupTarget_player = new CinemachineTargetGroup.Target
        {
            weight = 1f,
            radius = 1f,
            target = GameManager.Instance.GetPlayer.transform
        };

        CinemachineTargetGroup.Target[] targetArray = new CinemachineTargetGroup.Target[]
        {
            groupTarget_player
        };

        targetGroup.m_Targets = targetArray;
    }
}
