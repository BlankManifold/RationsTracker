using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PortionsSet : Control
{
    private struct MovingPortionInfo
    {
        public Portion PortionToBeMoved = null;
        public float YShift = 0f;
        public float YOnRelease = 0f;

        public MovingPortionInfo()
        {
            PortionToBeMoved = null;
            YShift = 0f;
            YOnRelease = 0f;
        }

        public readonly void SnapPosition()
        {
            PortionToBeMoved.GlobalPosition = new Vector2(PortionToBeMoved.GlobalPosition.X, YOnRelease);
        }

    }

    #region Members and signals
    private MovingPortionInfo _movingPortionInfo = new();
    private Window _popupWindow;
    private LineEdit _popupNameLineEdit;
    private SpinBox _popupTargetValueSpinBox;
    private VBoxContainer _portionsContainer;
    private HBoxContainer _listButtonsContainer;
    private PortionsSetRes _portionsSetRes;
    public PortionsSetRes PortionsSetRes
    {
        get { return _portionsSetRes; }
        set { _portionsSetRes = value; }
    }

    [Signal]
    public delegate void AddPortionEventHandler(Portion portion);

    #endregion

    #region Godot func override
    public override void _Ready()
    {
        _popupWindow = GetNode<Window>("%PopupWindow");
        _popupNameLineEdit = GetNode<LineEdit>("%NameLineEdit");
        _popupTargetValueSpinBox = GetNode<SpinBox>("%TargetValueSpinBox");
        _portionsContainer = GetNode<VBoxContainer>("%PortionsContainer");

        SetPhysicsProcess(false);
    }
    public override void _PhysicsProcess(double delta)
    {
        float newY = GetGlobalMousePosition().Y - _movingPortionInfo.YShift;
        _movingPortionInfo.PortionToBeMoved.GlobalPosition = new Vector2(_movingPortionInfo.PortionToBeMoved.GlobalPosition.X, newY);

        if (_CheckPortionMovedUp(_movingPortionInfo.PortionToBeMoved))
        {
            int index = _movingPortionInfo.PortionToBeMoved.GetIndex();
            _movingPortionInfo.YOnRelease = _portionsContainer.GetChild<Portion>(index - 1).GlobalPosition.Y;
            _portionsContainer.MoveChild(_movingPortionInfo.PortionToBeMoved, index - 1);
            return;
        }
        if (_CheckPortionMovedDown(_movingPortionInfo.PortionToBeMoved))
        {
            int index = _movingPortionInfo.PortionToBeMoved.GetIndex();
            _movingPortionInfo.YOnRelease = _portionsContainer.GetChild<Portion>(index + 1).GlobalPosition.Y;
            _portionsContainer.MoveChild(_movingPortionInfo.PortionToBeMoved, index + 1);
            return;
        }
    }
    #endregion

    #region Private methods
    private bool _CheckPortionMovedUp(Portion portion)
    {
        int index = portion.GetIndex();
        if (index == 0 || _portionsContainer.GetChildCount() < 2)
            return false;

        float previousY = _portionsContainer.GetChild<Portion>(index - 1).GlobalPosition.Y;
        return previousY > portion.GlobalPosition.Y;
    }
    private bool _CheckPortionMovedDown(Portion portion)
    {
        int index = portion.GetIndex();
        if (index == _portionsContainer.GetChildCount() - 1 || _portionsContainer.GetChildCount() < 2)
            return false;

        float nextY = _portionsContainer.GetChild<Portion>(index + 1).GlobalPosition.Y;
        return nextY < portion.GlobalPosition.Y;
    }
    private void _CreatePortion(PortionRes portionRes = null)
    {
        portionRes ??= new PortionRes();

        portionRes.MaxValue = (int)_popupTargetValueSpinBox.Value;
        portionRes.PortionName = _popupNameLineEdit.Text;

        Portion portion = Globals.PackedScenes.Portion.Instantiate<Portion>();

        portion.MoveButtonChanged += OnPortionMoveButtonChanged;
        // portion.PortionNameChanged += OnPortionNameChanged;
        portion.Init(_portionsSetRes.SetName, portionRes);
        EmitSignal(SignalName.AddPortion, portion);

        _portionsContainer.AddChild(portion);
        GetTree().CallGroup(
            $"portions_{_portionsSetRes.SetName}",
            Portion.MethodName.AddSelectionCheckBox,
            [portionRes.PortionName]
        );
    }
    #endregion

    #region Public methods
    public void Init(PortionsSetRes res)
    {
        _portionsSetRes = res;

        foreach (PortionRes portionRes in _portionsSetRes.PortionsResList)
        {
            Portion portion = new();
            portion.MoveButtonChanged += OnPortionMoveButtonChanged;
            // portion.PortionNameChanged += OnPortionNameChanged;

            portion.Init(_portionsSetRes.SetName, portionRes);
            EmitSignal(SignalName.AddPortion, portion);
        }

        foreach (KeyValuePair<string, Portion> item in Globals.SetsData.GetPortionsDict(_portionsSetRes.SetName))
        {
            _portionsContainer.AddChild(item.Value);
        }
    }
    public void InitFromDict(string setName)
    {
        _portionsSetRes = Globals.SetsData.PortionsSetResDict[setName];

        foreach (Portion portion in Globals.SetsData.GetPortions(_portionsSetRes.SetName))
        {
            portion.Init(_portionsSetRes.SetName, portion.Info);
            _portionsContainer.AddChild(portion);
        }
    }
    public void Clear()
    {
        _portionsSetRes = null;

        foreach (Portion portion in _portionsContainer.GetChildren().Cast<Portion>())
            _portionsContainer.RemoveChild(portion);
    }
    public void UpdateSetName(string setName)
    {
        string oldSetName = _portionsSetRes.SetName;
        _portionsSetRes.SetName = setName;

        foreach (Portion portion in Globals.SetsData.GetPortions(_portionsSetRes.SetName))
        {
            portion.SetName = setName;
            portion.RemoveFromGroup($"portions_{oldSetName}");
            portion.AddToGroup($"portions_{setName}");
        }

        foreach (KeyValuePair<string, Portion> item in Globals.SetsData.GetPortionsDict(setName))
            item.Value.SetName = setName;
    }
    #endregion

    #region Response to signals
    public void _on_add_portion_button_button_down()
    {
        _popupWindow.Show();
        _popupNameLineEdit.GrabFocus();
    }
    public void _on_popup_window_close_requested()
    {
        _popupWindow.Hide();
    }
    public void _on_add_button_button_down()
    {
        _CreatePortion();
        _popupWindow.Hide();
    }
    public void _on_delete_all_portion_button_button_down()
    {
        foreach (KeyValuePair<string, Portion> item in Globals.SetsData.GetPortionsDict(_portionsSetRes.SetName))
        {
            Portion portion = item.Value;
            _portionsContainer.RemoveChild(portion);
            Globals.SetsData.RemovePortion(_portionsSetRes.SetName, portion.Info.PortionName, portion);
            portion.QueueFree();
        }
    }
    public void _on_reset_all_portion_button_button_down()
    {
        foreach (KeyValuePair<string, Portion> item in Globals.SetsData.GetPortionsDict(_portionsSetRes.SetName))
        {
            Portion portion = item.Value;
            portion.SubstractValueToProgressBar(portion.Info.IntrisicValue);
            portion.Info.IntrisicValue = 0;
        }
    }
    public void OnPortionMoveButtonChanged(Portion portion, bool down)
    {
        if (down)
        {
            SetPhysicsProcess(true);
            _movingPortionInfo.PortionToBeMoved = portion;
            _movingPortionInfo.YShift = GetGlobalMousePosition().Y - portion.GlobalPosition.Y;
            _movingPortionInfo.YOnRelease = portion.GlobalPosition.Y;
            return;
        }

        SetPhysicsProcess(false);
        _movingPortionInfo.SnapPosition();
        _movingPortionInfo.PortionToBeMoved = null;
    }

    // public void OnPortionNameChanged(string newName)
    // {

    // }
    #endregion
}
