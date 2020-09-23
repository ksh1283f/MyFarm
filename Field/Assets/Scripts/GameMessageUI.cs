using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameMessageUI : MonoBehaviour
{
    [SerializeField] Text messageText;
    [SerializeField] Image backGround;
    private string message;
    public string Message
    {
        get { return message; }
        set
        {
            if (value == message)
                return;
            message = value;
            messageText.text = message;
            if (string.IsNullOrEmpty(message))
                backGround.enabled = false;
            else
                backGround.enabled = true;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Message = string.Empty;
    }

}
