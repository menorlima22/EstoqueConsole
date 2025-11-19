using SeuProjeto.Servicos;
using System;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var armazenamento = new CsvArmazenamento();
var inventario = new InventarioServico();

// Carregar dados
(var produtos, var movimentos) = armazenamento.Carregar();
inventario.Produtos.AddRange(produtos);
inventario.Movimentos.AddRange(movimentos);

// Função para ler inteiro com segurança
int LerInt(string msg)
{
    int valor;
    while (true)
    {
        Console.Write(msg);
        if (int.TryParse(Console.ReadLine(), out valor))
            return valor;

        Console.WriteLine("Valor inválido. Digite um número.");
    }
}

// ---------------- MENU PRINCIPAL ----------------
while (true)
{
    Console.WriteLine("\n========== MENU =========\n");
    Console.WriteLine("1. Listar produtos");
    Console.WriteLine("2. Cadastrar produto");
    Console.WriteLine("3. Editar produto");
    Console.WriteLine("4. Excluir produto");
    Console.WriteLine("5. Entrada de estoque");
    Console.WriteLine("6. Saída de estoque");
    Console.WriteLine("7. Relatório: abaixo do mínimo");
    Console.WriteLine("8. Relatório: extrato por produto");
    Console.WriteLine("9. Salvar");
    Console.WriteLine("0. Sair");
    Console.Write("Opção: ");

    var opcao = Console.ReadLine();

    switch (opcao)
    {
        case "1":
            inventario.Listar();
            break;

        case "2":
            Console.Write("Nome: ");
            string nome = Console.ReadLine();

            Console.Write("Categoria: ");
            string categoria = Console.ReadLine();

            int minimo = LerInt("Estoque mínimo: ");
            int saldo = LerInt("Saldo inicial: ");

            inventario.Cadastrar(nome, categoria, minimo, saldo);
            break;

        case "3":
            int idEditar = LerInt("ID do produto para editar: ");

            Console.Write("Novo nome: ");
            string nNome = Console.ReadLine();

            Console.Write("Nova categoria: ");
            string nCategoria = Console.ReadLine();

            int nMinimo = LerInt("Novo estoque mínimo: ");

            inventario.Editar(idEditar, nNome, nCategoria, nMinimo);
            break;

        case "4":
            int idExcluir = LerInt("ID do produto para excluir: ");
            inventario.Remover(idExcluir);
            break;

        case "5": // ENTRADA
            int idEnt = LerInt("ID do produto (entrada): ");
            int qtdEnt = LerInt("Quantidade: ");

            Console.Write("Observação (opcional): ");
            string obsEnt = Console.ReadLine();

            inventario.Entrada(idEnt, qtdEnt, obsEnt);
            break;

        case "6": // SAÍDA
            int idSai = LerInt("ID do produto (saída): ");
            int qtdSai = LerInt("Quantidade: ");

            Console.Write("Observação (opcional): ");
            string obsSai = Console.ReadLine();

            inventario.Saida(idSai, qtdSai, obsSai);
            break;

        case "7":
            inventario.RelatorioAbaixoDoMinimo();
            break;

        case "8":
            int idRel = LerInt("ID do produto para extrato: ");
            inventario.RelatorioExtratoPorProduto(idRel);
            break;

        case "9":
            armazenamento.Salvar(inventario.Produtos, inventario.Movimentos);
            Console.WriteLine("Dados salvos com sucesso!");
            break;

        case "0":
            Console.WriteLine("Saindo...");
            return;

        default:
            Console.WriteLine("Opção inválida. Tente novamente.");
            break;
    }
}
