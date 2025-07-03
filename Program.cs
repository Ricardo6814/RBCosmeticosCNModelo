using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

class Produto
{
    public string Nome { get; set; }
    public int Quantidade { get; set; }
    public double ValorUnitario { get; set; }

    public double ValorTotal => Quantidade * ValorUnitario;
}

class Venda
{
    public string Produto { get; set; }
    public int Quantidade { get; set; }
    public double ValorUnitario { get; set; }
    public DateTime Data { get; set; }

    public double ValorTotal => Quantidade * ValorUnitario;
}

class Program
{
    static List<Produto> estoque = new List<Produto>();
    static List<Venda> vendas = new List<Venda>();

    static string caminhoEstoque = "estoque.txt";
    static string caminhoVendas = "vendas.txt";

    static void Main()
    {
        CarregarEstoque();
        CarregarVendas();

        while (true)
        {
            Console.Clear();
            Console.WriteLine("RB COSMÉTICOS - ESTOQUE");
            Console.WriteLine("================================");
            Console.WriteLine("1 - Adicionar Produto");
            Console.WriteLine("2 - Entrada de Estoque");
            Console.WriteLine("3 - Saída (Venda)");
            Console.WriteLine("4 - Visualizar Estoque");
            Console.WriteLine("5 - Gerar Relatório de Vendas (TXT)");
            Console.WriteLine("6 - Salvar e Sair");
            Console.WriteLine("7 - Gerar Relatório de Estoque (TXT)");
            Console.WriteLine("8 - Gerar Relatório de Vendas (TXT)");
            Console.WriteLine("================================");
            Console.Write("Escolha uma opção: ");
            string opcao = Console.ReadLine();

            switch (opcao)
            {
                case "1": AdicionarProduto(); break;
                case "2": EntradaEstoque(); break;
                case "3": SaidaEstoque(); break;
                case "4": ListarEstoque(); break;
                case "5": GerarRelatorioVendas(); break;
                case "6":
                    SalvarEstoque();
                    SalvarVendas();
                    Console.WriteLine("Tudo salvo. Até logo!");
                    return;
                case "7": GerarRelatorioEstoque(); break;
                case "8": GerarRelatorioVendas(); break;
                default: Console.WriteLine("Opção inválida."); break;
            }

            Console.WriteLine("\nPressione ENTER para continuar...");
            Console.ReadLine();
        }
    }

    static void AdicionarProduto()
    {
        Console.Write("Nome do produto: ");
        string nome = Console.ReadLine();

        if (BuscarProduto(nome) != null)
        {
            Console.WriteLine("Produto já existe! Use 'Entrada' para adicionar mais.");
            return;
        }

        Console.Write("Quantidade: ");
        if (!int.TryParse(Console.ReadLine(), out int quantidade) || quantidade <= 0)
        {
            Console.WriteLine("Quantidade inválida!");
            return;
        }

        Console.Write("Valor unitário: ");
        if (!double.TryParse(Console.ReadLine(), NumberStyles.Any, CultureInfo.InvariantCulture, out double valorUnitario))
        {
            Console.WriteLine("Valor inválido!");
            return;
        }

        estoque.Add(new Produto { Nome = nome, Quantidade = quantidade, ValorUnitario = valorUnitario });
        Console.WriteLine("Produto cadastrado com sucesso!");
    }

    static void EntradaEstoque()
    {
        Console.Write("Nome do produto: ");
        string nome = Console.ReadLine();

        Produto produto = BuscarProduto(nome);
        if (produto == null)
        {
            Console.WriteLine("Produto não encontrado.");
            return;
        }

        Console.Write("Quantidade a adicionar: ");
        if (!int.TryParse(Console.ReadLine(), out int entrada) || entrada <= 0)
        {
            Console.WriteLine("Quantidade inválida!");
            return;
        }

        produto.Quantidade += entrada;
        Console.WriteLine("Entrada registrada!");
    }

    static void SaidaEstoque()
    {
        Console.Write("Nome do produto: ");
        string nome = Console.ReadLine();

        Produto produto = BuscarProduto(nome);
        if (produto == null)
        {
            Console.WriteLine("Produto não encontrado.");
            return;
        }

        Console.Write("Quantidade a remover (venda): ");
        if (!int.TryParse(Console.ReadLine(), out int saida) || saida <= 0)
        {
            Console.WriteLine("Quantidade inválida!");
            return;
        }

        if (saida > produto.Quantidade)
        {
            Console.WriteLine("Estoque insuficiente!");
            return;
        }

        produto.Quantidade -= saida;

        vendas.Add(new Venda
        {
            Produto = produto.Nome,
            Quantidade = saida,
            ValorUnitario = produto.ValorUnitario,
            Data = DateTime.Now
        });

        Console.WriteLine("Venda registrada com sucesso!");
    }

    static void ListarEstoque()
    {
        Console.Clear();
        Console.WriteLine("ESTOQUE ATUAL\n");

        if (estoque.Count == 0)
        {
            Console.WriteLine("Nenhum produto cadastrado.");
            return;
        }

        foreach (var produto in estoque)
        {
            Console.WriteLine("================================");
            Console.WriteLine($"Produto: {produto.Nome}");
            Console.WriteLine($"Quantidade: {produto.Quantidade}");
            Console.WriteLine($"Valor Unitário: R$ {produto.ValorUnitario.ToString("F2", CultureInfo.InvariantCulture)}");
            Console.WriteLine($"Valor Total: R$ {produto.ValorTotal.ToString("F2", CultureInfo.InvariantCulture)}");
        }

        Console.WriteLine("================================\n");
    }

