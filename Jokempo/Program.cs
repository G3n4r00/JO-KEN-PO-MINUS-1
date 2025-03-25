using Figgle;
using Jokempo;
using System.Text.Json;

/// <summary>
/// Gerador de números aleatórios utilizado para seleção de jogadas do computador.
/// </summary>
Random random = new Random();

/// <summary>
/// Dicionário para armazenar estatísticas dos jogadores, mapeando nome do jogador para suas estatísticas.
/// </summary>
Dictionary<string, PlayerStatistics> jogadores = new Dictionary<string, PlayerStatistics>();

#region Métodos
/// <summary>
/// Inicializa o jogo configurando a codificação da saída do console e limpando a tela.
/// </summary>
void Inicializar()
{
    Console.OutputEncoding = System.Text.Encoding.UTF8;
    LimparTela();
    CarregarEstatisticas();
}

/// <summary>
/// Limpa a tela e exibe o banner do jogo.
/// </summary>
void LimparTela()
{
    Console.BackgroundColor = ConsoleColor.Black;
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Clear();
    ExibirBanner("Jokempo -1");
}

/// <summary>
/// Exibe uma barra de progresso animada para simular o processo de descoberta do vencedor.
/// <param name="progressoMaximo">determina a duração do progresso da barra.</param>
/// </summary>
void MostrarBarraProgresso(int progressoMaximo)
{
    // Limpa a tela antes de mostrar a barra de progresso
    LimparTela();

    // Configura o tamanho da barra de progresso
    int larguraBarra = 50;  // A largura da barra de progresso
    int progressoAtual = 0; // Inicia o progresso em 0

    // Exibe a barra de progresso com a mensagem
    Console.Write("[");
    for (int i = 0; i < larguraBarra; i++)
    {
        Console.Write(" ");  // Preenche com espaços vazios
    }
    Console.Write("]");
    Console.SetCursorPosition(1, Console.CursorTop);  // Move o cursor de volta para a posição de atualização da barra

    // Atualiza a barra de progresso
    while (progressoAtual <= progressoMaximo)
    {
        // Calcula a porcentagem de progresso
        int progressoPorcentagem = (int)((progressoAtual / (double)progressoMaximo) * larguraBarra);

        // Volta o cursor para a posição inicial da barra e limpa a barra
        Console.SetCursorPosition(1, Console.CursorTop - 1); // Volta a linha para o começo da barra
        Console.Write("[");
        for (int i = 0; i < larguraBarra; i++)
        {
            if (i < progressoPorcentagem)
            {
                Console.BackgroundColor = ConsoleColor.Green; // Cor da parte preenchida
                Console.Write(" "); // Parte preenchida
            }
            else
            {
                Console.BackgroundColor = ConsoleColor.Black; // Cor do fundo
                Console.Write(" "); // Parte não preenchida
            }
        }
        Console.Write("]");

        // Reseta a cor do fundo
        Console.BackgroundColor = ConsoleColor.Black;

        // Atualiza o progresso e espera 100ms para o próximo update
        progressoAtual++;
        Thread.Sleep(100); // Aguarda 100ms para simular o progresso

        // Exibe o progresso percentual
        Console.SetCursorPosition(0, Console.CursorTop + 1);  // Move para a linha abaixo
        Console.Write($"Descobrindo o vencedor: {((double)progressoAtual / progressoMaximo) * 100:0}%");
    }

    // Finaliza a barra de progresso ao completar
    Console.SetCursorPosition(0, Console.CursorTop + 1); // Muda para a próxima linha após completar
    Console.WriteLine("Resultado Alcançado!");
}

/// <summary>
/// Limpa a linha atual no console.
/// </summary>
void LimparLinha()
{
    Console.WriteLine("");
    Console.SetCursorPosition(Console.CursorLeft, Console.CursorTop - 1);
    Console.BackgroundColor = ConsoleColor.Black;
    Console.ForegroundColor = ConsoleColor.Green;
}

