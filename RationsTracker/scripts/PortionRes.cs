using Godot;

public partial class PortionRes : Resource
{
    // [Export] public string Identifier;
    [Export] public string PortionName;
    [Export] public bool Enabled;
    [Export] public int MaxValue;
    [Export] public int MinValue;
    [Export] public int Value;
    [Export] public int IntrisicValue;
    [Export] public Color PortionColor;
    [Export] public Godot.Collections.Array<string> LowerPortions;
    [Export] public Godot.Collections.Array<string> UpperPortions;

    public PortionRes()
    {
        // Identifier = $"{Convert.ToString(0,16)}";   
        PortionName = "";
        Enabled = true;
        MaxValue = 0;
        MinValue = 0;
        Value = 0;
        IntrisicValue = 0;
        PortionColor = new Color(0.5f, 0.5f, 0.5f, 1f);
        LowerPortions = [];
        UpperPortions = [];
    }

}