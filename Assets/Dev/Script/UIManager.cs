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
    [Header("Game")]
    [SerializeField] private GameObject panelGame;
    [SerializeField] private TextMeshProUGUI txtMessage;
    [Space]
    [SerializeField] private Button btn_Rotate;
    [SerializeField] private Button btn_Restart;
    [Space]
    [SerializeField] private TextMeshProUGUI txt_hitCount;
    [SerializeField] private TextMeshProUGUI txt_missCount;
    [Header("GameOver")]
    [SerializeField] private GameObject panelGameOver;
    [Space]
    [SerializeField] private TextMeshProUGUI txt_Winner;
    [Space]
    [SerializeField] private TextMeshProUGUI txt_totalShotCount;
    [SerializeField] private TextMeshProUGUI txt_totalHitCount;
    [SerializeField] private TextMeshProUGUI txt_totalMissCount;
    [Space]
    [SerializeField] private TextMeshProUGUI txt_time;

    public event EventHandler Rotate;

    private void Awake()
    {
        Instance = this;
        btn_Rotate.onClick.AddListener(BtnRotate);
        btn_Restart.onClick.AddListener(BtnRestart);
        panelGameOver.SetActive(false);
        UpdateText(0, 0);
    }

    public void MessageText(string _message)
    {
        txtMessage.text = _message;
        Debug.Log(_message);
    }

    public void EndGame(string _message, int _hitCount, int _missCount, string _time)
    {
        txt_Winner.text = _message;
        txt_totalHitCount.text = "HitCount: " + _hitCount.ToString();
        txt_totalMissCount.text = "MissCount: " + _missCount.ToString();
        txt_totalShotCount.text = "ShotCount: " + (_hitCount + _missCount);
        txt_time.text = "Time: " + _time;
        panelGame.SetActive(false);
        panelGameOver.SetActive(true);
    }

    private void BtnRotate()
    {
        Rotate?.Invoke(this, EventArgs.Empty);
    }

    private void BtnRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void UpdateText(int _hitCount, int _missCount)
    {
        txt_hitCount.text = "HitCount: " + _hitCount.ToString();
        txt_missCount.text = "MissCount: " + _missCount.ToString();
    }


}
