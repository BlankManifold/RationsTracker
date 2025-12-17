using Godot;
using System.Linq;
using System.Collections.Generic;


public partial class PortionOptionsBox : VBoxContainer
{
    private GridContainer _childrenCheckBoxesContainer;
    private ColorPicker _colorPicker;
    private ColorPickerButton _colorPickerButton;
    private Godot.Collections.Dictionary<string, CheckBox> _checkBoxesDict = [];
    private Godot.Collections.Dictionary<string, bool> _initialCheckBoxesNameDict = [];
    private string _setName = "";

    [Signal]
    public delegate void ConfirmedChangesEventHandler(Godot.Collections.Array<string> checkedChildren);
    [Signal]
    public delegate void DeletePortionEventHandler();
    [Signal]
    public delegate void DisablePortionEventHandler(bool disable);
    [Signal]
    public delegate void ColorChangedEventHandler(Color color);

    public void Init(Godot.Collections.Array<string> portionTypes)
    {
        foreach (string type in portionTypes)
        {
            CheckBox checkBox = new CheckBox
            {
                ButtonPressed = false,
                Text = type
            };

            _checkBoxesDict.Add(type, checkBox);
        }

        foreach (CheckBox checkBox in _checkBoxesDict.Values)
            _childrenCheckBoxesContainer.AddChild(checkBox);

    }
    public override void _Ready()
    {
        _childrenCheckBoxesContainer = GetNode<GridContainer>("%ChildrenCheckBoxesContainer");
        _colorPickerButton = GetNode<ColorPickerButton>("%ColorPickerButton");
        _colorPicker = _colorPickerButton.GetPicker();
        _colorPicker.DeferredMode = true;
        _colorPicker.ColorModesVisible = false;
        _colorPicker.SlidersVisible = false;
        _colorPicker.HexVisible = false;
    }

    public void Disable(string name, bool disable = true)
    {
        _checkBoxesDict[name].Disabled = disable;
        _checkBoxesDict[name].ButtonPressed = false;
    }
    public void Clear()
    {
        foreach (CheckBox checkBox in _checkBoxesDict.Values)
            checkBox.ButtonPressed = false;
    }
    public void UpdateCheckBoxes(
        Godot.Collections.Array<string> lowerTypes,
        Godot.Collections.Array<string> upperTypes
    )
    {
        foreach (string type in lowerTypes)
        {
            _checkBoxesDict[type].ButtonPressed = true;
            _initialCheckBoxesNameDict[type] = true;
        }

        foreach (string type in upperTypes)
            Disable(type);
    }
    private void _UpdateInitialDict()
    {
        foreach (KeyValuePair<string, CheckBox> item in _checkBoxesDict)
        {
            _initialCheckBoxesNameDict[item.Key] = item.Value.ButtonPressed;
        }
    }
    public void Reset()
    {
        foreach (KeyValuePair<string, bool> item in _initialCheckBoxesNameDict)
        {
            _checkBoxesDict[item.Key].ButtonPressed = item.Value;
        }
    }
    public void AddCheckBox(string type)
    {
        if (_checkBoxesDict.ContainsKey(type))
            return;

        CheckBox checkBox = new()
        {
            ButtonPressed = false,
            Text = type
        };

        _checkBoxesDict.Add(type, checkBox);
        _childrenCheckBoxesContainer.AddChild(checkBox);
    }
    public void RemoveCheckBox(string type)
    {
        CheckBox checkBox = _checkBoxesDict[type];
        _checkBoxesDict.Remove(type);
        _childrenCheckBoxesContainer.RemoveChild(checkBox);
        checkBox.QueueFree();
    }
    public Godot.Collections.Array<string> GetCheckedPortionTypes()
    {
        return [.. _checkBoxesDict.Where(x => x.Value.ButtonPressed).Select(x => x.Key)];
    }
    public void UpdateCheckBoxName(string oldName, string newName)
    {
        _checkBoxesDict[newName] = _checkBoxesDict[oldName];
        _checkBoxesDict.Remove(oldName);
        _checkBoxesDict[newName].Text = newName;
    }
    public void UpdateColorPickerColor(Color color)
    {
        _colorPicker.Color = color;
        _colorPickerButton.Color = color;
    }


    public void _on_confirm_button_button_down()
    {
        Visible = false;
        _UpdateInitialDict();
        EmitSignal(SignalName.ConfirmedChanges, GetCheckedPortionTypes());
    }
    public void _on_esc_button_button_down()
    {
        Visible = false;
        Reset();
    }
    public void _on_delete_button_button_down()
    {
        Visible = false;
        EmitSignal(SignalName.DeletePortion);
    }
    public void _on_disable_button_toggled(bool pressed)
    {
        EmitSignal(SignalName.DisablePortion, pressed);
    }

    public void _on_color_picker_button_color_changed(Color color)
    {
        EmitSignal(SignalName.ColorChanged, color);
    }
}
