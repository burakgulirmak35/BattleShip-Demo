using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    [Space]
    [SerializeField] private TextMeshProUGUI txtMessage;
    [Space]
    [SerializeField] private Button btn_Rotate;
    [SerializeField] private Button btn_Restart;
    [Header("GameOver")]
    [SerializeField] private GameObject panelGameOver;
    [Space]
    [SerializeField] private TextMeshProUGUI txt_Winner;
    [Space]
    [SerializeField] private TextMeshProUGUI txt_p1shotCount;
    [SerializeField] private TextMeshProUGUI txt_p1hitCount;
    [Space]
    [SerializeField] private TextMeshProUGUI txt_time;

    public event EventHandler Rotate;

    private void Awake()
    {
        Instance = this;
        btn_Rotate.onClick.AddListener(BtnRotate);
        btn_Restart.onClick.AddListener(BtnRestart);
    }

    public void MessageText(string _message)
    {
        txtMessage.text = _message;
        Debug.Log(_message);
    }

    private void EndGame(string _winner)
    {
        panelGameOver.SetActive(true);
        txt_Winner.text = _winner + "win";
    }

    private void BtnRotate()
    {
        Rotate?.Invoke(this, EventArgs.Empty);
    }

    private void BtnRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


}
