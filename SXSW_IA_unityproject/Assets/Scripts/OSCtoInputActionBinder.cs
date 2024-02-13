using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using OscJack;
using System.Linq;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;

public class OSCtoInputActionBinder : MonoBehaviour
{
    OscDevice _oscDevice;

    void Start(){
        _oscDevice = InputSystem.GetDevice<OscDevice>();
        setOSC();
    }

    void Update(){
        
    }

    OscServer server;
    public int port;
    public string address;

    public void setOSC()
    {
        if (server == null)
            {
                server = new OscServer(port);
            }

        Dictionary<string, OscMessageDispatcher.MessageCallback> osc_callbacks = new Dictionary<string, OscMessageDispatcher.MessageCallback>() {       
            { "/inputAction/osc/a", 
                (string address, OscDataHandle data) => {
                    _oscDevice.setMessage(0, data.GetElementAsInt(0));
            } },
            { "/inputAction/osc/b", 
                (string address, OscDataHandle data) => {
                    _oscDevice.setMessage(1,data.GetElementAsInt(0));
            } },
            { "/inputAction/osc/c", 
                (string address, OscDataHandle data) => {
                    _oscDevice.setMessage(2,data.GetElementAsInt(0));
            } },
            { "/inputAction/osc/d", 
                (string address, OscDataHandle data) => {
                    _oscDevice.setMessage(3,data.GetElementAsInt(0));
            } },
        };

        foreach(var (osc_address, callback) in osc_callbacks) {
            server.MessageDispatcher.AddCallback( osc_address, callback);
        }
    }
}

// カスタムデバイスの入力状態
[StructLayout(LayoutKind.Explicit)]
public struct OscDeviceState : IInputStateTypeInfo
{
    // フォーマット識別子
    public FourCC format => new FourCC('O', 'S', 'C', 'D');

    // ボタン
    [FieldOffset(0)] 
    [InputControl(name = "A", layout = "Button", bit = 0, displayName = "OSC_A")]
    [InputControl(name = "B", layout = "Button", bit = 1, displayName = "OSC_B")]
    [InputControl(name = "C", layout = "Button", bit = 2, displayName = "OSC_C")]
    [InputControl(name = "D", layout = "Button", bit = 3, displayName = "OSC_D")]
    public byte buttons;
}



// カスタムデバイス
[InputControlLayout(displayName = "OSC", stateType = typeof(OscDeviceState))]
#if UNITY_EDITOR
// Unityエディタで初期化処理を呼び出すのに必要
[UnityEditor.InitializeOnLoad]
#endif
public class OscDevice : InputDevice, IInputUpdateCallbackReceiver
{
    // ボタン
    public ButtonControl buttonA { get; private set; }
    public ButtonControl buttonB { get; private set; }
    public ButtonControl buttonC { get; private set; }
    public ButtonControl buttonD { get; private set; }

    //OSCtoInputActionBinder binder;

    // 初期化
    static OscDevice()
    {
        // デバイスのレイアウトを登録
        InputSystem.RegisterLayout<OscDevice>();
    }

    public int OSC_A, OSC_B, OSC_C, OSC_D;

    // セットアップ完了時に呼び出される
    protected override void FinishSetup()
    {
        base.FinishSetup();
        // ボタンを取得
        buttonA = GetChildControl<ButtonControl>("A");
        buttonB = GetChildControl<ButtonControl>("B");
        buttonC = GetChildControl<ButtonControl>("C");
        buttonD = GetChildControl<ButtonControl>("D");
    }

    public void OnUpdate()
    {
        var state = new OscDeviceState();
        {
            if(OSC_A == 1)  state.buttons |= 1 << 0;
            if(OSC_B == 1)  state.buttons |= 1 << 1;
            if(OSC_C == 1)  state.buttons |= 1 << 2;
            if(OSC_D == 1)  state.buttons |= 1 << 3;
        };

        // 入力状態をキューに追加
        InputSystem.QueueStateEvent(this, state);
    }

    public void setMessage(int _index, int _val){      
        if(_index == 0) OSC_A = _val;
        if(_index == 1) OSC_B = _val;
        if(_index == 2) OSC_C = _val;
        if(_index == 3) OSC_D = _val;

    }
}
