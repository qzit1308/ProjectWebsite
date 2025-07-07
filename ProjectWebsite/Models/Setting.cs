using System;
using System.Collections.Generic;

namespace ProjectWebsite.Models;

public partial class Setting
{
    public string SettingKey { get; set; } = null!;

    public string SettingValue { get; set; } = null!;
}
