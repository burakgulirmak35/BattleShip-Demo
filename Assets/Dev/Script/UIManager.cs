using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI txt_Message;
    [SerializeField] private Button btn_Rotate;

    public event EventHandler Rotate;

    private void Awake()
    {
        Instance = this;
        btn_Rotate.onClick.AddListener(BtnRotate);
    }

    public void MessageText(string _message)
    {
        txt_Message.text = _message;
        Debug.Log(_message);
    }

    private void BtnRotate()
    {
        Rotate?.Invoke(this, EventArgs.Empty);
    }
}
