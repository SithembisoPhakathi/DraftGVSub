using System;
using System.Collections.Generic;

namespace GeneralValuationSubs.Models;

public partial class Admin
{
    public int Id { get; set; }

    public string? SapNumberOld { get; set; }

    public string? SapNumber { get; set; }

    public string? FirstName { get; set; }

    public string? Surname { get; set; }

    public string? EmailAddress { get; set; }

    public string? Role { get; set; }

    public string? Password { get; set; }

    public string? EncryptedPassword { get; set; }

    public string? ConfirmPassword { get; set; }
}