/// <summary>
/// Exibe uma mensagem centralizada no console.
/// </summary>
/// <param name="mensagem">Mensagem a ser exibida.</param>
void EscreverMensagem(string mensagem)
{
    Console.SetCursorPosition((Console.WindowWidth - mensagem.Length) / 2, Console.CursorTop);
    Console.WriteLine(mensagem);
}

/// <summary>
/// Exibe um banner estilizado com a mensagem fornecida.
/// </summary>
/// <param name="mensagem">Texto do banner.</param>
void ExibirBanner(string mensagem)
{
    string banner = FiggleFonts.Larry3d.Render(mensagem);
    var linhas = banner.Split("\n");
    foreach (var linha in linhas)
    {
        if ((Console.WindowWidth - linha.Length) / 2 >= 0)
            Console.SetCursorPosition((Console.WindowWidth - linha.Length) / 2, Console.CursorTop);
        Console.WriteLine(linha);
    }
}

/// <summary>
/// Exibe um menu com opções e retorna a escolha do jogador.
/// </summary>
/// <param name="mensagem">Mensagem a ser exibida.</param>
/// <param name="opcoes">Lista de opções disponíveis.</param>
/// <returns>O caractere correspondente à escolha do usuário.</returns>
char ExibirMenu(string mensagem, params (char valor, string texto, ConsoleColor cor)[] opcoes)
{
    EscreverMensagem(mensagem);
    Console.SetCursorPosition((Console.WindowWidth - (mensagem.Length / 2)) / 2, Console.CursorTop);

    //prepara as opções do menu
    List<char> valores = new(); //Lista que armazena os caracteres associados às opções do menu
    List<string> botoes = new(); //Lista que armazena as representações textuais das opções para exibição no console

    foreach (var opcao in opcoes)
    {
        botoes.Add($" [{opcao.valor}] {opcao.texto} ");
        valores.Add(opcao.valor);
    }

    Console.SetCursorPosition((Console.WindowWidth - (string.Join("", botoes).Length)) / 2, Console.CursorTop);

    for (int i = 0; i < botoes.Count; i++)
    {
        Console.BackgroundColor = opcoes[i].cor;
        Console.Write(botoes[i]);
        Console.BackgroundColor = ConsoleColor.Black;
        Console.Write(" ");
    }

    Console.SetCursorPosition(Console.WindowWidth / 2, Console.CursorTop + 1);
    return ValidarEntrada(valores.ToArray());
}

/// <summary>
/// Valida a entrada do usuário, garantindo que ele escolha uma opção válida.
/// </summary>
/// <param name="opcoesValidas">Lista de opções válidas.</param>
/// <returns>O caractere escolhido pelo usuário.</returns>
char ValidarEntrada(params char[] opcoesValidas)
{
    char opcao = Console.ReadKey().KeyChar;
    while (!opcoesValidas.Contains(opcao))
    {
        Console.BackgroundColor = ConsoleColor.DarkRed;
        Console.ForegroundColor = ConsoleColor.White;
        EscreverMensagem("Opção inválida. Tente novamente.");
        Console.SetCursorPosition(Console.WindowWidth / 2, Console.CursorTop - 1);
        opcao = Console.ReadKey().KeyChar;
    }
    return opcao;
}

