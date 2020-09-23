using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public static float MOVE_AREA_RADIUS = 15.0f; // 섬의 반지름.
    public static float MOVE_SPEED = 5.0f; // 이동 속도.
    private struct Key
    { // 키 조작 정보 구조체.
        public bool up; // ↑.
        public bool down; // ↓.
        public bool right; // →.
        public bool left; // ←.
        public bool pick; // 줍는다／버린다.
        public bool action; // 먹는다 / 수리한다.
        public bool num1;
        public bool num2;
        public bool num3;

    };
    private Key key; // 키 조작 정보를 보관하는 변수.
    public enum STEP
    { // 플레이어의 상태를 나타내는 열거체.
        NONE = -1, // 상태 정보 없음.
        MOVE = 0, // 이동 중.
        REPAIRING, // 수리 중.
        EATING, // 식사 중.
        NUM, // 상태가 몇 종류 있는지 나타낸다(=3).

        SaveLumber, // 나무 저장
        Seed,   // 씨앗 심기
        Harvest,    // 작물 수확
        Watering,   // 물주기
        Sleep,  // 잠자기

        Sell,   // 판매

    };

    public STEP step = STEP.NONE; // 현재 상태.
    public STEP next_step = STEP.NONE; // 다음 상태.
    public float step_timer = 0.0f; // 타이머.
    private GameObject closest_item = null; // 플레이어의 정면에 있는 GameObject.
    private GameObject carried_item = null; // 플레이어가 들어올린 GameObject.
    private ItemRoot item_root = null; // ItemRoot 스크립트를 가짐.
    public GUIStyle guistyle; // 폰트 스타일.

    private GameObject closest_event = null;// 주목하고 있는 이벤트를 저장.
    private EventRoot event_root = null; // EventRoot 클래스를 사용하기 위한 변수.

    private GameStatus game_status = null;
    private PlayerMove playerMove = null;
    [SerializeField] GameMessageUI messageUI;
    AudioSource effect;

    private TYPE seedType = TYPE.NONE;

    void OnClickedSeedBtn(TYPE type)
    {
        seedType = type;
    }
    private void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
        effect = GetComponent<AudioSource>();
    }

    void Start()
    {
        this.step = STEP.NONE; // 현 단계 상태를 초기화.
        this.next_step = STEP.MOVE; // 다음 단계 상태를 초기화.
        this.item_root = GameObject.Find("GameRoot").GetComponent<ItemRoot>();
        this.guistyle.fontSize = 16;

        this.guistyle.fontSize = 16;
        this.event_root =
        GameObject.Find("GameRoot").GetComponent<EventRoot>();

        this.game_status = GameObject.Find("GameRoot").GetComponent<GameStatus>();
        game_status.OnClickedSeedBtn += OnClickedSeedBtn;
    }

    // 입력 정보를 가져오고 상태에 변화가 있을 때의 처리를 거쳐 각 상태별로 실행.
    void Update()
    {
        this.get_input(); // 상호작용 키 입력 정보 취득.
        playerMove.MoveUpdate();

        this.step_timer += Time.deltaTime;
        float eat_time = 2.0f; // 사과는 2초에 걸쳐 먹는다.
        float repair_time = 2.0f; // 수리에 걸리는 시간도 2초.

        // 상태를 변화시킨다---------------------.
        if (this.next_step == STEP.NONE)
        { // 다음 예정이 없으면.
            switch (this.step)
            {
                case STEP.MOVE: // '이동 중' 상태의 처리.
                    do
                    {
                        if (!this.key.action)
                        { // 액션 키가 눌려있지 않다.
                            break; // 루프 탈출.
                        }
                        // 주목하는 이벤트가 있을 때.
                        if (this.closest_event != null)
                        {
                            if (!this.is_event_ignitable())
                            { // 이벤트를 시작할 수 없으면.
                                break; // 아무 것도 하지 않는다.
                            }
                            // 이벤트 종류를 가져온다.
                            Event.TYPE ignitable_event = this.event_root.getEventType(this.closest_event);
                            switch (ignitable_event)
                            {
                                case Event.TYPE.ROCKET: // 이벤트의 종류가 ROCKET이면.
                                                        // REPAIRING(수리) 상태로 이행.
                                    this.next_step = STEP.REPAIRING;
                                    break;
                            }
                            break;
                        }

                        if (this.carried_item != null)
                        {
                            // 가지고 있는 아이템 판별.
                            TYPE carried_item_type = this.item_root.getItemType(this.carried_item);
                            switch (carried_item_type)
                            {
                                case TYPE.APPLE: // 사과라면.
                                case TYPE.PLANT: // 식물이라면.
                                                 // '식사 중' 상태로 이행.
                                    this.next_step = STEP.EATING;
                                    break;
                            }
                        }
                    } while (false);
                    break;
                case STEP.EATING: // '식사 중' 상태의 처리.
                    if (this.step_timer > eat_time)
                    { 
                        // 2초 대기.
                        this.next_step = STEP.MOVE; // '이동' 상태로 이행.
                    }
                    break;
                   
            }
        }
        // 상태가 변화했을 때------------.
        while (this.next_step != STEP.NONE)
        {
            this.step = this.next_step;
            this.next_step = STEP.NONE;
            switch (this.step)
            {
                case STEP.MOVE:
                    break;
                case STEP.EATING: // '식사 중' 상태의 처리.
                    if (this.carried_item != null)
                    {
                        // 가지고 있던 아이템을 폐기.
                        GameObject.Destroy(this.carried_item);
                        this.carried_item = null;
                    }
                    break;
            }
            this.step_timer = 0.0f;
        }

        // 각 상황에서 반복할 것----------.
        switch (this.step)
        {
            case STEP.MOVE:
                playerMove.MoveFixedUpdate();
                this.pick_or_drop_control();
                Sell();
                Seed(); // ui 버튼으로
                Harvest();
                SaveLumber();
                Sleep();
                Watering();

                // 이동 가능한 경우는 항상 배가 고파진다. 
                //this.game_status.alwaysSatiety();
                break;
        }
    }

    // 키 입력을 조사해 그 결과를 바탕으로 맴버 변수 key의 값을 갱신한다.
    private void get_input()
    {
        // Z 키가 눌렸으면 true를 대입.
        this.key.pick = Input.GetKeyDown(KeyCode.Z);
        // X 키가 눌렸으면 true를 대입.
        this.key.action = Input.GetKeyDown(KeyCode.X);
        key.num1 = Input.GetKeyDown(KeyCode.Alpha1);
        key.num2 = Input.GetKeyDown(KeyCode.Alpha2);
        key.num3 = Input.GetKeyDown(KeyCode.Alpha3);
    }

    //(obsolete) 키 입력에 따라 실제로 이동시키는 처리를 한다.
    private void move_control()
    {
        Vector3 move_vector = Vector3.zero; // 이동용 벡터.
        Vector3 position = this.transform.position; // 현재 위치를 보관.
        bool is_moved = false;
        if (this.key.right)
        { // →키가 눌렸으면.
            move_vector += Vector3.right; // 이동용 벡터를 오른쪽으로 향한다.
            is_moved = true; // '이동 중' 플래그.
        }
        if (this.key.left)
        {
            move_vector += Vector3.left;
            is_moved = true;
        }
        if (this.key.up)
        {
            move_vector += Vector3.forward;
            is_moved = true;
        }
        if (this.key.down)
        {
            move_vector += Vector3.back;
            is_moved = true;
        }
        move_vector.Normalize(); // 길이를 1로.
        move_vector *= MOVE_SPEED * Time.deltaTime; // 속도×시간＝거리.
        position += move_vector; // 위치를 이동.
        position.y = 0.0f; // 높이를 0으로 한다.
                           // 세계의 중앙에서 갱신한 위치까지의 거리가 섬의 반지름보다 크면.
                           //if (position.magnitude > MOVE_AREA_RADIUS)
                           //{
                           //    position.Normalize();
                           //    position *= MOVE_AREA_RADIUS; // 위치를 섬의 끝자락에 머물게 한다.
                           //}

        if (move_vector.magnitude > 0.01f)
        {
            Quaternion q = Quaternion.LookRotation(move_vector,
            Vector3.up);
            this.transform.rotation =
            Quaternion.Lerp(this.transform.rotation, q, 0.2f);
            // Lerp의 비율을 0.1f에서 0.2f으로↑↑↑.
        }

        // 새로 구한 위치(position)의 높이를 현재 높이로 되돌린다.
        position.y = this.transform.position.y;
        // 실제 위치를 새로 구한 위치로 변경한다.
        this.transform.position = position;
        // 이동 벡터의 길이가 0.01보다 큰 경우.
        // =어느 정도 이상의 이동한 경우.
        if (move_vector.magnitude > 0.01f)
        {
            // 캐릭터의 방향을 천천히 바꾼다.
            Quaternion q = Quaternion.LookRotation(move_vector, Vector3.up);
            this.transform.rotation =
            Quaternion.Lerp(this.transform.rotation, q, 0.1f);
        }

        if (is_moved)
        {
            // 들고 있는 아이템에 따라 '체력 소모 정도'를 조사한다.
            float consume = this.item_root.getConsumeSatiety(this.carried_item);
            // 가져온 '소모 정도'를 체력에서 뺀다.
            this.game_status.addSatiety(-consume * Time.deltaTime);
        }
    }

    // 주목 중이거나 들고 있는 아이템이 있을 때 표시
    void OnGUI()
    {
        float x = 20.0f;
        float y = Screen.height - 40.0f;

        if (closest_item != null)
        {
            //GUI.Label(new Rect(x, y, 200.0f, 20.0f), "Z:줍는다", guistyle);
            messageUI.Message = string.Concat(closest_item.tag, "\nZ:줍는다");
        }
        else if (closest_event != null)
        {
            if (closest_event.layer == LayerMask.NameToLayer("Mail"))
            {
                messageUI.Message = string.Concat("Mail", "\n1. 모두 판매한다");
            }
            else if (closest_event.layer == LayerMask.NameToLayer("House"))
            {
                messageUI.Message = string.Concat("House", "\n1. 잠자기\n2. 나무저장");
            }
            else if (closest_event.layer == LayerMask.NameToLayer("Farm"))
            {
                string message = "Farm";
                Farm farm = closest_event.GetComponent<Farm>(); // todo 리펙토링 필요한 부분
                if (farm.seed == TYPE.NONE)
                    message = string.Concat(message, "\n씨앗버튼: 심기");
                else
                {
                    if (!farm.isWatered)
                        message = string.Concat(message, "\n2.물뿌리기");
                    if (farm.isCanHarvest())
                        message = string.Concat(message, "\n3. 수확하기");
                }
                messageUI.Message = message;
            }
        }
        else
            messageUI.Message = string.Empty;

       
        switch (step)
        {
            case STEP.EATING:
                GUI.Label(new Rect(x, y, 200.0f, 20.0f),
                "우적우적우물우물……", guistyle);
                break;
            case STEP.REPAIRING:
                GUI.Label(new Rect(x + 200.0f, y, 200.0f, 20.0f), "수리중",
                guistyle);
                break;
            case STEP.SaveLumber:
                GUI.Label(new Rect(x + 200.0f, y, 200.0f, 20.0f), "나무 저장",
                guistyle);
                break;
            case STEP.Seed:
                GUI.Label(new Rect(x + 200.0f, y, 200.0f, 20.0f), "씨앗 심음",
                guistyle);
                break;
            case STEP.Harvest:
                GUI.Label(new Rect(x + 200.0f, y, 200.0f, 20.0f), "수확",
                guistyle);
                break;
            case STEP.Watering:
                GUI.Label(new Rect(x + 200.0f, y, 200.0f, 20.0f), "물을 주었다.",
                guistyle);
                break;
            case STEP.Sleep:
                GUI.Label(new Rect(x + 200.0f, y, 200.0f, 20.0f), "Zzz",
                guistyle);
                break;
            case STEP.Sell:
                GUI.Label(new Rect(x + 200.0f, y, 200.0f, 20.0f), "판매완료",
                guistyle);
                break;
        }

        if (this.is_event_ignitable())
        { // 이벤트가 시작 가능한 경우.
          // 이벤트용 메시지를 취득.
            string message = this.event_root.getIgnitableMessage(this.closest_event);
            GUI.Label(new Rect(x + 200.0f, y, 200.0f, 20.0f),
            "X:" + message, guistyle);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            closest_item = other.gameObject;
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("House"))
        {
            closest_event = other.gameObject;
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Farm"))
        {
            closest_event = other.gameObject;
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Mail"))
        {
            closest_event = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (closest_event != null)
            closest_event = null;

        if (closest_item != null)
            closest_item = null;
    }

    /// <summary>
    /// 물주기
    /// </summary>
    private void Watering()
    {
        do
        {
            if (!this.key.num2)
            { // '줍기/버리기'키가 눌리지 않았으면.
                break; // 아무것도 하지 않고 메소드 종료.
            }
            if (this.closest_event != null)
            {
                if (closest_event.layer != LayerMask.NameToLayer("Farm"))
                    break;
                if (!game_status.IsCanWork(10))
                    break;

                Farm farm = closest_event.GetComponent<Farm>();
                if (farm.isWatered)
                    break;

                effect.Play();
                farm.SetWater();
                game_status.Hp -= 10;   // todo 작물 종류에 맞게 값 변경

            }

        } while (false);
    }

    private void Sell()
    {
        do
        {
            if (!this.key.num1)
            { // 키가 눌리지 않았으면.
                break; // 아무것도 하지 않고 메소드 종료.
            }
            if (this.closest_event != null)
            {
                if (closest_event.layer != LayerMask.NameToLayer("Mail"))
                    break;

                if (!game_status.IsCanWork(10))
                    break;

                effect.Play();
                game_status.SellAllItem();
                game_status.Hp -= 10;
            }

        } while (false);
    }

    private void Seed()
    {
        do
        {
            if (seedType == TYPE.NONE)
            { // 키가 눌리지 않았으면.
                break; // 아무것도 하지 않고 메소드 종료.
            }
            if (this.closest_event != null)
            {
                if (closest_event.layer != LayerMask.NameToLayer("Farm"))
                    break;

                if (game_status.ItemDic[seedType] == 0)
                    break;

                if (!game_status.IsCanWork(10))
                    break;
                Farm farm = closest_event.GetComponent<Farm>();

                if (farm.seed != TYPE.NONE)
                    break;

                if (seedType != TYPE.OSeed && seedType != TYPE.SSeed)
                    game_status.ItemDic[seedType]--;
                
                TYPE tempType = TYPE.NONE;
                E_Kind kind = E_Kind.None;
                switch (seedType)
                {
                    case TYPE.MSeed:
                        tempType = TYPE.Macintosh;
                        kind = E_Kind.A;
                        break;
                    case TYPE.CSeed:
                        tempType = TYPE.Corn;
                        kind = E_Kind.B;
                        break;
                    case TYPE.OSeed:
                        tempType = TYPE.Orange;
                        kind = E_Kind.C;
                        break;
                    case TYPE.TSeed:
                        tempType = TYPE.Tomato;
                        kind = E_Kind.A;
                        break;
                    case TYPE.GSeed:
                        tempType = TYPE.Grape;
                        kind = E_Kind.B;
                        break;
                    case TYPE.SSeed:
                        tempType = TYPE.Strawberry;
                        kind = E_Kind.C;
                        break;
                }
                ItemData data = new ItemData(tempType, kind);
                farm.InitFarm(data);

                effect.Play();
                game_status.Hp -= 10;   // todo 작물 종류에 맞게 값 변경
                game_status.OnUpdatedItemList(game_status.ItemDic);
            }

        } while (false);
        seedType = TYPE.NONE;
    }

    private void Harvest()
    {
        do
        {
            if (!this.key.num3)
            { // 키가 눌리지 않았으면.
                break; // 아무것도 하지 않고 메소드 종료.
            }
            if (this.closest_event != null)
            {
                if (closest_event.layer != LayerMask.NameToLayer("Farm"))
                    break;

                Farm farm = closest_event.GetComponent<Farm>();
                if (farm.seed == TYPE.NONE) // 아무것도 안심겨 있으면
                    break;

                if (farm.remainTurn > 0)    // 다 자라지 않았다면
                    break;

                if (!game_status.IsCanWork(farm.itemData.UseHP))
                    break;
                effect.Play();
                game_status.Hp -= farm.itemData.UseHP;   // todo 작물 종류에 맞게 값 변경
                farm.Harvest();
            }

        } while (false);
    }

    private void SaveLumber()
    {
        do
        {
            if (!this.key.num2)
            { // 키가 눌리지 않았으면.
                break; // 아무것도 하지 않고 메소드 종료.
            }
            if (this.closest_event != null)
            {
                if (closest_event.layer != LayerMask.NameToLayer("House"))
                    break;

                if (game_status.ItemDic[TYPE.Lumber] == 0)
                    break;

                if (!game_status.IsCanWork(10))
                    break;
                effect.Play();
                game_status.Hp -= 10;
                game_status.RemainLumberCount += game_status.ItemDic[TYPE.Lumber];
                game_status.ItemDic[TYPE.Lumber] = 0;

                game_status.OnUpdatedItemList(game_status.ItemDic);
            }

        } while (false);
    }

    private void Sleep()
    {
        do
        {
            if (!this.key.num1)
            { // 키가 눌리지 않았으면.
                break; // 아무것도 하지 않고 메소드 종료.
            }
            if (this.closest_event != null)
            {
                if (closest_event.layer != LayerMask.NameToLayer("House"))
                    break;

                // 턴계산, ui갱신, 작물 갱신
                effect.Play();
                game_status.ChangeTurn();
            }

        } while (false);
    }

    

    


    // 물건을 줍거나 떨어뜨린다.
    private void pick_or_drop_control()
    {
        do
        {
            if (!this.key.pick)
            { // '줍기/버리기'키가 눌리지 않았으면.
                break; // 아무것도 하지 않고 메소드 종료.
            }
            if(closest_item != null)
            {
                Item tempItem = closest_item.GetComponent<Item>();
                if (!game_status.IsCanWork(10))
                    break;

                TYPE type = tempItem.itemData.ItemType;
                effect.Play();
                foreach (var item in game_status.ItemDic)
                {
                    if(item.Key == type)
                    {
                        game_status.GetItem(type, 1);
                        Destroy(closest_item);
                        item_root.respawnedItemList.Remove(closest_item);
                        game_status.Hp -= 10;
                        return;
                    }
                }
                

            }
        } while (false);
    }

    // 접촉한 물건이 자신의 정면에 있는지 판단한다.
    private bool is_other_in_view(GameObject other)
    {
        bool ret = false;
        do
        {
            Vector3 heading = // 자신이 현재 향하고 있는 방향을 보관.
            this.transform.TransformDirection(Vector3.forward);
            Vector3 to_other = // 자신 쪽에서 본 아이템의 방향을 보관.
            other.transform.position - this.transform.position;
            heading.y = 0.0f;
            to_other.y = 0.0f;
            heading.Normalize(); // 길이를 1로 하고 방향만 벡터로.
            to_other.Normalize(); // 길이를 1로 하고 방향만 벡터로.
            float dp = Vector3.Dot(heading, to_other); // 양쪽 벡터의 내적을 취득.
            if (dp < Mathf.Cos(45.0f))
            { // 내적이 45도인 코사인 값 미만이면.
                break; // 루프를 빠져나간다.
            }
            ret = true; // 내적이 45도인 코사인 값 이상이면 정면에 있다.
        } while (false);
        return (ret);
    }

    private bool is_event_ignitable()
    {
        bool ret = false;
        do
        {
            if (this.closest_event == null)
            { // 주목 이벤트가 없으면.
                break; // false를 반환한다.
            }
            // 들고 있는 아이템 종류를 가져온다.
            TYPE carried_item_type = this.item_root.getItemType(this.carried_item);
            // 들고 있는 아이템 종류와 주목하는 이벤트의 종류에서.
            // 이벤트가 가능한지 판정하고, 이벤트 불가라면 false를 반환한다.
            if (!this.event_root.isEventIgnitable(carried_item_type, this.closest_event))
            {
                break;
            }
            ret = true; // 여기까지 오면 이벤트를 시작할 수 있다고 판정!.
        } while (false);
        return (ret);
    }
}
