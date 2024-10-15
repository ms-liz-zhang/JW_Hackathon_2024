using Godot;
using System;
using Newtonsoft.Json;

public partial class PlayerInfo : GodotObject
{
	public String PlayerName;
	public Color Color;

	public string Serialize()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }

    public static PlayerInfo Deserialize(string json)
    {
        return JsonConvert.DeserializeObject<PlayerInfo>(json);
    }
}
