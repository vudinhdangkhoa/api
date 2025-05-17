using System;
using System.Collections.Generic;

namespace api.Models;

public partial class Message
{
    public int IdMess { get; set; }

    public string NguoiGui { get; set; } = null!;

    public string? NoiDung { get; set; }

    public int? IdChat { get; set; }

    public virtual Chat? IdChatNavigation { get; set; }
}
