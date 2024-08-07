﻿using System.ComponentModel.DataAnnotations;
using MoonCore.Blazor.Bootstrap.Attributes.Auto;

namespace MoonCore.Blazor.Test.Data;

public class TestForm
{
    [Required(ErrorMessage = "Idk lmao")]
    [Section("A very important section", Icon = "bx-data")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Idk lmao email")]
    [EmailAddress]
    public string Email { get; set; }

    public int Ara { get; set; }
    [CustomComponent("CustomBool")]
    public bool A { get; set; }
}