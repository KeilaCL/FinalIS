using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinalIS.Models
{
    public class GeneralResult
    {
        public bool Result { get; set; }
        public string ErrorMessage { get; set; }

        public int EstadoCandidato { get; set; }
    }
}
