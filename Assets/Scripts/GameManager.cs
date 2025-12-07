using System;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using TMPro;
using static UIPlayerBoard;

public class GameManager : MonoBehaviour
{
    public static GameManager IN;

    [SerializeField] private string gameName;
    [SerializeField, TextArea(0,10)] private string notes;
    [SerializeField] private TextMeshProUGUI[] gameNameTexts;

    public UICardCell Deck;
    public UIPlayerBoard PlayerBoard => PlayerBoards[0];
    public UIPlayerBoard[] PlayerBoards;

    [Space, SerializeField] private SplashScreen splashScreen;

    private int currentPlayerIndex = 0;
    public UIPlayerBoard CurrentPlayerBoard => this.PlayerBoards[this.currentPlayerIndex];
    public EPlayerType CurrentPlayer => (EPlayerType)this.currentPlayerIndex + 1;
    public bool IsPlayersTurn => this.currentPlayerIndex == 0;

    [PropertySpace(10), Button(ButtonSizes.Large), GUIColor(1, 0, 0)]
    public void DeletePlayerPrefs()
    {
        PlayerPrefs.DeleteAll();

        if (!Application.isPlaying) return;

        SaveManager.IsClearingPlayerPrefs = true;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    private void OnValidate() 
    {
        SetGameNameTexts();
    }

    private void Awake()
    {
        SetGameNameTexts();
        this.splashScreen.gameObject.SetActive(true);
    }

    public void SetGameNameTexts()
    {
        if(string.IsNullOrWhiteSpace(this.gameName)) return;
        
        foreach (var text in this.gameNameTexts)
        {
            text.text = this.gameName;
        }
    }

    private IEnumerator Start()
    {
        DOTween.Init();
        var singletonManager = this.GetComponent<SingletonManager>();
        singletonManager.Init();

        //this doesn't fix the loading bug on first play
        while (DeckManager.IN == null)
        {
            yield return null;
            GC.Collect();
            singletonManager.Init();
        }

        CardsManager.IN.Init();
        SaveManager.IN.LoadGame();

        DeckManager.IN.Init();
    }

    private void OnApplicationPause(bool inIsPaused)
    {
#if !UNITY_EDITOR
        this.splashScreen.gameObject.SetActive(true);
        UIAttackPanel.IN.Hide();
#endif
    }
}