    static void GerarRelatorioEstoque()
    {
        string caminho = "relatorio_estoque.txt";

        using (StreamWriter writer = new StreamWriter(caminho))
        {
            writer.WriteLine("RELATÓRIO DE ESTOQUE - RB COSMÉTICOS");
            writer.WriteLine($"Emitido em: {DateTime.Now:dd/MM/yyyy HH:mm}");
            writer.WriteLine();

            if (estoque.Count == 0)
            {
                writer.WriteLine("Nenhum produto no estoque.");
            }
            else
            {
                foreach (var p in estoque)
                {
                    writer.WriteLine($"Produto: {p.Nome}");
                    writer.WriteLine($"Quantidade: {p.Quantidade}");
                    writer.WriteLine($"Valor Unitário: R$ {p.ValorUnitario.ToString("F2", CultureInfo.InvariantCulture)}");
                    writer.WriteLine($"Valor Total: R$ {p.ValorTotal.ToString("F2", CultureInfo.InvariantCulture)}");
                    writer.WriteLine("----------------------------------------");
                }
            }
        }

        Console.WriteLine($"Relatório de estoque gerado: {Path.GetFullPath(caminho)}");
    }

    static void GerarRelatorioVendas()
    {
        string caminho = "relatorio_vendas.txt";

        using (StreamWriter writer = new StreamWriter(caminho))
        {
            writer.WriteLine("RELATÓRIO DE VENDAS - RB COSMÉTICOS");
            writer.WriteLine($"Emitido em: {DateTime.Now:dd/MM/yyyy HH:mm}");
            writer.WriteLine();

            if (vendas.Count == 0)
            {
                writer.WriteLine("Nenhuma venda registrada.");
            }
            else
            {
                double totalGeral = 0;

                foreach (var v in vendas)
                {
                    writer.WriteLine($"Produto: {v.Produto}");
                    writer.WriteLine($"Quantidade: {v.Quantidade}");
                    writer.WriteLine($"Valor Unitário: R$ {v.ValorUnitario.ToString("F2", CultureInfo.InvariantCulture)}");
                    writer.WriteLine($"Data da Venda: {v.Data:dd/MM/yyyy HH:mm}");
                    writer.WriteLine($"Valor Total: R$ {v.ValorTotal.ToString("F2", CultureInfo.InvariantCulture)}");
                    writer.WriteLine("----------------------------------------");
                    totalGeral += v.ValorTotal;
                }

                writer.WriteLine($"TOTAL GERAL VENDIDO: R$ {totalGeral.ToString("F2", CultureInfo.InvariantCulture)}");
            }
        }

        Console.WriteLine($"Relatório de vendas gerado: {Path.GetFullPath(caminho)}");
    }

    static Produto BuscarProduto(string nome)
    {
        return estoque.Find(p => p.Nome.Equals(nome, StringComparison.OrdinalIgnoreCase));
    }

    static void CarregarEstoque()
    {
        if (!File.Exists(caminhoEstoque)) return;

        string[] linhas = File.ReadAllLines(caminhoEstoque);
        foreach (string linha in linhas)
        {
            string[] partes = linha.Split(';');
            if (partes.Length == 3)
            {
                try
                {
                    Produto produto = new Produto
                    {
                        Nome = partes[0],
                        Quantidade = int.Parse(partes[1]),
                        ValorUnitario = double.Parse(partes[2], CultureInfo.InvariantCulture)
                    };
                    estoque.Add(produto);
                }
                catch
                {
                    Console.WriteLine("Erro ao carregar um produto do estoque.");
                }
            }
        }
    }

    static void SalvarEstoque()
    {
        using (StreamWriter writer = new StreamWriter(caminhoEstoque))
        {
            foreach (var produto in estoque)
            {
                writer.WriteLine($"{produto.Nome};{produto.Quantidade};{produto.ValorUnitario.ToString(CultureInfo.InvariantCulture)}");
            }
        }
    }

    static void CarregarVendas()
    {
        if (!File.Exists(caminhoVendas)) return;

        string[] linhas = File.ReadAllLines(caminhoVendas);
        foreach (string linha in linhas)
        {
            string[] partes = linha.Split(';');
            if (partes.Length == 4)
            {
                try
                {
                    vendas.Add(new Venda
                    {
                        Produto = partes[0],
                        Quantidade = int.Parse(partes[1]),
                        ValorUnitario = double.Parse(partes[2], CultureInfo.InvariantCulture),
                        Data = DateTime.ParseExact(partes[3], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture)
                    });
                }
                catch
                {
                    Console.WriteLine("Erro ao carregar uma venda.");
                }
            }
        }
    }

    static void SalvarVendas()
    {
        using (StreamWriter writer = new StreamWriter(caminhoVendas))
        {
            foreach (var venda in vendas)
            {
                writer.WriteLine($"{venda.Produto};{venda.Quantidade};{venda.ValorUnitario.ToString(CultureInfo.InvariantCulture)};{venda.Data:yyyy-MM-dd HH:mm:ss}");
            }
        }
    }
}
