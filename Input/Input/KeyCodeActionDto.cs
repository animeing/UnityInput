using System;

public class KeyCodeActionDto
{
    public string KeyCodeName
    {
        get;
        set;
    }
    public int Priority
    {
        get;
        set;
    }
    public string KeyCode
    {
        get;
        set;
    }
    public Action Action
    {
        get;
        set;
    }
    public Action DisableAction
    {
        get;
        set;
    } = ()=>{};
}
