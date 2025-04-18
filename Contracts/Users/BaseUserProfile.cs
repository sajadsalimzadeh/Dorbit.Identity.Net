﻿using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dorbit.Identity.Contracts.Users;

public class BaseUserProfile
{
    public Gender Gender { get; set; }

    public DateTime? BirthDate { get; set; }
}