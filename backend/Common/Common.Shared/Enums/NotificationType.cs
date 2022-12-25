using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Shared.Enums
{
    public enum NotificationType
    {
        NotifyProgramIsComingSpinAward = 1,
        NotifyPrizeWinner = 2,
        PurchaseOrderStatus = 3,
        NotifyAdmin = 4
    }
    public enum SendNotifyType
    {
        SendAll = 1,
        IncludeUsers = 2,
        ExcludeUsers = 3
    }
}
