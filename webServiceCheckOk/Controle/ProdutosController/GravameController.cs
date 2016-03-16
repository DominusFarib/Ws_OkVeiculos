﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using webServiceCheckOk.BaseDados;
using webServiceCheckOk.Controle.Fornecedores;
using webServiceCheckOk.Controle.Inteligencia;
using webServiceCheckOk.Controle.Inteligencia.Utils;
using webServiceCheckOk.Model;
using webServiceCheckOk.Model.ProdutosModel;

namespace webServiceCheckOk.Controle.ProdutosController
{
    public class GravameController
    {
        public string logServer { get; set; }

        public GravameModel getGravame(UsuarioModel usuario, Veiculo carro, bool flagCompleto = false)
        {
            // CODIGO CONSULTA = 8
            // FORNECEDORES 2=>SEAP 3=>BOA VISTA 4=>CREDIFY
            GravameModel gravameDados = new GravameModel();
            int codFornecedor = Verificacao.getFornecedorConsulta(8);
            
            try
            {
                this.logServer = string.Empty;

                switch (codFornecedor)
                {
                    case 2:
                        this.logServer += "|SEAPE_GRAVAME";
                        FornSeape gravameSeape = new FornSeape(carro);
                        
                        if (flagCompleto)
                            gravameDados = gravameSeape.getGravameCompleto();
                        else
                            gravameDados = gravameSeape.getGravameSimples();

                        this.logServer += "_" + gravameDados.IdConsulta;
                       
                    break;
                    case 3:
                        this.logServer += "|BOAVISTA_GRAVAME";
                        FornBoaVista gravameBV = new FornBoaVista(carro);
                        gravameBV.codConsulta = "905";

                        gravameDados = gravameBV.getGravame();

                        this.logServer += "_" + gravameDados.CodFornecedor;

                    break;
                    case 4:
                        this.logServer += "|CREDIFY_GRAVAME";
                        FornCredify gravameCredify = new FornCredify(carro, usuario);

                        gravameDados = gravameCredify.getGravame();

                        this.logServer += "_" + gravameDados.CodFornecedor;
                    break;
                    case 5:
                        this.logServer += "|TDI_GRAVAME";
                        FornTdi gravameTdi = new FornTdi(carro, usuario);

                        gravameDados = gravameTdi.getGravame();

                        this.logServer += "_" + gravameTdi.idConsulta;
                    break;
                    case 6:
                        this.logServer += "|SINALIZA_GRAVAME";
                        FornSinaliza gravameSinaliza = new FornSinaliza(carro, usuario, "62");

                        gravameDados = gravameSinaliza.getGravame();

                        this.logServer += "_" + gravameDados.IdConsulta;
                    break;
                    default:
                        this.logServer += "|SINALIZA_GRAVAME";
                        FornSinaliza gravameDefault = new FornSinaliza(carro, usuario, "62");

                        gravameDados = gravameDefault.getGravame();

                        this.logServer += "_62";
                    break;
                }

                return gravameDados;
            }
            catch(Exception e)
            {
                throw e;
            }
        }
    }
}