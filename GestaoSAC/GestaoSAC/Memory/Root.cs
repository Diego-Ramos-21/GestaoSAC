using System.Collections.Generic;

namespace GestaoSAC.Memory
{
    public class Root
    {
        public bool status { get; set; }
        public int total { get; set; }
        public int page { get; set; }
        public List<Contact> list { get; set; }
    }
}
