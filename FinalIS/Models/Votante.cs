using System;
using System.Collections.Generic;

namespace FinalIS.Models;

public partial class Votante
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Dpi { get; set; } = null!;

    //public virtual ICollection<Voto> Votos { get; set; } = new List<Voto>();
}
