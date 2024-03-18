using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace RGamaFelix.TimeTracker.Domain.Model;

public abstract class AbstractEntityBase : IEntityBase
{
    public Guid Id { get; set; }
}