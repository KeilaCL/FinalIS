using System;
using System.Collections.Generic;

namespace FinalIS.Models;

public partial class Voto
{
    public int Id { get; set; }

    public int IdVotante { get; set; }
    public string ip { get; set; }

    public int? IdCandidato { get; set; }

    public DateTime Fecha { get; set; }

    public sbyte EsNulo { get; set; }

    //public virtual Candidato? IdCandidatoNavigation { get; set; }

    //public virtual Votante IdVotanteNavigation { get; set; } = null!;
}
