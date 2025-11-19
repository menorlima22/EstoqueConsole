using System;
using System.Collections.Generic;
using System.Linq;
using SeuProjeto.Modelo;

namespace SeuProjeto.Servicos
{
    public class InventarioServico
    {
        public List<Produto> Produtos { get; private set; } = new();
        public List<Movimento> Movimentos { get; private set; } = new();

        // ---------------- LISTAR ----------------
        public void Listar()
        {
            if (!Produtos.Any())
            {
                Console.WriteLine("Nenhum produto cadastrado.");
                return;
            }

            Console.WriteLine("ID | Nome | Categoria | Saldo | Min");
            foreach (var p in Produtos)
                Console.WriteLine($"{p.Id} | {p.Nome} | {p.Categoria} | {p.Saldo} | {p.EstoqueMinimo}");
        }

        // ---------------- CADASTRAR ----------------
        public void Cadastrar(string nome, string categoria, int minimo, int saldo)
        {
            int id = Produtos.Any() ? Produtos.Max(p => p.Id) + 1 : 1;
            Produtos.Add(new Produto(id, nome, categoria, minimo, saldo));
            Console.WriteLine("Produto cadastrado.");
        }

        // ---------------- EDITAR ----------------
        public void Editar(int id, string nome, string categoria, int minimo)
        {
            var p = Produtos.FirstOrDefault(x => x.Id == id);
            if (p == null)
            {
                Console.WriteLine("Produto não encontrado.");
                return;
            }

            p.Nome = nome;
            p.Categoria = categoria;
            p.EstoqueMinimo = minimo;

            Console.WriteLine("Produto atualizado.");
        }

        // ---------------- REMOVER ----------------
        public void Remover(int id)
        {
            var p = Produtos.FirstOrDefault(x => x.Id == id);
            if (p == null)
            {
                Console.WriteLine("Produto não encontrado.");
                return;
            }

            if (p.Saldo < 0)
            {
                Console.WriteLine("Não é possível remover produto com saldo negativo.");
                return;
            }

            Produtos.Remove(p);
            Console.WriteLine("Produto removido.");
        }

        // ---------------- ENTRADA ----------------
        public void Entrada(int id, int qtd, string obs)
        {
            var p = Produtos.FirstOrDefault(x => x.Id == id);
            if (p == null)
            {
                Console.WriteLine("Produto não encontrado.");
                return;
            }

            p.Saldo += qtd;

            Movimentos.Add(new Movimento
            {
                Id = Movimentos.Count + 1,
                ProdutoId = id,
                Tipo = "ENTRADA",
                Quantidade = qtd,
                Data = DateTime.Now,
                Observacao = obs ?? ""
            });

            Console.WriteLine("Entrada registrada.");
        }

        // ---------------- SAÍDA ----------------
        public void Saida(int id, int qtd, string obs)
        {
            var p = Produtos.FirstOrDefault(x => x.Id == id);
            if (p == null)
            {
                Console.WriteLine("Produto não encontrado.");
                return;
            }

            if (p.Saldo < qtd)
            {
                Console.WriteLine("Saldo insuficiente.");
                return;
            }

            p.Saldo -= qtd;

            Movimentos.Add(new Movimento
            {
                Id = Movimentos.Count + 1,
                ProdutoId = id,
                Tipo = "SAIDA",
                Quantidade = qtd,
                Data = DateTime.Now,
                Observacao = obs ?? ""
            });

            Console.WriteLine("Saída registrada.");
        }

        // ---------------- RELATÓRIO: abaixo do mínimo ----------------
        public void RelatorioAbaixoDoMinimo()
        {
            var abaixo = Produtos.Where(p => p.Saldo < p.EstoqueMinimo).ToList();

            if (!abaixo.Any())
            {
                Console.WriteLine("Nenhum produto abaixo do mínimo.");
                return;
            }

            Console.WriteLine("ID | Nome | Categoria | Saldo | Min | Dif | % Abaixo");

            foreach (var p in abaixo)
            {
                int dif = p.EstoqueMinimo - p.Saldo;
                double perc = ((double)dif / p.EstoqueMinimo) * 100;

                Console.WriteLine($"{p.Id} | {p.Nome} | {p.Categoria} | {p.Saldo} | {p.EstoqueMinimo} | {dif} | {perc:F0}%");
            }
        }

        // ---------------- RELATÓRIO: extrato ----------------
        public void RelatorioExtratoPorProduto(int id)
        {
            var p = Produtos.FirstOrDefault(x => x.Id == id);
            if (p == null)
            {
                Console.WriteLine("Produto não encontrado.");
                return;
            }

            var movs = Movimentos
                .Where(m => m.ProdutoId == id)
                .OrderBy(m => m.Data)
                .ToList();

            Console.WriteLine($"Extrato do produto: {p.Nome} ({p.Categoria})");

            if (!movs.Any())
            {
                Console.WriteLine("Sem movimentos.");
                return;
            }

            int totalEntrada = movs.Where(x => x.Tipo == "ENTRADA").Sum(x => x.Quantidade);
            int totalSaida = movs.Where(x => x.Tipo == "SAIDA").Sum(x => x.Quantidade);

            foreach (var m in movs)
                Console.WriteLine($"{m.Data:dd/MM HH:mm} | {m.Tipo} | {m.Quantidade} | {m.Observacao}");

            Console.WriteLine($"\nTotal Entradas: {totalEntrada}");
            Console.WriteLine($"Total Saídas: {totalSaida}");
            Console.WriteLine($"Saldo Atual: {p.Saldo}");
        }
    }
}
