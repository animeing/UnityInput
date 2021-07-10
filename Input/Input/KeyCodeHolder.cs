using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyCodeHolder : MonoBehaviour
{
    public string FunctionName
    {
        get;
        protected set;
    }
    public Dictionary<string, KeyCodeActionDto> KeyCodeActions
    {
        get;
        private set;
    } = new Dictionary<string, KeyCodeActionDto>();

    [HideInInspector]
    public int priority;

    public void AddKeyCodeAction(string keyCode, string keyCodeName, Action keyAction, Action disableAction)
    {
        KeyCodeActionDto keyCodeAction = new KeyCodeActionDto();
        keyCodeAction.KeyCodeName = keyCodeName;
        keyCodeAction.KeyCode = keyCode;
        keyCodeAction.Action = keyAction;
        keyCodeAction.Priority = priority;
        keyCodeAction.DisableAction = disableAction;
        if (KeyCodeActions.ContainsKey(keyCode))
        {
            KeyCodeActions[keyCode] = keyCodeAction;
        }
        else
        {
            KeyCodeActions.Add(keyCode, keyCodeAction);
        }
    }
}
