using System;
using System.Collections.Generic;

namespace FinalIS.Models;

public partial class Candidato
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Partido { get; set; } = null!;

    public string? Descripcion { get; set; }

    //public virtual ICollection<Voto> Votos { get; set; } = new List<Voto>();
}
