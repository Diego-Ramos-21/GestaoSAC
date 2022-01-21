using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestaoSAC.Memory
{
    class Authorization
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public int Expires_in { get; set; }
    }
}
