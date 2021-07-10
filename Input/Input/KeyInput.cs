using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class KeyInput : MonoBehaviour
{
    public Dictionary<string, KeyCodeHolder> KeyCodeActions
    {
        get;
        private set;
    } = new Dictionary<string, KeyCodeHolder>();
    public ConcurrentDictionary<string, KeyCodeActionDto> CurrentKeyCodeAction
    {
        get;
        private set;
    } = new ConcurrentDictionary<string, KeyCodeActionDto>();
    public Dictionary<string, List<KeyCodeActionDto>> KeyCodeHistory
    {
        get;
        private set;
    } = new Dictionary<string, List<KeyCodeActionDto>>();

    void Update()
    {
        foreach(KeyCodeActionDto keyCodeAction in CurrentKeyCodeAction.Values)
        {
            if(keyCodeAction == null)
            {
                continue;
            }
            keyCodeAction.Action();
        }
    }
    public bool AddKeyCodeActions(KeyCodeHolder keyCodeHolder)
    {
        if (KeyCodeActions.ContainsKey(keyCodeHolder.FunctionName))
        {
            return false;
        }
        KeyCodeActions.Add(keyCodeHolder.FunctionName, keyCodeHolder);
        return true;
    }

    public void RemoveKeyCodeActions(KeyCodeHolder keyCodeHolder)
    {
        if(KeyCodeActions.ContainsKey(keyCodeHolder.FunctionName))
        {
            KeyCodeActions.Remove(keyCodeHolder.FunctionName);
        }
        foreach(string keyCode in keyCodeHolder.KeyCodeActions.Keys)
        {
            if (KeyCodeHistory.ContainsKey(keyCode))
            {
                KeyCodeHistory[keyCode].Remove(keyCodeHolder.KeyCodeActions[keyCode]);
            }

        }
    }
    public void ActiveKeyCodeActionHolder(string functionName)
    {
        foreach(KeyCodeActionDto keyCodeAction in KeyCodeActions[functionName].KeyCodeActions.Values)
        {
            if(!CurrentKeyCodeAction.ContainsKey(keyCodeAction.KeyCode))
            {
                CurrentKeyCodeAction.TryAdd(keyCodeAction.KeyCode, keyCodeAction);
                AddKeyCodeHistory(keyCodeAction);
                continue;
            }
            if(CurrentKeyCodeAction[keyCodeAction.KeyCode].Priority < keyCodeAction.Priority)
            {
                CurrentKeyCodeAction[keyCodeAction.KeyCode].DisableAction();
                CurrentKeyCodeAction[keyCodeAction.KeyCode] = keyCodeAction;
                AddKeyCodeHistory(keyCodeAction);
            }
        }
    }
    private void AddKeyCodeHistory(KeyCodeActionDto keyCodeAction)
    {
        if(!KeyCodeHistory.ContainsKey(keyCodeAction.KeyCode))
        {
            KeyCodeHistory.Add(keyCodeAction.KeyCode, new List<KeyCodeActionDto>());
        }
        KeyCodeHistory[keyCodeAction.KeyCode].Add(keyCodeAction);
    }

    public void BackHistory()
    {
        int maxPriority = GetCurrentMaxPriority();
        foreach(KeyCodeActionDto keyCodeAction in new List<KeyCodeActionDto>(CurrentKeyCodeAction.Values))
        {
            if(keyCodeAction.Priority == maxPriority)
            {
                KeyCodeActionDto codeAction = new KeyCodeActionDto();
                if(TryGetHistory(keyCodeAction.KeyCode, ref codeAction))
                {
                    keyCodeAction.DisableAction();
                    if (CurrentKeyCodeAction.ContainsKey(keyCodeAction.KeyCode))
                    {
                        CurrentKeyCodeAction[keyCodeAction.KeyCode] = codeAction;
                        Debug.Log(keyCodeAction.KeyCodeName);
                    }
                    else
                    {
                        CurrentKeyCodeAction.TryAdd(keyCodeAction.KeyCode, codeAction);
                    }
                }
                else
                {
                    KeyCodeActionDto re;
                    if(CurrentKeyCodeAction.TryRemove(keyCodeAction.KeyCode, out re))
                        re.DisableAction();
                }
            }
        }
    }

    private bool TryGetHistory(string keyCode, ref KeyCodeActionDto keyCodeAction)
    {
        if(!KeyCodeHistory.ContainsKey(keyCode) || KeyCodeHistory[keyCode].Count <= 1)
        {
            return false;
        }
        keyCodeAction = KeyCodeHistory[keyCode][KeyCodeHistory[keyCode].Count - 2];
        KeyCodeHistory[keyCode].Remove(KeyCodeHistory[keyCode][KeyCodeHistory[keyCode].Count - 1]);
        return true;
    }

    private int GetCurrentMaxPriority()
    {
        int max = 0;
        foreach(KeyCodeActionDto keyCodeAction in CurrentKeyCodeAction.Values)
        {
            if(keyCodeAction == null)
            {
                continue;
            }
            if(keyCodeAction.Priority > max)
            {
                max = keyCodeAction.Priority;
            }
        }
        return max;
    }
}
