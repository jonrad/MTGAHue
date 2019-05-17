using System.Runtime.Serialization;

namespace MTGADispatcher.ClientModels
{
    public enum ZoneType
    {
        [EnumMember(Value = "ZoneType_Battlefield")]
        Battlefield,
        [EnumMember(Value = "ZoneType_Graveyard")]
        Graveyard,
        [EnumMember(Value = "ZoneType_Hand")]
        Hand,
        [EnumMember(Value = "ZoneType_Library")]
        Library,
        [EnumMember(Value = "ZoneType_Limbo")]
        Limbo,
        [EnumMember(Value = "ZoneType_Pending")]
        Pending,
        [EnumMember(Value = "ZoneType_Revealed")]
        Revealed,
        [EnumMember(Value = "ZoneType_Stack")]
        Stack
    }
}
