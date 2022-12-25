using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Common.Shared.Enums.MiliGame
{
    public enum LotStatus
    {
        [Description("Kích hoạt")]
        Active = 1,
        [Description("Ngưng kích hoạt")]
        InActive = 2,
    }
    public enum WarehouseStatus
    {
        [Description("Tạo mới")]
        IsCreate = 1,
        [Description("Đã tham gia")]
        Joined = 2,
        [Description("Chưa tham gia")]
        NotParticipate = 3
    }
    public enum ProgrameStatus
    {
        Active = 1,
        InActive = 2,
        Cancel = 3,
        IsAward = 4,
    }
    public enum RefundCode
    {
        IsCompleted = 1,
        IsNotComplete = 2
    }
}
