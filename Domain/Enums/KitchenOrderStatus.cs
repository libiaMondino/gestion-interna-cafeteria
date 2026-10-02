using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Enums
{
    public enum KitchenOrderStatus
    {
        Pending = 1,
        Sent = 2,
        InPreparation = 3,
        Ready = 4,
        Delivered = 5,
        Cancelled = 6
    }
}
