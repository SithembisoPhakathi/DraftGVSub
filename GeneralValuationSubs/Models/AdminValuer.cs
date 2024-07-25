using System;
using System.Collections.Generic;

namespace GeneralValuationSubs.Models;

public partial class AdminValuer
{
    public int Id { get; set; }

    public string? SapNumberOld { get; set; }

    public string? SapNumber { get; set; }

    public string? FirstName { get; set; }

    public string? SecondName { get; set; }

    public string? Surname { get; set; }

    public string? EmailAddress { get; set; }

    public string? Role { get; set; }

    public string? Sector { get; set; }

    public string? Username { get; set; }

    public string? UsernameOld { get; set; }

    public string? PhoneNo { get; set; }
}
