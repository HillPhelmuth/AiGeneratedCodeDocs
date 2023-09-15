using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AiGeneratedCodeDocs.Models;

public class UserData
{
    public UserData(string username)
    {
        UserName = username;
    }
    public string UserName { get; set; }
    public List<AppSettings> Apps { get; set; } = new();
    public string? Theme { get; set; }
}
