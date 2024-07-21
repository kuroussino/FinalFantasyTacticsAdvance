using System;
using UnityEngine;

public class EventsManager
{
    #region Variables & Properties

    #region Local
    #endregion

    #region Public
    public static Action AllNodesRegistered;
    public static Action AllNodesRegisteredP2;
    public static Action AllNodesRegisteredP3;
    public static Action<NodeB[]> NodeClicked;
    public static Action TurnChanged;
    public static Action<CharacterDirection> DirectionChosen;

    public static Action<Character> ShowControlledCharacterUI;
    public static Action HideAllUI;
    public static Action<Character> ShowTargetUI;
    public static Action<Character, Character> ShowAttackUI;
    public static Action ShowConformationUI;

    public static Action HideActionAndShowMenu;

    public static Action<bool> toggleMoveBtn;
    public static Action<bool> toggleAttackBtn;

    //Turns
    public static Action MoveClicked;
    public static Action FightClicked;
    public static Action<Character> TargetChosen;

    public static Action<int> StartDmgShow;
    public static Action EndDmgShow;
    #endregion

    #endregion

    #region Monobehaviour
    #endregion

    #region Methods
    #endregion
}
