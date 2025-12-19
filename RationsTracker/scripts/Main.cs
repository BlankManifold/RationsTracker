using Godot;

public partial class Main : Node
{
    private PortionsSet _portionsSet;
    private OptionButton _selectionWeeklyResetDayButton;
    private OptionButton _selectionMonthlyResetDayButton;
    private OptionButton _selectionPeriodScaleButton;
    private int _currentSetIndex = 0;
    private Godot.Collections.Array<string> _setsNameList = [];
    private LineEdit _setNameLineEdit;
    private LineEdit _addNewSetLineEdit;
    private Window _addNewSetWindow;

    [Signal]
    public delegate void ResetDaySelectedEventHandler(int index);

    public override void _Ready()
    {
        Handlers.SaveLoadHandler.CreateSaveSetsDir();

        _portionsSet = GetNode<PortionsSet>("%PortionsSet");
        _setNameLineEdit = GetNode<LineEdit>("%SetNameLineEdit");
        _addNewSetLineEdit = GetNode<LineEdit>("%AddNewSetLineEdit");
        _addNewSetWindow = GetNode<Window>("%AddNewSetWindow");
        _selectionPeriodScaleButton = GetNode<OptionButton>("%SelectionPeriodScaleButton");
        _selectionWeeklyResetDayButton = GetNode<OptionButton>("%SelectionWeeklyResetDayButton");
        _selectionMonthlyResetDayButton = GetNode<OptionButton>("%SelectionMonthlyResetDayButton");

        ResetDaySelected += OnSelectedResetDaySelected;

        for (int i = 1; i <= 31; i++)
            _selectionMonthlyResetDayButton.AddItem(i.ToString());

        _setsNameList = Handlers.SaveLoadHandler.CreateSetsNameList();

        foreach (string setName in _setsNameList)
            Globals.SetsData.AddSet(Handlers.SaveLoadHandler.LoadSet(setName));

        if (_setsNameList.Count != 0)
        {
            _portionsSet.InitFromDict(_setsNameList[0]);
            _setNameLineEdit.Text = _setsNameList[0];
        }
        else
        {
            PortionsSetRes portionsSetRes = new() { SetName = "NomeSet" };
            Globals.SetsData.AddSet(portionsSetRes);
            _portionsSet.InitFromDict("NomeSet");
        }
        InitOptionButton(_portionsSet.PortionsSetRes);
    }

    #region Private methods
    private void ChangeCurrentSet(int index)
    {
        if (_setsNameList.Count == 1)
            return;

        _currentSetIndex = Mathf.PosMod(index, _setsNameList.Count);

        string nextSetName = _setsNameList[_currentSetIndex];
        PortionsSetRes portionsSetRes = Globals.SetsData.PortionsSetResDict[nextSetName];

        _setNameLineEdit.Text = portionsSetRes.SetName;
        _portionsSet.Clear();
        _portionsSet.InitFromDict(portionsSetRes.SetName);
        InitOptionButton(portionsSetRes);
    }
    private void UpdatePeriodScaleOptionButton(int periodScaleIndex)
    {
        string periodScaleText = _selectionPeriodScaleButton.GetItemText(periodScaleIndex);

        switch (periodScaleText)
        {
            case "Weekly":
                _selectionWeeklyResetDayButton.Visible = true;
                _selectionWeeklyResetDayButton.Select(0);

                _selectionMonthlyResetDayButton.Visible = false;
                break;
            case "Monthly":
                _selectionMonthlyResetDayButton.Visible = true;
                _selectionMonthlyResetDayButton.Select(0);

                _selectionWeeklyResetDayButton.Visible = false;
                break;
            default:
                break;
        }
    }
    private void UpdateResetDayOptionButton(int resetDayIndex)
    {
        string periodScaleText = _selectionPeriodScaleButton.GetItemText(_portionsSet.PortionsSetRes.PeriodScaleIndex);
        switch (periodScaleText)
        {
            case "Weekly":
                _selectionWeeklyResetDayButton.Select(resetDayIndex);
                break;
            case "Monthly":
                _selectionMonthlyResetDayButton.Select(resetDayIndex);
                break;
        }
    }
    private void InitOptionButton(PortionsSetRes portionsSetRes)
    {
        UpdatePeriodScaleOptionButton(portionsSetRes.PeriodScaleIndex);
        _selectionPeriodScaleButton.Select(portionsSetRes.PeriodScaleIndex);

        UpdateResetDayOptionButton(portionsSetRes.ResetDayIndex);
    }
    #endregion

