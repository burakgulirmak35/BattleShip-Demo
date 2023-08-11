using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    private TextMeshProUGUI txt_Message;
    private Button btn_Rotate;

    private void Awake()
    {
        Instance = this;
    }

    public void MessageText(string _message)
    {
        txt_Message.text = _message;
    }

}
