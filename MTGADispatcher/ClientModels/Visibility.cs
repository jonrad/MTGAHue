using System.Runtime.Serialization;

namespace MTGADispatcher.ClientModels
{
    public enum Visibility
    {
        [EnumMember(Value = "Visibility_Private")]
        Private,
        [EnumMember(Value = "Visibility_Public")]
        Public,
        [EnumMember(Value = "Visibility_Hidden")]
        Hidden
    }
}
