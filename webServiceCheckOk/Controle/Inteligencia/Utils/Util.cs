using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace webServiceCheckOk.Controle.Inteligencia.Utils
{
    public class Util
    {
        // ANULA/DESTROI ATRIBUTOS STRING VAZIOS PARA QUALQUER TIPO OBJETO
        public static T unSetDadosVazios<T>(T objeto)
        {

            Type tipo = objeto.GetType();
            PropertyInfo[] propriedades = tipo.GetProperties();
            foreach (PropertyInfo atributo in propriedades)
            {
                if (atributo.GetType().Name.GetType() == typeof(String))
                    if (atributo.GetValue(objeto, null) == String.Empty)
                        atributo.SetValue(objeto, null);
            }
            
            return objeto;
        }

        public static List<T> unSetListaVazia <T>(List<T> objeto)
        {

            Type tipo = objeto.GetType();
            PropertyInfo[] propriedades = tipo.GetProperties();

            foreach (PropertyInfo atributo in propriedades)
            {
                Type tipo2 = atributo.GetType();
                PropertyInfo[] propriedades2 = tipo2.GetProperties();
                
                foreach (PropertyInfo atributo2 in propriedades2)
                {
                    if (atributo2.GetType().Name.GetType() == typeof(String))
                        if (atributo2.GetValue(atributo, null) == String.Empty)
                            atributo2.SetValue(atributo, null);
                }
            }

            return objeto;
        }

    }
}