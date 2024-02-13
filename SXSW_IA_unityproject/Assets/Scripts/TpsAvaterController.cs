using UnityEngine;
using UnityEngine.InputSystem;

public class TpsAvaterController : MonoBehaviour
{
    private OscDevice _OscDevice;
    //public GameObject managerObj;
    //MotionManager manager;

    public GameObject tpscam;
    [SerializeField] private PlayerInput _playerInput;
    private InputAction _handsUp;
    private InputAction _clap;
    private InputAction _pointL;
    private InputAction _turnL;
    private InputAction _pointR;
    private InputAction _turnR;
    private InputAction _jump;
    private InputAction _goForword;
    

    private void Awake()
    {
        // デバイスの追加
        _OscDevice = InputSystem.AddDevice<OscDevice>();
        
        //アクション登録
        _handsUp= _playerInput.actions["HandsUp"] ;
        _clap= _playerInput.actions["Clap"] ;
        _pointL= _playerInput.actions["Point_L"] ;
        _turnL= _playerInput.actions["Turn_L"] ;
        _pointR= _playerInput.actions["Point_R"] ;
        _turnR= _playerInput.actions["Turn_R"] ;
        _jump= _playerInput.actions["Jump"] ;
        _goForword= _playerInput.actions["Go_Forword"] ;
        
    }

    private void OnDestroy()
    {
        // デバイスの削除
        InputSystem.RemoveDevice(_OscDevice);
    }

    // (int, [0, 1, 2]) # 両手の動作。0 = アイドル, 1 = handsup, 2 = clap
    // (int, [0, 1, 2]) # 左手の動作。0 = アイドル, 1 = point, 2 = turn
    // (int, [0, 1, 2]) # 両足の動作。0 = アイドル, 1 = jump, 2 = Goforword

    private void Update()
    {
        if(_handsUp != null){
            if(_handsUp.IsPressed()){
                MotionManager.shared.arms_action = 1; // handsup
                MotionManager.shared.arms_handsup = 1;
            }else{
                MotionManager.shared.arms_handsup = 0;
            }
        }

        if(_clap != null){
            if(_clap.IsPressed()){
                MotionManager.shared.arms_action = 2; // clap
                MotionManager.shared.arms_clap = 1;
            }else{
                MotionManager.shared.arms_clap = 0;
            }
        }

        if(_pointL != null){
            if(_pointL.IsPressed()){
                MotionManager.shared.arm_L_action = 1; // point
                MotionManager.shared.arm_L_point = 1;
            }else{
                MotionManager.shared.arm_L_point = 0;
            }
        }

        if(_turnL != null){
            if(_turnL.IsPressed()){
                MotionManager.shared.arm_L_action = 2; // turn
                MotionManager.shared.arm_L_turn = 1;
            }else{
                MotionManager.shared.arm_L_turn = 0;
            }
        }

        if(_pointR != null){
            if(_pointR.IsPressed()){
                MotionManager.shared.arm_R_action = 1; // point
                MotionManager.shared.arm_R_point = 1;
            }else{
                MotionManager.shared.arm_R_point = 0;
            }
        }

        if(_turnR != null){
            if(_turnR.IsPressed()){
                MotionManager.shared.arm_R_action = 2; // turn
                MotionManager.shared.arm_R_turn = 1;
            }else{
                MotionManager.shared.arm_R_turn = 0;
            }
        }

        if(_jump != null){
            if(_jump.IsPressed()){
                MotionManager.shared.legs_action = 1; // jump
                MotionManager.shared.legs_jump = 1;
            }else{
                MotionManager.shared.legs_jump = 0;
            }
        }

        if(_goForword != null){
            if(_goForword.IsPressed()){
                MotionManager.shared.legs_action = 2; // goForward
                MotionManager.shared.legs_walk_forward = 1;
                this.transform.position += transform.forward * 0.01f;
            }else{
                MotionManager.shared.legs_walk_forward = 0;
            }
        }
        Vector3 direction = new Vector3 (tpscam.transform.eulerAngles.x, tpscam.transform.eulerAngles.y, tpscam.transform.eulerAngles.z);
        transform.rotation = Quaternion.Euler(0f, direction.y, 0f);
        
    }
}