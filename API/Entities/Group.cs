using System;
using System.ComponentModel.DataAnnotations;

namespace API.Entities;

public class Group
{
    [Key]
    public required string Name { get; set; }

    public ICollection<Connection> Connections { get; set; } = [];

    public static implicit operator Task<object>(Group v)
    {
        throw new NotImplementedException();
    }
}