/// <summary>
/// Solicita o nome do jogador e o registra no sistema.
/// </summary>
/// <returns>Nome do jogador.</returns>
string RegistrarJogador()
{
    LimparTela();
    EscreverMensagem("Qual é o seu nome?");
    Console.SetCursorPosition(Console.WindowWidth / 2, Console.CursorTop);
    string nomeJogador = Console.ReadLine();

    // Normaliza o nome do jogador (remove espaços extras, converte para título)
    nomeJogador = NormalizarNomeJogador(nomeJogador);

    while (string.IsNullOrEmpty(nomeJogador))
    {
        Console.BackgroundColor = ConsoleColor.DarkRed;
        Console.ForegroundColor = ConsoleColor.White;
        EscreverMensagem("Você precisa digitar o seu nome. Pode ser o seu apelido...");
        Console.SetCursorPosition(Console.WindowWidth / 2, Console.CursorTop - 2);
        LimparLinha();
        Console.SetCursorPosition(Console.WindowWidth / 2, Console.CursorTop);
        nomeJogador = NormalizarNomeJogador(Console.ReadLine());
    }

    // Se o jogador não existir, cria um novo registro de estatísticas
    if (!jogadores.ContainsKey(nomeJogador))
    {
        jogadores[nomeJogador] = new PlayerStatistics();
    }

    return nomeJogador;
}

/// <summary>
/// Normaliza o nome do jogador, removendo espaços extras e formatando a capitalização.
/// </summary>
/// <param name="nome">Nome original do jogador.</param>
/// <returns>Nome do jogador formatado.</returns>
string NormalizarNomeJogador(string nome)
{
    if (string.IsNullOrWhiteSpace(nome))
        return string.Empty;

    // Remove espaços extras no início e no fim
    nome = nome.Trim();

    // Converte a primeira letra de cada palavra para maiúscula
    return string.Join(" ",
        nome.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(palavra =>
                char.ToUpper(palavra[0]) +
                palavra.Substring(1).ToLower()));
}

/// <summary>
/// Obtém as escolhas do jogador para duas mãos no jogo Jokempo.
/// </summary>
/// <returns>Uma tupla contendo os caracteres representando as duas mãos escolhidas.</returns>

(char mao1, char mao2) ObterOpcoesJogador()
{
    char mao1 = ExibirMenu("Escolha a primeira mão!",
                           ('0', "Pedra ✊", ConsoleColor.DarkGray),
                           ('1', "Papel ✋", ConsoleColor.DarkYellow),
                           ('2', "Tesoura ✌", ConsoleColor.DarkCyan));

    char mao2 = ExibirMenu("Escolha a segunda mão!",
                           ('0', "Pedra ✊", ConsoleColor.DarkGray),
                           ('1', "Papel ✋", ConsoleColor.DarkYellow),
                           ('2', "Tesoura ✌", ConsoleColor.DarkCyan));

    return (mao1, mao2); // Retorna ambas as opções em uma tupla
}

/// <summary>
/// Permite ao jogador escolher uma mão entre duas opções com um limite de tempo.
/// </summary>
/// <param name="mao1">Primeira opção de mão.</param>
/// <param name="mao2">Segunda opção de mão.</param>
/// <returns>A mão escolhida pelo jogador.</returns>
char EscolherMaoEntreDuas(char mao1, char mao2)
{
    // Cria um dicionário para mapear os valores para as descrições
    var opcoes = new Dictionary<char, (string texto, ConsoleColor cor)>
    {
        { '0', ("Pedra ✊", ConsoleColor.DarkGray) },
        { '1', ("Papel ✋", ConsoleColor.DarkYellow) },
        { '2', ("Tesoura ✌", ConsoleColor.DarkCyan) }
    };

    
    char escolhaJogador = ExibirMenu("Escolha uma mão:",
        (mao1, opcoes[mao1].texto, opcoes[mao1].cor),
        (mao2, opcoes[mao2].texto, opcoes[mao2].cor));

    return escolhaJogador;
}

/// <summary>
/// Determina a jogada final do computador com base nas jogadas do jogador e suas próprias opções.
/// </summary>
/// <param name="j1">Primeira mão do jogador.</param>
/// <param name="j2">Segunda mão do jogador.</param>
/// <param name="c1">Primeira mão do computador.</param>
/// <param name="c2">Segunda mão do computador.</param>
/// <returns>A mão escolhida pelo computador com maior probabilidade de vitória.</returns>

