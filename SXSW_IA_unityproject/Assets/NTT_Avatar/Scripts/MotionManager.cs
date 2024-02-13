using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionManager : MonoBehaviour
{
    // 移動モードの指定。Masterへの送信用。
    // 0: スタート地点に固定
    // 1: 自由移動
    // 2: DJ卓に固定
    [Header("Motion Flags")]
    public bool should_play_DJ = false;
    public bool is_talk_part = false;

    [Header("Info to Master")]
    public int position_state = 0;

    [Header("Info from Master")]

    // 疎通確認
    public int ping = 0;

    // ### アニメーション選択状態

    public int arms_action; // (int, [0, 1, 2]) # 両手の動作。0 = アイドル, 1 = ハンズアップ, 2 = 手拍子
    public int arm_L_action; // (int, [0, 1, 2]) # 左手の動作。0 = アイドル, 1 = 指差し、2 = 腕回し
    public int arm_R_action; // (int, [0, 1, 2]) # 右手の動作。0 = アイドル, 1 = 指差し、2 = 腕回し
    public int legs_action; // (int, [0, 1, 2]) # 両足の動作。0 = アイドル, 1 = ジャンプ, 2 = 歩きまわる

    // ### タイミング情報

    public float time_bpm; // # (float) テンポ情報, 60で一秒に一泊
    public int time_tick; // # (int [0, 1]) 拍頭で1, それ以外で0
    public int ableton_time; // (int [msec]) ableton liveの再生位置
    public int ableton_start; // (int [0, 1]) ableton liveのスタート信号

    // ### 首の向き

    public float neck_angle; // (float, [-180~180])

    // ### ハンズアップ

    public int arms_handsup; // (int, [0, 1])

    // ### 手拍子

    public int arms_clap; // (int, [0, 1])

    // ### 指差し（1）
    public int arm_L_point; // (int, [0, 1])
    public int arm_R_point; // (int, [0, 1])

    // ### 手を回す（3）

    public int arm_L_turn; // (int, [0, 1])
    public int arm_R_turn; // (int, [0, 1])


    // ### ジャンプ（2）

    public int legs_jump; // (int, [0, 1))
        // アイドル状態 = ステップ、筋電入力でジャンプ

    // ### 歩きまわる（4）

    public int legs_walk_forward; // (int, [0, 1])
    public float legs_walk_speed; // (float, [0~], default: 1)
    public Vector3 legs_walk_direction; // (Vector3, (x, 0, z), length=1)

    public Vector3 root_position;

    public static MotionManager shared;

    void OnEnable() {
        shared = this;
    }

    void OnDisable() {
        shared = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


// `MotionManager.shared.legs_walk_forward`