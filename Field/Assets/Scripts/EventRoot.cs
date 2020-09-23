using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 처음에 이벤트의 종류를 나타내는 class.
public class Event
{ // 이벤트 종류.
    public enum TYPE
    {
        NONE = -1, // 없음.
        ROCKET = 0, // 우주선 수리.

        //SaveLumber, // 나무 저장
        //Seed,   // 씨앗 심기
        //Harvest,    // 작물 수확
        //Watering,   // 물주기
        //Sleep,  // 잠자기

        //Sell,   // 판매

        House,
        Farm,
        Mail,
    };
};


public class EventRoot : MonoBehaviour
{
    public Event.TYPE getEventType(GameObject event_go)
    {
        Event.TYPE type = Event.TYPE.NONE;
        if (event_go != null)
        { // 인수의 GameObject가 비어있지 않으면.
            if (event_go.layer == LayerMask.NameToLayer("House"))
            {
                //type = Event.TYPE.;
                Debug.Log("get event type is house");
                type = Event.TYPE.House;
            }
            else if (event_go.layer == LayerMask.NameToLayer("Farm"))
            {
                //typ
                Debug.Log("get event type is farm");
                type = Event.TYPE.Farm;
            }
            else if (event_go.layer == LayerMask.NameToLayer("Mail"))
            {
                type = Event.TYPE.Mail;
            }

        }
        return (type);
    }

    // 철광석이나 식물을 든 상태에서 우주선에 접촉했는지 확인
    public bool isEventIgnitable(TYPE carried_item, GameObject event_go)
    {
        bool ret = false;
        Event.TYPE type = Event.TYPE.NONE;
        if (event_go != null)
        {
            type = this.getEventType(event_go); // 이벤트 타입을 구한다.
        }
        switch (type)
        {
            case Event.TYPE.ROCKET:
                if (carried_item == TYPE.IRON)
                { // 가지고 있는 것이 철광석이라면.
                    ret = true; // '이벤트할 수 있어요！'라고 응답한다.
                }
                if (carried_item == TYPE.PLANT)
                { // 가지고 있는 것이 식물이라면.
                    ret = true; // '이벤트할 수 있어요！'라고 응답한다.
                }
                break;
        }
        return (ret);
    }
    // 지정된 게임 오브젝트의 이벤트 타입 반환
    public string getIgnitableMessage(GameObject event_go)
    {
        string message = string.Empty;
        Event.TYPE type = Event.TYPE.NONE;
        if (event_go != null)
        {
            type = this.getEventType(event_go);
        }
        switch (type)
        {
            case Event.TYPE.House:
                message = "1. 나무저장\n2. 잠자기";
                break;
            case Event.TYPE.Farm:
                message = "1. 물뿌리기\n2. 수확하기";
                break;
            case Event.TYPE.Mail:
                message = "1. 모두 팔기";
                break;
        }
        return (message);
    }
}
