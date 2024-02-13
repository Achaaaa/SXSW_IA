using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineManager : MonoBehaviour
{
    [SerializeField] private PlayableDirector playableDirector;

    // private 
    [SerializeField] private bool forcePlayMode = false;
    void Start()
    {
        playableDirector.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (forcePlayMode) return;

        // Debug.Log("state "+playableDirector.state);
        double managerTime = (double)MotionManager.shared.ableton_time / 1000;
        
        double diff = managerTime - playableDirector.time;
        
        if(MotionManager.shared.ableton_start == 1){
            // シグナルが再生状態
            // if(playableDirector.state != PlayState.Playing){
            //     playableDirector.Play();
            // }
            if(diff < 0.3f){
                if(diff < 0.05f){
                    Time.timeScale = 1;
                }else{
                    if(managerTime > playableDirector.time){
                        Time.timeScale = (float) (1.0 + diff * 0.1);
                    }else{
                        Time.timeScale = (float) (1.0 - diff * 0.1);
                    }
                }
            }else{
                playableDirector.time = managerTime;
                Time.timeScale = 1;
            };
        }else{
            // シグナルが停止状態
            // if(playableDirector.state == PlayState.Playing){
            //     playableDirector.Pause();
            // }
            playableDirector.time = managerTime;
            Time.timeScale = 1;
        }
        // MotionManager.shared.ableton_time;
        
        
    }
}
