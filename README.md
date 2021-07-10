# UnityInput
OriginalUnityInput
これは競合する同じキー名の操作について、優先順位を設けることで優先順位の高い物のみを実行するものです。
例えば、マップ内のアイテム獲得と攻撃のモーションに、JoyButton1が割り当てられている場合に
アイテム獲得の優先順位を攻撃モーションよりも高くすることでアイテム獲得のモーションのみを実行させることが可能です。

これにより、ユーザがある程度自由にキーバインドを行うことが可能になります。
※ただし、画面単位でキーバインドのグループを作っておくことを推奨します。例えば「キャラクターの通常移動」で一つのグループ「メニュー操作」で一つのグループと言ったような具合です。
これはキー名を基準に同一キーであるか確認を行っているので「Input Manager」で競合している場合は対応できません。

## 使い方例
この使い方例は開発途中のものです。予告なく変化する可能性があります。
```C#
class PlayerCharacterControl : KeyCodeHolder
{
    public float vertical;  // stores vertical input.
    public float horizontal; // stores horizontal input.
    KeyInput keyInput;
    
    void Awake()
    {
        FunctionName = "ControlManager"; //ゲーム内でユニークなキーコントロール単位名
        priority = 0; //同じキー名がkeyInputに登録されるとこの値が大きいほうが優先される。
        keyInput = GameObject.Find("Base").GetComponent<KeyInput>(); //KeyInputがAddComponentされてるObjectからKeyInputを取得
        keyInput.AddKeyCodeActions(this); //ゲームコントロール単位をkeyInputに登録
        keyInput.ActiveKeyCodeActionHolder(FunctionName); //キーコントロール名単位のキー入力を有効にします。(競合している場合はpriorityが高いほうが優先されます。)
        GetDefaultInput();
    }
    
    private void GetDefaultInput()
    {
        AddKeyCodeAction(
            "Vertical", //競合判定に用いるキー名
            "VerticalMove", //コントロールを表す名前(自由に設定することが可能です。)
            () => { vertical = Input.GetAxis("Vertical"); }, //キーが有効である場合、毎フレーム毎に呼び出されます。
            () => { vertical = 0; }); //キーが無効になった場合に1度呼び出されます。
        
        AddKeyCodeAction(
            "Horizontal",
            "HorizontalMove",
            () => { horizontal = Input.GetAxis("Horizontal"); },
            () => { horizontal = 0; });
    }
}

```