char jogadaComputador(char j1, char j2, char c1, char c2)
{
    var dicJogadasVencidas = new Dictionary<char, char>
    {
        { '0', '2' }, // Pedra vence Tesoura
        { '1', '0' }, // Papel vence Pedra
        { '2', '1' }  // Tesoura vence Papel
    };

    int avaliarJogada(char jogada)
    {
        int score = 0;

        if (j1 == dicJogadasVencidas[jogada] || j2 == dicJogadasVencidas[jogada])
            score += 2; // Se a jogada ganha contra alguma carta do jogador, melhor para o computador

        if (j1 == jogada || j2 == jogada)
            score += 1; // Se empata com alguma carta do jogador, menor impacto 

        return score;
    }

    int scoreC1 = avaliarJogada(c1);
    int scoreC2 = avaliarJogada(c2);

    return scoreC1 > scoreC2 ? c1 : c2; // Maior score significa maior chance de ganhar
}

/// <summary>
/// Retorna o nome correspondente à opção escolhida.
/// </summary>
/// <param name="opcao">Opção escolhida.</param>
/// <returns>Nome da opção.</returns>
string ObterNomeOpcao(char opcao)
{
    return opcao switch
    {
        '0' => "Pedra ✊",
        '1' => "Papel ✋",
        '2' => "Tesoura ✌",
        _ => ""
    };
}

/// <summary>
/// Gera as escolhas aleatórias do computador para duas mãos.
/// </summary>
/// <returns>Uma tupla contendo os caracteres representando as duas mãos do computador.</returns>

(char maocomp1, char maocomp2) EscolhaMaosComputador()
{
    // Computador selecionando suas jogadas
    List<int> opcoes = new List<int> { 0, 1, 2 };
    opcoes = opcoes.OrderBy(x => random.Next()).ToList();

    // Converte os valores int para char e retorna a tupla
    return ((char)(opcoes[0] + '0'), (char)(opcoes[1] + '0'));
}

/// <summary>
/// Exibe as jogadas escolhidas pelo jogador e pelo computador.
/// </summary>
/// <param name="jogador1">Primeira mão do jogador.</param>
/// <param name="jogador2">Segunda mão do jogador.</param>
/// <param name="comp1">Primeira mão do computador.</param>
/// <param name="comp2">Segunda mão do computador.</param>

void MostrarJogadas(char jogador1, char jogador2, char comp1, char comp2)
{
    LimparTela();

    EscreverMensagem($"Você escolheu {ObterNomeOpcao(jogador1)} e {ObterNomeOpcao(jogador2)} e Eu escolhi {ObterNomeOpcao(comp1)} e {ObterNomeOpcao(comp2)}.");
}

/// <summary>
/// Executa uma rodada do jogo Jokempo, comparando a escolha do jogador com a do computador.
/// </summary>
/// <param name="opcao">Opção final escolhida pelo jogador.</param>
/// <param name="opcaoPC">Opção final escolhida pelo computador.</param>
/// <returns>Resultado da rodada (-1 = derrota do jogador, 0 = empate, 1 = vitória do jogador).</returns>

int JogarRodada(char opcao, char opcaoPC)
{
    LimparTela();

    EscreverMensagem($"Então no final você acabou escolhendo {ObterNomeOpcao(opcao)} e eu escolhi {ObterNomeOpcao(opcaoPC)}...");
    int resultado = ValidaRodada(opcao, opcaoPC);
    Console.WriteLine("");
    Console.BackgroundColor = ConsoleColor.DarkMagenta;
    switch (resultado)
    {
        case -1:
            EscreverMensagem("Haha, eu venci! Não foi dessa vez.");
            break;
        case 0:
            EscreverMensagem("Legal! Nós empatamos!");
            break;
        case 1:
            EscreverMensagem("Parabéns! Você venceu.");
            break;
    }
    LimparLinha();
    Console.WriteLine("");
    return resultado;
}

