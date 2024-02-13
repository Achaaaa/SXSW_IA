using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;
using System.Threading;
using System;
using System.Linq;
using System.ComponentModel;

public class OSCManager : MonoBehaviour
{
    [Header("Flags")]
    [SerializeField] bool is_tokyo = false;
    [SerializeField] bool is_talk_part = false;


    [Header("Server")]
    [SerializeField] string computed_sever_address = "";
    int port = 0;
    const int linz_unity_port = 12003;
    const int tokyo_unity_port = 13003;
    const int linz_talkpart_port = 12008;
    const int tokyo_talkpart_port = 13008;

    OscServer server;

    [Header("Client")]
    [SerializeField] string computed_client_address = "";
    // client
    string masterAddress = "192.168.2.10";
    int masterPort = 12001;

    const string linz_master_address = "192.168.2.10";
    const string tokyo_master_address = "192.168.3.10";
    const string local_master_address = "127.0.0.1";
    const int linz_master_port = 12001;
    const int tokyo_master_port = 13005; // mirror

    OscClient client;
    OscClient clientLocal;
    float lastOSCToMasterTime = 0;
    const float oscToMasterFPS = 30;

    // for Convert
    int camera_lookright;

    void OnEnable()
    {
        if (is_tokyo) {
            if (is_talk_part) {
                port = tokyo_talkpart_port;
            } else {
                port = tokyo_unity_port;
            }

            masterAddress = tokyo_master_address;
            masterPort = tokyo_master_port;
        } else {
            if (is_talk_part) {
                port = linz_talkpart_port;
            } else {
                port = linz_unity_port;
            }

            masterAddress = linz_master_address;
            masterPort = linz_master_port;
        }

        computed_sever_address = $"127.0.0.1:{port}";
        computed_client_address = $"{masterAddress}:{masterPort}";
        

        client = new OscClient(masterAddress, masterPort);
        clientLocal = new OscClient(local_master_address, masterPort);

        if (server == null)
        {
            server = new OscServer(port);
        }

        Dictionary<string, OscMessageDispatcher.MessageCallback> osc_callbacks = new Dictionary<string, OscMessageDispatcher.MessageCallback>() {
            
            //test
            { "/camera/look_right", // (int, [0, 1, 2]) # 右を向く
                (string address, OscDataHandle data) => {
                    camera_lookright = data.GetElementAsInt(0);
            } },
            
            { "/motion/arms_action", // (int, [0, 1, 2]) # 両手の動作。0 = アイドル, 1 = ハンズアップ, 2 = 手拍子
                (string address, OscDataHandle data) => {
                    int intValue = data.GetElementAsInt(0);
                    MotionManager.shared.arms_action = intValue;
            } },
            { "/motion/arm_L_action", // (int, [0, 1, 2]) # 左手の動作。0 = アイドル, 1 = 指差し、2 = 腕回し
                (string address, OscDataHandle data) => { 
                    int intValue = data.GetElementAsInt(0);
                    MotionManager.shared.arm_L_action = intValue;
            } },
            { "/motion/arm_R_action", // (int, [0, 1, 2]) # 右手の動作。0 = アイドル, 1 = 指差し、2 = 腕回し
                (string address, OscDataHandle data) => { 
                    int intValue = data.GetElementAsInt(0);
                    MotionManager.shared.arm_R_action = intValue;
            } },
            { "/motion/legs_action", // (int, [0, 1, 2]) # 両足の動作。0 = アイドル, 1 = ジャンプ, 2 = 歩きまわる
                (string address, OscDataHandle data) => { 
                    int intValue = data.GetElementAsInt(0);
                    MotionManager.shared.legs_action = intValue;
            } },

            // ### タイミング情報

            { "/motion/time_bpm", // # (float) テンポ情報, 60で一秒に一泊
                (string address, OscDataHandle data) => {
                    float floatValue = data.GetElementAsFloat(0);
                    MotionManager.shared.time_bpm = floatValue;
            } },
            { "/motion/time_tick", // # (int [0, 1]) 拍頭で1, それ以外で0
                (string address, OscDataHandle data) => {
                    int intValue = data.GetElementAsInt(0);
                    MotionManager.shared.time_tick = intValue;
            } },

            // ### 首の向き

            { "/motion/neck_angle", // (float, [-180~180])
                (string address, OscDataHandle data) => {
                    float floatValue = data.GetElementAsFloat(0);
                    MotionManager.shared.neck_angle = floatValue;
            } },

            // ### 手拍子

            { "/motion/arms_clap", // (int, [0, 1])
                (string address, OscDataHandle data) => {
                int intValue = data.GetElementAsInt(0);
                MotionManager.shared.arms_clap = intValue;
            } },

            // ### 手を回す（3）

            { "/motion/arm_L_turn", // (int, [0, 1])
                (string address, OscDataHandle data) => {
                    int intValue = data.GetElementAsInt(0);
                    MotionManager.shared.arm_L_turn = intValue;
            } },
            { "/motion/arm_R_turn", // (int, [0, 1])
                (string address, OscDataHandle data) => {
                    int intValue = data.GetElementAsInt(0);
                    MotionManager.shared.arm_R_turn = intValue;
            } },

            // ### 指差し（1）

            { "/motion/arm_L_point", // (int, [0, 1])
                (string address, OscDataHandle data) => {
                    int intValue = data.GetElementAsInt(0);
                    MotionManager.shared.arm_L_point = intValue;
            } },
            { "/motion/arm_R_point", // (int, [0, 1])
                (string address, OscDataHandle data) => {
                    int intValue = data.GetElementAsInt(0);
                    MotionManager.shared.arm_R_point = intValue;
            } },

            // ### ハンズアップ

            { "/motion/arms_handsup", // (int, [0, 1])
                (string address, OscDataHandle data) => {
                    int intValue = data.GetElementAsInt(0);
                    MotionManager.shared.arms_handsup = intValue;
            } },

            // ### ジャンプ（2）

            { "/motion/legs_jump", // (int, [0, 1))
                (string address, OscDataHandle data) => {
                    int intValue = data.GetElementAsInt(0);
                    MotionManager.shared.legs_jump = intValue;
            } },
                // アイドル状態 = ステップ、筋電入力でジャンプ

            // ### 歩きまわる（4）

            { "/motion/legs_walk_forward", // (int, [0, 1])
                (string address, OscDataHandle data) => {
                    int intValue = data.GetElementAsInt(0);
                    MotionManager.shared.legs_walk_forward = intValue;
            } },
            { "/motion/legs_walk_speed", // (float, [0~], default: 1)
                (string address, OscDataHandle data) => {
                    float floatValue = data.GetElementAsFloat(0);
                    MotionManager.shared.legs_walk_speed = floatValue;

            } },
            { "/motion/legs_walk_direction", // (Vector3, (x, 0, z), length=1)
                (string address, OscDataHandle data) => {
                    float x = data.GetElementAsFloat(0);
                    float y = data.GetElementAsFloat(1);
                    float z = data.GetElementAsFloat(2);
                    Vector3 vector3Value = new Vector3(x, y, z);
                    MotionManager.shared.legs_walk_direction = vector3Value;
            } },

            // 位置情報の管理
            { "/motion/root_position", // (Vector3, (x, y, z))
                (string address, OscDataHandle data) => {
                    float x = data.GetElementAsFloat(0);
                    float y = data.GetElementAsFloat(1);
                    float z = data.GetElementAsFloat(2);
                    Vector3 vector3Value = new Vector3(x, y, z);
                    MotionManager.shared.root_position = vector3Value;
            } },

            // Ableton Live関連情報
            { "/ableton/time", // (int [msec])
                (string address, OscDataHandle data) => {
                    int intValue = data.GetElementAsInt(0);
                    MotionManager.shared.ableton_time = intValue;
            } },
            { "/ableton/start", // (int)
                (string address, OscDataHandle data) => {
                int intValue = data.GetElementAsInt(0);
                MotionManager.shared.ableton_start = intValue;
            } },

            // 疎通確認
            { "/ping", 
                (string address, OscDataHandle data) => {
                int intValue = data.GetElementAsInt(0);
                MotionManager.shared.ping = intValue;

                if (is_talk_part) {
                    client.Send("/unity_talkpart/ping", intValue);
                    clientLocal.Send("/unity_talkpart/ping", intValue);
                } else {
                    client.Send("/unity/ping", intValue);
                    clientLocal.Send("/unity/ping", intValue);
                }

            } },
        };

        foreach(var (osc_address, callback) in osc_callbacks) {
            server.MessageDispatcher.AddCallback( osc_address, callback);
        }
    }

    void OnDisable() {
        server?.Dispose();
        server = null;

        client?.Dispose();
        client = null;
        clientLocal?.Dispose();
        clientLocal = null;
    }


    // Start is called before the first frame update
    void Start() {
    }

    void Update() {
        float oscTimeDelta = Time.time - lastOSCToMasterTime;

        // トークパートのプログラムからは信号を送らない
        bool shouldSendPositionState = !is_talk_part & (oscTimeDelta > 1 / oscToMasterFPS);

        if (shouldSendPositionState) {
            client.Send("/unity/position_state", MotionManager.shared.position_state);
            clientLocal.Send("/unity/position_state", MotionManager.shared.position_state);
            lastOSCToMasterTime = Time.time;
        }
    }
}
