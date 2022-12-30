using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameplay;
using Gameplay.Turns;
using UnityEngine.UI;

public class TurnMenu : EdgeMenu, iDynamicObject, iFreeze
{
    public Turn ActiveTurn { get { return TurnManager.Instance.ActiveTurn; } }
    public List<Button> PhaseButtons = new List<Button>();

    [SerializeField]
    private Button BattleButton;
    [SerializeField]
    private Button EndButton;

    
    
    public GameObject menuObject;

    #region Interface
    [SerializeField]
    private DynamicObject _dynamicObject = null;
    public DynamicObject dynamicObject
    {
        get
        {
            return _dynamicObject;
        }
        set
        {
            _dynamicObject = value;
        }
    }
    
    #endregion

    #region Initialization
    private void Awake()
    {
        Game.OnNewPhaseStart += OnNewPhaseStart;
        Close();
    }

    private void OnNewPhaseStart(Turn turn, int phaseIndex)
    {
        
        Refresh();
        SetPhase(phaseIndex);
        
    }
    #endregion


    #region overrides
    public void OpenCloseButton()
    {
        ToggleOpen();
    }
    protected override void Open()
    {
        _isOpen = true;
        menuObject.SetActive(true);
        this.MoveToWaypoint("Open");
        this.Freeze();
        //menuObject.transform.localPosition = new Vector3(GetComponent<RectTransform>().rect.width, transform.localPosition.y, -2f);

    }

    protected override void Close()
    {
        _isOpen = false;

        this.MoveToWaypoint("Close");
        this.Thaw();
        //menuObject.transform.localPosition = new Vector3(-GetComponent<RectTransform>().rect.width, transform.localPosition.y, -2f);
    }
    #endregion

    #region Phase Commands and Changing
    protected void Refresh()
    {
        for (int i = 0; i < PhaseButtons.Count; i++)
        {
            PhaseButtons[i].image.color = Color.white;
            PhaseButtons[i].interactable = false;
        }
    }
    protected void SetPhase(int index)
    {
        for (int i = 0; i < PhaseButtons.Count; i++)
        {
            if (i < index)
            {
                PhaseButtons[i].image.color = Color.red;
            }
            else if (i == index)
            {
                PhaseButtons[i].image.color = Color.green;
            }
            if (i > index)
            {
                PhaseButtons[i].image.color = Color.white;
            }
        }

        BattleButton.interactable = index == 1;
        EndButton.interactable = index > 0;
    }

    public void BattleCommand()
    {
        App.AskYesNo("Do you want to start the Battle Phase?", StartBattlePhase);
    }
    protected void StartBattlePhase(bool startPhase)
    {
        if (startPhase)
        {
            TurnManager.StartBattlePhase();
            Close();
        }
    }
    public void EndCommand()
    {
        App.AskYesNo("Do you want to End your turn?", StartEndPhase);
    }
    protected void StartEndPhase(bool startPhase)
    {
        if (startPhase)
        {
            TurnManager.StartEndPhase();
            Close();
        }
    }
    #endregion

    private void OnDestroy()
    {
        Game.OnNewPhaseStart -= OnNewPhaseStart;
    }
}
