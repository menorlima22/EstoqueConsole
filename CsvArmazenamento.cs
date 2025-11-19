using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SeuProjeto.Modelo;

namespace SeuProjeto.Servicos
{
    public class CsvArmazenamento
    {
        private readonly string produtosPath = "C:\\Users\\acastagna\\source\\repos\\estoqueconsoleme\\data\\produtos.csv";
        private readonly string movPath = "C:\\Users\\acastagna\\source\\repos\\estoqueconsoleme\\data\\movimentos.csv";

        // ------------------- SALVAR -------------------
        public void Salvar(List<Produto> produtos, List<Movimento> movs)
        {
            Directory.CreateDirectory("data");

            // PRODUTOS
            string tmpProd = produtosPath + ".tmp";
            using (var sw = new StreamWriter(tmpProd, false, Encoding.UTF8))
            {
                sw.WriteLine("id;nome;categoria;estoqueMinimo;saldo");

                foreach (var p in produtos)
                    sw.WriteLine($"{p.Id};{Esc(p.Nome)};{Esc(p.Categoria)};{p.EstoqueMinimo};{p.Saldo}");
            }
            Replace(tmpProd, produtosPath);

            // MOVIMENTOS
            string tmpMov = movPath + ".tmp";
            using (var sw = new StreamWriter(tmpMov, false, Encoding.UTF8))
            {
                sw.WriteLine("id;produtoId;tipo;quantidade;data;observacao");

                foreach (var m in movs)
                    sw.WriteLine($"{m.Id};{m.ProdutoId};{m.Tipo};{m.Quantidade};{m.Data:O};{Esc(m.Observacao)}");
            }
            Replace(tmpMov, movPath);
        }

        // ------------------- ATOMIC WRITE -------------------
        private void Replace(string tmp, string final)
        {
            if (File.Exists(final)) File.Delete(final);
            File.Move(tmp, final);
        }

        // ------------------- ESCAPE CSV -------------------
        private string Esc(string s)
        {
            if (string.IsNullOrEmpty(s)) return "";

            if (s.Contains(";") || s.Contains("\""))
                return "\"" + s.Replace("\"", "\"\"") + "\"";

            return s;
        }

        // ------------------- UNESCAPE CSV -------------------
        private string Un(string s)
        {
            if (string.IsNullOrEmpty(s)) return s;

            if (s.StartsWith("\"") && s.EndsWith("\""))
                return s[1..^1].Replace("\"\"", "\"");

            return s;
        }

        // ------------------- CARREGAR -------------------
        public (List<Produto>, List<Movimento>) Carregar()
        {
            var produtos = new List<Produto>();
            var movs = new List<Movimento>();

            if (File.Exists(produtosPath))
            {
                foreach (var l in File.ReadLines(produtosPath).Skip(1))
                {
                    if (string.IsNullOrWhiteSpace(l)) continue;

                    var c = Parse(l);

                    // Suporte a CSV antigo (sem categoria)
                    string categoria = c.Count >= 3 ? Un(c[2]) : "Sem categoria";
                    int minimo = c.Count >= 4 ? int.Parse(c[3]) : 0;
                    int saldo = c.Count >= 5 ? int.Parse(c[4]) : 0;

                    produtos.Add(new Produto(
                        int.Parse(c[0]),
                        Un(c[1]),
                        categoria,
                        minimo,
                        saldo
                    ));
                }
            }

            if (File.Exists(movPath))
            {
                foreach (var l in File.ReadLines(movPath).Skip(1))
                {
                    if (string.IsNullOrWhiteSpace(l)) continue;

                    var c = Parse(l);

                    movs.Add(new Movimento
                    {
                        Id = int.Parse(c[0]),
                        ProdutoId = int.Parse(c[1]),
                        Tipo = c[2],
                        Quantidade = int.Parse(c[3]),
                        Data = DateTime.Parse(c[4]),
                        Observacao = Un(c[5])
                    });
                }
            }

            return (produtos, movs);
        }

        // ------------------- PARSE CSV -------------------
        private List<string> Parse(string line)
        {
            var result = new List<string>();
            bool inQuote = false;
            var sb = new StringBuilder();

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];

                if (c == '"')
                {
                    if (inQuote && i + 1 < line.Length && line[i + 1] == '"')
                    {
                        sb.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuote = !inQuote;
                    }
                }
                else if (c == ';' && !inQuote)
                {
                    result.Add(sb.ToString());
                    sb.Clear();
                }
                else
                {
                    sb.Append(c);
                }
            }

            result.Add(sb.ToString());
            return result;
        }
    }
}
