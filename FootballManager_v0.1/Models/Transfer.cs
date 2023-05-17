using System;
using System.Collections.Generic;

namespace FootballManager_v0._1.Models;

public partial class Transfer
{
    public int TransferId { get; set; }

    public int PlayerId { get; set; }

    public int SellingUserId { get; set; }

    public int BuyingUserId { get; set; }

    public int TransferFee { get; set; }

    public DateTime? TransferDate { get; set; }

    public virtual User BuyingUser { get; set; } = null!;

    public virtual Player Player { get; set; } = null!;

    public virtual User SellingUser { get; set; } = null!;
}
