using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webServiceCheckOk.Model
{
    [Serializable]
    public class EnderecoModel
    {
        public string Logradouro { get; set; }
        public string TpLogradouro { get; set; }
        public string Numero { get; set; }
        public string Cep { get; set; }
        public string Bairro { get; set; }
        public string Municipio { get; set; }
        public string UF { get; set; }
        public string Complemento { get; set; }
    }
}