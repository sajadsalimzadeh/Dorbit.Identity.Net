﻿using System.Collections.Generic;

namespace Dorbit.Identity.Contracts;

public class NotificationDto
{
    public string Title { get; set; }
    public string Body { get; set; }
    public string Icon { get; set; }
    public string Url { get; set; }
    public Dictionary<string, string> Data { get; set; }
}