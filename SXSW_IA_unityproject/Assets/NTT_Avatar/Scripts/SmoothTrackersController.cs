using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothTrackersController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] float clapPositionSpeed = 100;
    [SerializeField] float jumpPositionSpeed = 100;
    [SerializeField] float walkPositionSpeed = 100;
    [SerializeField] float idleSpeed = 10;


    [Header("References")]

    [SerializeField] SmoothTracker head;
    [SerializeField] SmoothTracker leftHand;
    [SerializeField] SmoothTracker rightHand;
    [SerializeField] SmoothTracker weist;
    [SerializeField] SmoothTracker leftFoot;
    [SerializeField] SmoothTracker rightFoot;

    Dictionary<string, float> defaultPositionSpeedDictionary = new Dictionary<string, float>();
    Dictionary<string, float> defaultRotationSpeedDictionary = new Dictionary<string, float>();
    Dictionary<string, Transform> defaultTrackerTargetDictionary = new Dictionary<string, Transform>();

    // 下半身のモーションを上半身のモーションの上に上書きしているかどうか。
    public bool overridingLegs = false;
    // 右手左手を上げ下げの途中かどうかを判定するためのもの。
    public float armLeftTransition = 0;
    public float armRightTransition = 0;
    // DJ卓で操作をする必要があるかどうか
    public bool shouldPlayDJLeft = false;
    public bool shouldPlayDJRight = false;

    public bool isIdleMotion = false;
    [SerializeField] Transform leftDJTarget;
    [SerializeField] Transform rightDJTarget;

    List<SmoothTracker> trackers;

    // Start is called before the first frame update
    void Start()
    {
        trackers = new List<SmoothTracker>() {
            head, weist, leftHand, rightHand, leftFoot, rightFoot,
        };

        foreach (var tracker in trackers) {
            defaultPositionSpeedDictionary[tracker.name] = tracker.positionSpeed;
            defaultRotationSpeedDictionary[tracker.name] = tracker.rotationSpeed;
            defaultTrackerTargetDictionary[tracker.name] = tracker.target;
        }

        leftHand.secondaryTarget = leftDJTarget;
        rightHand.secondaryTarget = rightDJTarget;
    }

    // Update is called once per frame
    void Update()
    {
        ResetPosOffsets();
        ResetSmoothSpeed();

        UpdateIdleMotionSmoothSpeed();

        UpdateHandSmoothSpeed();
        UpdateLegSmoothSpeed();

        UpdateHandTransition();
        UpdateLegsJump();

        UpdateDJ();
    }

    void ResetPosOffsets() {
        // 現在の状態に合わせて、手足の位置を微修正するためのオフセット値の初期化。
        // ジャンプしている時に手も一緒に動くようにとか、
        // 手を挙げる時、突き出すのではなく前に出しながら半円を書いて動かすようにとか

        foreach (var tracker in trackers) {
            tracker.posOffset = new Vector3();
        }
    }

    void ResetSmoothSpeed() {
        foreach (var tracker in trackers) {
            tracker.positionSpeed = defaultPositionSpeedDictionary[tracker.name];
            tracker.rotationSpeed = defaultRotationSpeedDictionary[tracker.name];
        }
    }

    void UpdateIdleMotionSmoothSpeed() {
        var trackers = new List<SmoothTracker>() {
            weist, leftFoot, rightFoot, head, rightHand, leftHand,
        };

        foreach (var tracker in trackers) {
            if (isIdleMotion) {
                tracker.positionSpeed = idleSpeed;
                tracker.rotationSpeed = idleSpeed;
            }
        }

    }

    void UpdateHandSmoothSpeed() {
        // 手拍子時、トラッカーに鋭敏に手が追従して欲しいので、個別にトラッカーのスムージング値を制御する。
        bool isClapping = (MotionManager.shared.arms_action == 2) && (MotionManager.shared.arms_clap > 0);

        var trackers = new List<SmoothTracker>() {
            leftHand, rightHand
        };

        foreach (var tracker in trackers) {
            if (isClapping) {
                tracker.positionSpeed = clapPositionSpeed;
            } 
        }
    }

    void UpdateLegSmoothSpeed() {
        // ジャンプ時、トラッカーに鋭敏に各部位が追従して欲しいので、トラッカーのスムージング値を調整する。
        bool isJumping = (MotionManager.shared.legs_action == 1) && (MotionManager.shared.legs_jump > 0);
        bool isWalking = (MotionManager.shared.legs_action == 2) && (MotionManager.shared.legs_walk_forward > 0);

        var trackers = new List<SmoothTracker>() {
            weist, leftFoot, rightFoot, head, rightHand, leftHand,
        };

        foreach (var tracker in trackers) {
            if (isJumping) {
                tracker.positionSpeed = jumpPositionSpeed;
            } 

            if (isWalking) {
                tracker.positionSpeed = walkPositionSpeed;
            }
        }
    }

    [SerializeField] float armMoveOrientation = 30;
    void UpdateHandTransition() {
        // 手を挙げるときに半円を描くように
        float r = .6f;
        leftHand.posOffset += Quaternion.AngleAxis(-armMoveOrientation, Vector3.up) * new Vector3(0, 0, Mathf.Sin(armLeftTransition * Mathf.PI)) * r;
        rightHand.posOffset += Quaternion.AngleAxis(armMoveOrientation, Vector3.up) * new Vector3(0, 0, Mathf.Sin(armRightTransition * Mathf.PI)) * r;
    }

    FilteredFloat leftDJWeight = new FilteredFloat(.2f);
    FilteredFloat rightDJWeight = new FilteredFloat(.2f);
    void UpdateDJ() {
        // 必要があればDJ卓に手を置く
        leftDJWeight.target = shouldPlayDJLeft ? 1 : 0;
        leftDJWeight.Update();
        leftHand.secondaryTargetWeight = leftDJWeight.current;

        rightDJWeight.target = shouldPlayDJRight ? 1 : 0;
        rightDJWeight.Update();
        rightHand.secondaryTargetWeight = rightDJWeight.current;
    }


    public float defaultHeadZ = -.0f;
    void UpdateLegsJump() {
        // ジャンプするときに、もし手拍子など手のアクションが発動していたら、手足がバラバラに動いているように見えないように、
        // 体の上下移動と手の移動を同期する。
        bool isJumping = (MotionManager.shared.legs_action == 1) && (MotionManager.shared.legs_jump > 0);
        float defaultWeistHeight = 1.0f;

        var upperBody = new List<SmoothTracker>() {
            leftHand, rightHand,
        };

        foreach (var tracker in upperBody) {
            if (isJumping && overridingLegs) {
                var pos = weist.target.transform.position - new Vector3(0, defaultWeistHeight, 0);
                pos.z = head.target.transform.localPosition.z - defaultHeadZ;
                tracker.posOffset += pos;
            }  
        }
    }
}