/// <summary>
/// Valida o resultado de uma rodada comparando as jogadas do jogador e do computador.
/// </summary>
/// <param name="opcaoJogador">Opção escolhida pelo jogador.</param>
/// <param name="opcaoPC">Opção escolhida pelo computador.</param>
/// <returns>Resultado da rodada: 1 (vitória do jogador), 0 (empate), -1 (vitória do computador).</returns>

int ValidaRodada(char opcaoJogador, char opcaoPC)
{
    int resultado = opcaoJogador switch
    {
        char o when o == opcaoPC => 0,
        char o when o == '0' && opcaoPC == '2' => 1,
        char o when o == '1' && opcaoPC == '0' => 1,
        char o when o == '2' && opcaoPC == '1' => 1,
        _ => -1
    };

    return resultado;
}

/// <summary>
/// Atualiza as estatísticas do jogador após cada rodada.
/// </summary>
/// <param name="nomeJogador">Nome do jogador a ter as estatísticas atualizadas.</param>
/// <param name="resultado">Resultado da rodada (-1 = derrota, 0 = empate, 1 = vitória).</param>

void AtualizarEstatisticas(string nomeJogador, int resultado)
{
    // Se o jogador ainda não existir no dicionário, adiciona
    if (!jogadores.ContainsKey(nomeJogador))
    {
        jogadores[nomeJogador] = new PlayerStatistics();
    }

    // Atualiza as estatísticas com base no resultado
    switch (resultado)
    {
        case -1: // Derrota
            jogadores[nomeJogador].Defeats++;
            break;
        case 0: // Empate
            jogadores[nomeJogador].Draws++;
            break;
        case 1: // Vitória
            jogadores[nomeJogador].Victories++;
            break;
    }
}

/// <summary>
/// Salva as estatísticas dos jogadores em um arquivo JSON no diretório de aplicativos locais.
/// </summary>
void SalvarEstatisticas()
{
    try
    {
        string caminhoArquivo = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Jokempo",
            "pontuacoes.json"
        );

        // Cria o diretório se não existir
        Directory.CreateDirectory(Path.GetDirectoryName(caminhoArquivo));

        if (jogadores.Count > 0)
        {
            var opcoes = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            string json = JsonSerializer.Serialize(jogadores, opcoes);
            File.WriteAllText(caminhoArquivo, json, System.Text.Encoding.UTF8);

            Console.WriteLine($"Estatísticas salvas em: {caminhoArquivo}");
        }
        else
        {
            Console.WriteLine("Não há estatísticas para salvar.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao salvar as estatísticas: {ex.Message}");
    }
}

/// <summary>
/// Carrega as estatísticas dos jogadores de um arquivo JSON.
/// </summary>
void CarregarEstatisticas()
{
    try
    {
        string caminhoArquivo = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Jokempo",
            "pontuacoes.json"
        );

        if (File.Exists(caminhoArquivo))
        {
            string json = File.ReadAllText(caminhoArquivo, System.Text.Encoding.UTF8);

            if (!string.IsNullOrWhiteSpace(json))
            {
                var opcoes = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                jogadores = JsonSerializer.Deserialize<Dictionary<string, PlayerStatistics>>(json, opcoes);

                if (jogadores != null)
                {
                    EscreverMensagem("Estatísticas carregadas com sucesso!");
                    
                }
                else
                {
                    Console.WriteLine("Falha na deserialização do JSON.");
                    jogadores = new Dictionary<string, PlayerStatistics>();
                }
            }
            else
            {
                Console.WriteLine("Arquivo JSON está vazio.");
                jogadores = new Dictionary<string, PlayerStatistics>();
            }
        }
        else
        {
            jogadores = new Dictionary<string, PlayerStatistics>();
            Console.WriteLine("Arquivo de estatísticas não encontrado. Criando novo.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao carregar estatísticas: {ex.Message}");
        jogadores = new Dictionary<string, PlayerStatistics>();
    }
}

