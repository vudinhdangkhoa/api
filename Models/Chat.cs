using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Chat
{
    public int IdChat { get; set; }

    public int IdKh { get; set; }

    public string NameChat { get; set; } = null!;

    public string? Avatar { get; set; }

    public virtual KhachHang IdKhNavigation { get; set; } = null!;

    public virtual ICollection<Message> Messages { get; set; } = new List<Message>();
}
