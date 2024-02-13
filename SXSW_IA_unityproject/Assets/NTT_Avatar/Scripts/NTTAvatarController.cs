using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;


public class NTTAvatarController : MonoBehaviour
{
    [SerializeField] Animator trackerAnimator;
    [SerializeField] Animator avatarAnimator;
    [SerializeField] SmoothTrackersController smoothTrackerController;

    readonly int hash_sync_beat = Animator.StringToHash("sync_beat");
    readonly int hash_legs_idle = Animator.StringToHash("legs_idle");
    readonly int hash_legs_jump = Animator.StringToHash("legs_jump");
    readonly int hash_legs_jump_small = Animator.StringToHash("legs_jump_small");
    readonly int hash_legs_walk = Animator.StringToHash("legs_walk");
    readonly int hash_arm_L_idle = Animator.StringToHash("arm_L_idle");
    readonly int hash_arm_L_point = Animator.StringToHash("arm_L_point");
    readonly int hash_arm_L_wave = Animator.StringToHash("arm_L_wave");
    readonly int hash_arm_both_point = Animator.StringToHash("arm_both_point");
    readonly int hash_arm_R_idle = Animator.StringToHash("arm_R_idle");
    readonly int hash_arm_R_point = Animator.StringToHash("arm_R_point");
    readonly int hash_arm_R_turn = Animator.StringToHash("arm_R_turn");
    readonly int hash_arms_idle = Animator.StringToHash("arms_idle");
    readonly int hash_arms_handsup = Animator.StringToHash("arms_handsup");
    readonly int hash_arms_clap = Animator.StringToHash("arms_clap");
    readonly int hash_idle = Animator.StringToHash("idle");

    int tracker_base_layer_index;
    int tracker_legs_layer_index;
    int tracker_legs_override_layer_index;
    int tracker_arm_L_layer_index;
    int tracker_arm_R_layer_index;
    int tracker_arm_R_override_layer_index;
    int tracker_arm_both_layer_index;

    int avatar_base_layer_index;
    int avatar_hand_L_layer_index;
    int avatar_hand_R_layer_index;

    List<int>trackerAnimatorTriggerHashs = new List<int>();
    List<int>avatarAnimatorTriggerHashs = new List<int>();

    bool isRight;

    // Start is called before the first frame update
    void Start()
    {
        LoadLayerIndex();
        LoadTriggerHashes();
    }

    void LoadLayerIndex() {
        tracker_base_layer_index = trackerAnimator.GetLayerIndex("Base");
        tracker_arm_L_layer_index = trackerAnimator.GetLayerIndex("Arm Left");
        tracker_arm_R_layer_index = trackerAnimator.GetLayerIndex("Arm Right");
        tracker_arm_R_override_layer_index = trackerAnimator.GetLayerIndex("Arm Right Override");
        tracker_arm_both_layer_index = trackerAnimator.GetLayerIndex("Arm Both");

        tracker_legs_layer_index = trackerAnimator.GetLayerIndex("Legs");
        tracker_legs_override_layer_index = trackerAnimator.GetLayerIndex("Legs Override");
        
        //

        avatar_base_layer_index = trackerAnimator.GetLayerIndex("Base");
        avatar_hand_L_layer_index = trackerAnimator.GetLayerIndex("Left Hand");
        avatar_hand_R_layer_index = trackerAnimator.GetLayerIndex("Right Hand");
    }