    #region Response to signals
    public void _on_tree_exiting()
    {
        foreach (string setName in _setsNameList)
            Handlers.SaveLoadHandler.SaveSet(Globals.SetsData.PortionsSetResDict[setName]);
    }
    public void _on_selection_period_scale_button_item_selected(int index)
    {
        _portionsSet.PortionsSetRes.PeriodScaleIndex = index;
        _portionsSet.PortionsSetRes.ResetDayIndex = 0;
        UpdatePeriodScaleOptionButton(index);
    }
    public void OnSelectedResetDaySelected(int index)
    {
        _portionsSet.PortionsSetRes.ResetDayIndex = index;
    }
    public void _on_selection_weekly_reset_day_button_item_selected(int index) => EmitSignal(SignalName.ResetDaySelected, index);
    public void _on_selection_monthly_reset_day_button_item_selected(int index) => EmitSignal(SignalName.ResetDaySelected, index);
    public void _on_next_set_button_button_down()
    {
        // Handlers.SaveLoadHandler.SaveSet(Globals.SetsData.PortionsSetResDict[_setsNameList[_currentSetIndex]]);
        ChangeCurrentSet(_currentSetIndex + 1);
    }
    public void _on_set_name_line_edit_text_submitted(string setName)
    {
        string oldSetName = _setsNameList[_currentSetIndex];
        if (oldSetName == setName)
            return;

        Globals.SetsData.ChangeSetName(setName, oldSetName);
        _setsNameList[_currentSetIndex] = setName;
        _portionsSet.UpdateSetName(setName);
        _setNameLineEdit.ReleaseFocus();
    }
    public void _on_previous_set_button_button_down()
    {
        // Handlers.SaveLoadHandler.SaveSet(Globals.SetsData.PortionsSetResDict[_setsNameList[_currentSetIndex]]);
        ChangeCurrentSet(_currentSetIndex - 1);
    }
    public void _on_add_set_button_button_down()
    {
        _addNewSetWindow.Visible = true;
    }
    public void _on_remove_set_button_button_down()
    {
        if (_setsNameList.Count > 1)
        {
            string setNameToBeRemoved = _setsNameList[_currentSetIndex];
            Globals.SetsData.RemoveSet(setNameToBeRemoved);
            _setsNameList.Remove(setNameToBeRemoved);

            ChangeCurrentSet(_currentSetIndex);
        }
    }
    public void _on_confirm_button_button_down()
    {
        // Handlers.SaveLoadHandler.SaveSet(Globals.SetsData.PortionsSetResDict[_setsNameList[_currentSetIndex]]);
        string newSetName = _addNewSetLineEdit.Text;

        _setNameLineEdit.Text = newSetName;
        _setsNameList.Add(newSetName);
        _currentSetIndex = _setsNameList.Count - 1;

        PortionsSetRes portionsSetRes = new PortionsSetRes { SetName = newSetName };
        Globals.SetsData.AddSet(portionsSetRes);

        _portionsSet.Clear();
        _portionsSet.InitFromDict(newSetName);

        InitOptionButton(portionsSetRes);
        _addNewSetWindow.Visible = false;
    }
    public void _on_cancel_button_button_down()
    {
        _addNewSetWindow.Visible = false;
    }
    public void _on_portions_set_add_portion(Portion portion)
    {
        Globals.SetsData.AddPortion(_setsNameList[_currentSetIndex], portion.Info.PortionName, portion);
    }
    #endregion
}
