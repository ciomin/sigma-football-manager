using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using System.Collections.Generic;


namespace FootballManager_v0._1.Models;

public partial class News
{
    public int NewsId { get; set; }
    [Display(Name = "Administrator")]
    public int AdminId { get; set; }

    public string Post { get; set; } = null!;

    public virtual Administrator Admin { get; set; } = null!;

   
}
