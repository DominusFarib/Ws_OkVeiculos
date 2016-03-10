using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace webServiceCheckOk.Model
{
    [Serializable]
    public class Telefone
    {
        [XmlElement("TIPO")]
        public string Tipo { get; set; }


        [XmlElement("DDD")]
        public string DDD { get; set; }

        [XmlElement("COD_AREA")]
        public string CodArea { get; set; }

        [XmlElement("NUMERO")]
        public string Numero { get; set; }

        public Telefone(string Tipo, string DDD, string CodArea, string Numero)
        {
            this.Tipo=Tipo;
            this.CodArea = CodArea;
            this.DDD=DDD;
            this.Numero=Numero;
        }

        public Telefone(string DDD, string Numero)
        {
            this.DDD = DDD;
            this.Numero = Numero;
        }

        public Telefone(){}
    }
}