    void LoadTriggerHashes() {
        for (int i = 0; i < trackerAnimator.parameterCount; i++)
        {
            var p =  trackerAnimator.GetParameter(i);
            if (p.type == AnimatorControllerParameterType.Trigger) {
                trackerAnimatorTriggerHashs.Add(p.nameHash);
            }
        }

        for (int i = 0; i < avatarAnimator.parameterCount; i++)
        {
            var p =  avatarAnimator.GetParameter(i);
            if (p.type == AnimatorControllerParameterType.Trigger) {
                avatarAnimatorTriggerHashs.Add(p.nameHash);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateActions();
        UpdatePosition();
    }

    void ResetAllTriggers() {
        foreach(var hash in trackerAnimatorTriggerHashs) {
            trackerAnimator.ResetTrigger(hash);
        }

        foreach(var hash in avatarAnimatorTriggerHashs) {
            avatarAnimator.ResetTrigger(hash);
        }
    }

    void ResetActionFlags() {
        arm_L_active = false;
        arm_R_active = false;
        arm_both_active = false;

        arm_R_override_active = false;

        legs_active = false;
        legs_override_active = false;
    }

    FilteredFloat armLeftLayerWeight = new FilteredFloat(0.3f);
    FilteredFloat armRightLayerWeight = new FilteredFloat(0.3f);
    FilteredFloat armRightOverrideLayerWeight = new FilteredFloat(0.3f);
    FilteredFloat armBothLayerWeight = new FilteredFloat(0.3f);
    FilteredFloat legsOverrideLayerWeight = new FilteredFloat(0.3f);

    bool arm_L_active = false;
    bool arm_R_active = false;
    bool arm_both_active = false;

    bool arm_R_override_active = false;

    bool legs_active = false;
    bool legs_override_active = false;
    void UpdateActions() {
        ResetAllTriggers();
        ResetActionFlags();

        UpdateArmRightLayer();
        UpdateArmLeftLayer();
        UpdateArmRightOverrideLayer();
        UpdateArmBothLayer();
        UpdateBaseAndLegsOverrideLayer();

        UpdateDJ();
        UpdateIdleMotion();
    }

    void UpdateArmRightLayer() {
        var manager = MotionManager.shared;

        int arm_R_point_index = 1;
        int arm_R_turn_index = 2;

        bool arm_R_point = (manager.arm_R_action == arm_R_point_index) && (manager.arm_R_point > 0);
        bool arm_R_turn = (manager.arm_R_action == arm_R_turn_index) && (manager.arm_R_turn > 0);

        if (arm_R_point) {
            trackerAnimator.SetTrigger(hash_arm_R_point);
            avatarAnimator.SetTrigger(hash_arm_R_point);
            arm_R_active = true;
        } else {
            avatarAnimator.SetTrigger(hash_arm_R_idle);
        }

        if (arm_R_turn) {
            trackerAnimator.SetTrigger(hash_arm_R_turn);
            arm_R_active = true;
        } 

        if (!arm_R_active) {
            trackerAnimator.SetTrigger(hash_arm_R_idle);
        }

        armRightLayerWeight.target = arm_R_active ? 1 : 0;
        armRightLayerWeight.Update();
        trackerAnimator.SetLayerWeight(tracker_arm_R_layer_index, armRightLayerWeight.current);

        smoothTrackerController.armRightTransition = armRightLayerWeight.current;
    }

    void UpdateArmLeftLayer() {
        var manager = MotionManager.shared;

        int arm_L_point_index = 1;
        int arm_L_wave_index = 2;

        bool arm_L_point = (manager.arm_L_action == arm_L_point_index) && (manager.arm_L_point > 0);
        bool arm_L_wave = (manager.arm_L_action == arm_L_wave_index) && (manager.arm_L_turn > 0);

        if (arm_L_point) {
            trackerAnimator.SetTrigger(hash_arm_L_point);
            avatarAnimator.SetTrigger(hash_arm_L_point);
            arm_L_active = true;
        } else {
            avatarAnimator.SetTrigger(hash_arm_L_idle);
        }

        if (arm_L_wave) {
            trackerAnimator.SetTrigger(hash_arm_L_wave);
            arm_L_active = true;
        } 


        if (!arm_L_active) {
            trackerAnimator.SetTrigger(hash_arm_L_idle);
            avatarAnimator.SetTrigger(hash_arm_L_idle);
        }

        armLeftLayerWeight.target = arm_L_active ? 1 : 0;
        armLeftLayerWeight.Update();
        trackerAnimator.SetLayerWeight(tracker_arm_L_layer_index, armLeftLayerWeight.current);

        smoothTrackerController.armLeftTransition = armLeftLayerWeight.current;
    }

    void UpdateArmRightOverrideLayer() {
        var manager = MotionManager.shared;

        if (arm_L_active && arm_R_active && !arm_both_active) {
            arm_R_override_active = true;
        }

        armRightOverrideLayerWeight.target = arm_R_override_active ? 1 : 0;
        armRightOverrideLayerWeight.Update();
        trackerAnimator.SetLayerWeight(tracker_arm_R_override_layer_index, armRightOverrideLayerWeight.current);
    }

    void UpdateArmBothLayer() {
        var manager = MotionManager.shared;

        int arms_handsup_index = 1;
        int arms_clap_index = 2;
        int arm_L_point_index = 1;
        int arm_R_point_index = 1;

        bool arms_handsup = (manager.arms_action == arms_handsup_index) && (manager.arms_handsup > 0);
        bool arms_clap = (manager.arms_action == arms_clap_index) && (manager.arms_clap > 0);

        bool arm_R_point = (manager.arm_R_action == arm_R_point_index) && (manager.arm_R_point > 0);
        bool arm_L_point = (manager.arm_L_action == arm_L_point_index) && (manager.arm_L_point > 0);

        if (arms_handsup) {
            trackerAnimator.SetTrigger(hash_arms_handsup);
            arm_both_active = true;
        }

        if (arms_clap) {
            trackerAnimator.SetTrigger(hash_arms_clap);
            arm_both_active = true;
        }

        if (arm_L_point && arm_R_point) {
            trackerAnimator.SetTrigger(hash_arm_both_point);
            arm_both_active = true;
        } 
        
        if (!arm_both_active) {
            trackerAnimator.SetTrigger(hash_arms_idle);
        }

        armBothLayerWeight.target = arm_both_active ? 1 : 0;
        armBothLayerWeight.Update();
        trackerAnimator.SetLayerWeight(tracker_arm_both_layer_index, armBothLayerWeight.current);

        smoothTrackerController.armLeftTransition = Mathf.Max(smoothTrackerController.armLeftTransition, armBothLayerWeight.current);
        smoothTrackerController.armRightTransition = Mathf.Max(smoothTrackerController.armRightTransition, armBothLayerWeight.current);
    }

    void UpdateBaseAndLegsOverrideLayer() {
        var manager = MotionManager.shared;

        int legs_jump_index = 1;
        int legs_walk_index = 2;

        bool legs_jump = (manager.legs_action == legs_jump_index) && (manager.legs_jump > 0);
        bool legs_walk = (manager.legs_action == legs_walk_index) && (manager.legs_walk_forward > 0);

        if (MotionManager.shared.is_talk_part) {
            legs_walk = false;
        }

        if (legs_jump) {
            // if (arm_L_active || arm_R_active) {
            //     trackerAnimator.SetTrigger(hash_legs_jump_small);
            // } else {
            trackerAnimator.SetTrigger(hash_legs_jump);
            // }
            legs_active = true;
        }

        if (legs_walk) {
            trackerAnimator.SetTrigger(hash_legs_walk);
            legs_active = true;
        }

        if (!legs_active) {
            trackerAnimator.SetTrigger(hash_legs_idle);
        }

        if (legs_active && (arm_both_active || arm_L_active || arm_R_active)) {
            legs_override_active = true;
        }

        smoothTrackerController.overridingLegs = legs_override_active;

        legsOverrideLayerWeight.target = legs_override_active ? 1 : 0;
        legsOverrideLayerWeight.Update();
        trackerAnimator.SetLayerWeight(tracker_legs_override_layer_index, legsOverrideLayerWeight.current);
    }

    void UpdateDJ() {
        // DJプレイをするのは後半だけ
        if (!MotionManager.shared.should_play_DJ) {
            smoothTrackerController.shouldPlayDJLeft = false;
            smoothTrackerController.shouldPlayDJRight = false;
            return;
        }

        if (arm_L_active || arm_both_active) {
            smoothTrackerController.shouldPlayDJLeft = false;
        } else {
            smoothTrackerController.shouldPlayDJLeft = true;
        }

        if (arm_R_active || arm_both_active) {
            smoothTrackerController.shouldPlayDJRight = false;
        } else {
            smoothTrackerController.shouldPlayDJRight = true;
        }
    }

    void UpdateIdleMotion() {
        bool isIdleMotion = 
            (!arm_both_active) &&
            (!arm_L_active) &&
            (!arm_R_active) &&
            (!legs_active);

        smoothTrackerController.isIdleMotion = isIdleMotion;
    }

    void UpdatePosition() {
        //if (MotionManager.shared.is_talk_part) {
            //transform.position = new Vector3(0, 0, 0);
            //transform.rotation = Quaternion.Euler(0, 180, 0);
            //return;
        //}

        // Vector3 direction = new Vector3 (tpscam.transform.eulerAngles.x, tpscam.transform.eulerAngles.y, tpscam.transform.eulerAngles.z);
        // float angle = Mathf.Atan2(direction.x, direction.z);
        
        //TDで保持してる座標を引っ張る場合、２拠点同期パフォーマンス前提
        // Vector3 position = new Vector3(MotionManager.shared.root_position.x, MotionManager.shared.root_position.y, -MotionManager.shared.root_position.z);
        // position.x *= -1;
        // position.y *= -1;
        // Vector3 direction = -MotionManager.shared.legs_walk_direction;
        // float angle = Mathf.Atan2(direction.x, direction.z);

        float lerpSpeed = 0.9999f;
        //transform.position = Vector3.Lerp(transform.position, position, 1f - Mathf.Pow(1 - lerpSpeed, Time.deltaTime));
        //var rotation = Quaternion.Euler(0.0f, Mathf.Rad2Deg * angle , 0.0f);
        //transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 1f - Mathf.Pow(1 - lerpSpeed, Time.deltaTime));

    }

}
