using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace webServiceCheckOk.Model
{
    public class UsuarioModel
    {
        public string Logon { get; set; }
        public string Senha { get; set; }
        public string CodAdmin { get; set; }
        public string Ip { get; set; }
        public Pessoa DadosPessoais { get; set; }
    }
}