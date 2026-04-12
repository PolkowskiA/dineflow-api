using System.Runtime.Serialization;

namespace DineFlow.API.Contracts
{
    public enum OrderTypeDto
    {
        [EnumMember(Value = "dine_in")]
        DineIn,

        [EnumMember(Value = "takeaway")]
        Takeaway
    }
}