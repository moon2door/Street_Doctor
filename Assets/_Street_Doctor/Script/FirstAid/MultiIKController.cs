using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Animations;

[System.Serializable]
public class IKTargetSet
{
    public string name;
    public Transform targetBone;         // Tip 본 (예: Foot_end)
    public Transform ikTarget;           // IK 타겟 오브젝트
    public Transform hand;               // 왼손/오른손
    public float activationDistance = 0.1f; // 더 민감하게 반응하도록 거리 조정
    public OVRInput.Controller controller = OVRInput.Controller.RTouch;

    [HideInInspector] public Vector3 originalPosition; // 초기 위치 저장
    [HideInInspector] public bool isFollowing = false;
    // ✅ 회전용 Constraint 추가
    public RotationConstraint rotationConstraint;
}

public class MultiIKController : MonoBehaviour
{
    public List<IKTargetSet> ikTargets = new List<IKTargetSet>();
    public float followSpeed = 20f;          // 손 위치로 따라가는 속도
    public float returnSpeed = 5f;           // 원래 위치로 돌아가는 속도
    public float maxStretchDistance = 0.6f; // ✅ 최대 허용 거리 (늘어짐 방지)

    void Start()
    {
        foreach (var set in ikTargets)
        {
            if (set.ikTarget != null)
                set.originalPosition = set.ikTarget.position;
            if (set.rotationConstraint != null)
                set.rotationConstraint.weight = 0f;
        }
    }
    void Update()
    {
        // 타겟별 따라가고 있는 손의 수 기록용
        Dictionary<Transform, int> activeFollows = new Dictionary<Transform, int>();

        foreach (var set in ikTargets)
        {
            if (set.hand == null || set.targetBone == null || set.ikTarget == null)
                continue;

            float distance = Vector3.Distance(set.hand.position, set.targetBone.position);
            bool isInRange = distance < set.activationDistance;
            bool isPressing = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, set.controller);

            if (isInRange && isPressing)
            {
                // ✅ 손과 타겟 사이 거리 제한
                Vector3 targetPos = set.hand.position;
                float stretch = Vector3.Distance(targetPos, set.targetBone.position);

                if (stretch > maxStretchDistance)
                {
                    Vector3 dir = (targetPos - set.targetBone.position).normalized;
                    targetPos = set.targetBone.position + dir * maxStretchDistance;
                }

                // 타겟 이동
                set.ikTarget.position = Vector3.Lerp(set.ikTarget.position, targetPos, Time.deltaTime * followSpeed);
                set.isFollowing = true;

                // 손 하나라도 붙으면 카운트
                if (!activeFollows.ContainsKey(set.ikTarget))
                    activeFollows[set.ikTarget] = 0;
                activeFollows[set.ikTarget]++;

                if (set.rotationConstraint != null)
                    set.rotationConstraint.weight = 1f;
            }
            // 복귀 처리 (손이 멀어진 경우)
            foreach (var item in ikTargets)
            {
                if (!set.isFollowing) continue;

                float dist = Vector3.Distance(set.hand.position, set.targetBone.position);
                bool stillInRange = dist < set.activationDistance;
                bool stillPressing = OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger, set.controller);

                if (!(stillInRange && stillPressing))
                {
                    int othersFollowing = activeFollows.ContainsKey(set.ikTarget) ? activeFollows[set.ikTarget] : 0;

                    if (othersFollowing <= 0)
                    {
                        set.ikTarget.position = Vector3.Lerp(set.ikTarget.position, set.originalPosition, Time.deltaTime * returnSpeed);

                        if (Vector3.Distance(set.ikTarget.position, set.originalPosition) < 0.01f)
                            set.isFollowing = false;

                        if (set.rotationConstraint != null)
                            set.rotationConstraint.weight = 0f;
                    }
                }
            }
        }
    }
}

