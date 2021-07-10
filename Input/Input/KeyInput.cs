using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

public class KeyInput : MonoBehaviour
{
    public Dictionary<string, KeyCodeHolder> keyCodeActions
    {
        get;
        private set;
    } = new Dictionary<string, KeyCodeHolder>();
    public ConcurrentDictionary<string, KeyCodeActionDto> currentKeyCodeAction
    {
        get;
        private set;
    } = new ConcurrentDictionary<string, KeyCodeActionDto>();
    public Dictionary<string, List<KeyCodeActionDto>> keyCodeHistory
    {
        get;
        private set;
    } = new Dictionary<string, List<KeyCodeActionDto>>();

    void Update()
    {
        foreach(KeyCodeActionDto keyCodeAction in currentKeyCodeAction.Values)
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
        if (keyCodeActions.ContainsKey(keyCodeHolder.FunctionName))
        {
            return false;
        }
        keyCodeActions.Add(keyCodeHolder.FunctionName, keyCodeHolder);
        return true;
    }

    public void RemoveKeyCodeActions(KeyCodeHolder keyCodeHolder)
    {
        if(keyCodeActions.ContainsKey(keyCodeHolder.FunctionName))
        {
            keyCodeActions.Remove(keyCodeHolder.FunctionName);
        }
        foreach(string keyCode in keyCodeHolder.KeyCodeActions.Keys)
        {
            if (keyCodeHistory.ContainsKey(keyCode))
            {
                keyCodeHistory[keyCode].Remove(keyCodeHolder.KeyCodeActions[keyCode]);
            }

        }
    }
    public void ActiveKeyCodeActionHolder(string functionName)
    {
        foreach(KeyCodeActionDto keyCodeAction in keyCodeActions[functionName].KeyCodeActions.Values)
        {
            if(!currentKeyCodeAction.ContainsKey(keyCodeAction.KeyCode))
            {
                currentKeyCodeAction.TryAdd(keyCodeAction.KeyCode, keyCodeAction);
                AddKeyCodeHistory(keyCodeAction);
                continue;
            }
            if(currentKeyCodeAction[keyCodeAction.KeyCode].Priority < keyCodeAction.Priority)
            {
                currentKeyCodeAction[keyCodeAction.KeyCode].DisableAction();
                currentKeyCodeAction[keyCodeAction.KeyCode] = keyCodeAction;
                AddKeyCodeHistory(keyCodeAction);
            }
        }
    }
    private void AddKeyCodeHistory(KeyCodeActionDto keyCodeAction)
    {
        if(!keyCodeHistory.ContainsKey(keyCodeAction.KeyCode))
        {
            keyCodeHistory.Add(keyCodeAction.KeyCode, new List<KeyCodeActionDto>());
        }
        keyCodeHistory[keyCodeAction.KeyCode].Add(keyCodeAction);
    }

    public void BackHistory()
    {
        int maxPriority = GetCurrentMaxPriority();
        foreach(KeyCodeActionDto keyCodeAction in new List<KeyCodeActionDto>(currentKeyCodeAction.Values))
        {
            if(keyCodeAction.Priority == maxPriority)
            {
                KeyCodeActionDto codeAction = new KeyCodeActionDto();
                if(TryGetHistory(keyCodeAction.KeyCode, ref codeAction))
                {
                    keyCodeAction.DisableAction();
                    if (currentKeyCodeAction.ContainsKey(keyCodeAction.KeyCode))
                    {
                        currentKeyCodeAction[keyCodeAction.KeyCode] = codeAction;
                        Debug.Log(keyCodeAction.KeyCodeName);
                    }
                    else
                    {
                        currentKeyCodeAction.TryAdd(keyCodeAction.KeyCode, codeAction);
                    }
                }
                else
                {
                    KeyCodeActionDto re;
                    currentKeyCodeAction.TryRemove(keyCodeAction.KeyCode, out re);
                    re.DisableAction();
                }
            }
        }
    }

    private bool TryGetHistory(string keyCode, ref KeyCodeActionDto keyCodeAction)
    {
        if(!keyCodeHistory.ContainsKey(keyCode) || keyCodeHistory[keyCode].Count <= 1)
        {
            return false;
        }
        keyCodeAction = keyCodeHistory[keyCode][keyCodeHistory[keyCode].Count - 2];
        keyCodeHistory[keyCode].Remove(keyCodeHistory[keyCode][keyCodeHistory[keyCode].Count - 1]);
        return true;
    }

    private int GetCurrentMaxPriority()
    {
        int max = 0;
        foreach(KeyCodeActionDto keyCodeAction in currentKeyCodeAction.Values)
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