/// <summary>
/// Exibe uma lista detalhada de estatísticas de todos os jogadores cadastrados.
/// </summary>
/// <param name="continuar">Referência para a variável que controla a continuação do jogo.</param>

void ListarEstatisticasJogadores(ref char continuar)
{
    LimparTela();
    Console.BackgroundColor = ConsoleColor.DarkGray;
    EscreverMensagem("Ranking de Jogadores:");
    LimparLinha();
    Console.WriteLine("");

    // Calcula pontuação e cria a lista ordenada
    var ranking = jogadores
        .Select(j => new
        {
            Nome = j.Key,
            Jogador = j.Value,
            Pontuacao = (j.Value.Victories * 2) + j.Value.Draws - j.Value.Defeats
        })
        .OrderByDescending(x => x.Pontuacao)
        .ToList();

    EscreverMensagem("__________________________________________________________________________");
    EscreverMensagem("|  Pos  |     Jogador     |  Vitórias  |  Empates  |  Derrotas  | Pontos |");
    EscreverMensagem("|------------------------------------------------------------------------|");

    for (int i = 0; i < ranking.Count; i++)
    {
        var jogador = ranking[i];
        EscreverMensagem(
            $"|  {(i + 1).ToString().PadRight(4)} |  {jogador.Nome.PadRight(15)} |  " +
            $"{jogador.Jogador.Victories.ToString().PadRight(9)} |  " +
            $"{jogador.Jogador.Draws.ToString().PadRight(8)} |  " +
            $"{jogador.Jogador.Defeats.ToString().PadRight(9)} | " +
            $"{jogador.Pontuacao.ToString().PadRight(5)} |"
        );
    }

    EscreverMensagem("_________________________________________________________________");
    Console.WriteLine("");
    continuar = ExibirMenu("E agora? Quer iniciar uma nova partida?", ('1', "Sim", ConsoleColor.DarkBlue), ('0', "Não", ConsoleColor.Red));
}
#endregion


#region Fluxo Principal
Inicializar();

var continuar = ExibirMenu("😀 Olá! Vamos jogar Jokempo -1?", ('1', "Sim", ConsoleColor.DarkBlue), ('0', "Não", ConsoleColor.Red));


while (continuar != '0')
{
    string nomeJogador = RegistrarJogador();
    LimparTela();
    EscreverMensagem($"Bem-vindo, {nomeJogador}! Vamos começar...");

    bool jogarNovamente;
    do
    {
        (char mao1JogadorEscolhida, char mao2JogadorEscolhida) = ObterOpcoesJogador();
        (char mao1Computador, char mao2Computador) = EscolhaMaosComputador();
        MostrarJogadas(mao1JogadorEscolhida, mao2JogadorEscolhida, mao1Computador, mao2Computador);

        char maoJogadorFinal = EscolherMaoEntreDuas(mao1JogadorEscolhida, mao2JogadorEscolhida);
        char maoComputadorFinal = jogadaComputador(mao1JogadorEscolhida, mao2JogadorEscolhida, mao1Computador, mao2Computador);

        MostrarBarraProgresso(30);

        int resultado = JogarRodada(maoJogadorFinal, maoComputadorFinal);


        AtualizarEstatisticas(nomeJogador, resultado);
        jogarNovamente = ExibirMenu("Quer jogar de novo?", ('1', "Sim", ConsoleColor.DarkBlue), ('0', "Não", ConsoleColor.Red)) == '1';
        LimparTela();
    } while (jogarNovamente);

    LimparTela();
    continuar = ExibirMenu("O que deseja fazer agora?", ('1', "Continuar com outro jogador", ConsoleColor.DarkBlue), ('2', "Listar jogadores e estatísticas", ConsoleColor.DarkGray), ('0', "Sair", ConsoleColor.Red));

    if (continuar == '2')
    {
        ListarEstatisticasJogadores(ref continuar);
    }
}

LimparTela();
SalvarEstatisticas();
EscreverMensagem("👋 Tchau! Até a próxima");
#endregion