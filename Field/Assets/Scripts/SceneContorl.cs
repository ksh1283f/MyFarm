using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum STEP
{ // 게임 상태.
    NONE = -1, // 상태 정보 없음.
    PLAY = 0, // 플레이 중.
    CLEAR, // 클리어 상태.
    GAMEOVER, // 게임 오버 상태.
    NUM, // 상태가 몇 종류인지 나타낸다(=3).
};

public class SceneContorl : MonoBehaviour
{
    // 아래 변수 추가
    private GameStatus game_status = null;
    private MaxScore maxScore = null;
    private PlayerControl player_control = null;

    private float GAME_OVER_TIME = 60.0f; // 제한시간은 60초.
    public STEP step = STEP.NONE; // 현대 단계.
    public STEP next_step = STEP.NONE; // 다음 단계.
    public float step_timer = 0.0f; // 타이머.
    private float clear_time = 0.0f; // 클리어 시간.
    public GUIStyle guistyle; // 폰트 스타일.
    void Start()
    {
        this.game_status = this.gameObject.GetComponent<GameStatus>();
        this.player_control = GameObject.Find("Player").GetComponent<PlayerControl>();
        maxScore = GameObject.Find("MaxScore").GetComponent<MaxScore>();
        maxScore.ResetPresentRecord();

        this.step = STEP.PLAY;
        this.next_step = STEP.PLAY;
        this.guistyle.fontSize = 64;
    }
    // 게임을 클리어했는지 또는 게임 오버인지 판정하고 게임 상태를 전환
    void Update()
    {
        //this.step_timer += Time.deltaTime;
        if (this.next_step == STEP.NONE)
        {
            switch (this.step)
            {
                case STEP.PLAY:
                    if (this.game_status.isGameClear())
                    {
                        // 클리어 상태로 이동.
                        this.next_step = STEP.CLEAR;
                    }
                    if (this.game_status.isGameOver())
                    {
                        // 게임 오버 상태로 이동.
                        this.next_step = STEP.GAMEOVER;
                    }
                    if (this.step_timer > GAME_OVER_TIME)
                    { // 제한 시간을 넘었으면 게임 오버. 
                        this.next_step = STEP.GAMEOVER;
                    }
                    break;
                // 클리어 시 및 게임 오버 시의 처리.
                case STEP.CLEAR:
                case STEP.GAMEOVER:
                    maxScore.SetScore(game_status.Gold);
                    SceneManager.LoadScene("ResultScene");
                    //if (Input.GetMouseButtonDown(0))
                    //{
                    //    // 마우스 버튼이 눌렸으면 GameScene을 다시 읽는다.
                    //    SceneManager.LoadScene("GameScene");
                    //}
                    break;
            }
        }
        while (this.next_step != STEP.NONE)
        {
            this.step = this.next_step;
            this.next_step = STEP.NONE;
            switch (this.step)
            {
                case STEP.CLEAR:
                    // PlayerControl을 제어 불가로.
                    this.player_control.enabled = false;
                    // 현재의 경과 시간으로 클리어 시간을 갱신.
                    this.clear_time = this.step_timer;
                    break;
                case STEP.GAMEOVER:
                    // PlayerControl를 제어 불가.
                    this.player_control.enabled = false;
                    break;
            }
            this.step_timer = 0.0f;
        }

    }
}
