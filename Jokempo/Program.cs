using Figgle;
using System.Text.Json;

//Criação da função para gerar números aleatórios
Random random = new Random();

//Função para salvar a pontuação

// Dicionário para armazenar estatísticas dos jogadores (nome -> (vitórias, empates, derrotas))
Dictionary<string, (int vitorias, int empates, int derrotas)> jogadores = new Dictionary<string, (int, int, int)>();

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

    while (string.IsNullOrEmpty(nomeJogador))
    {
        Console.BackgroundColor = ConsoleColor.DarkRed;
        Console.ForegroundColor = ConsoleColor.White;
        EscreverMensagem("Você precisa digitar o seu nome. Pode ser o seu apelido...");
        Console.SetCursorPosition(Console.WindowWidth / 2, Console.CursorTop - 2);
        LimparLinha();
        Console.SetCursorPosition(Console.WindowWidth / 2, Console.CursorTop);
        nomeJogador = Console.ReadLine();
    }

    if (!jogadores.ContainsKey(nomeJogador))
    {
        jogadores[nomeJogador] = (0, 0, 0);
    }

    return nomeJogador;
}

/// <summary>
/// Obtém a escolha do jogador.
/// </summary>
/// <returns>O caractere representando a escolha do jogador.</returns>
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

char EscolherMaoEntreDuas(char mao1, char mao2)
{
    // Cria um dicionário para mapear os valores para as descrições
    var opcoes = new Dictionary<char, (string texto, ConsoleColor cor)>
    {
        { '0', ("Pedra ✊", ConsoleColor.DarkGray) },
        { '1', ("Papel ✋", ConsoleColor.DarkYellow) },
        { '2', ("Tesoura ✌", ConsoleColor.DarkCyan) }
    };

    // Tempo máximo para o jogador fazer a escolha (em segundos)
    int tempoLimite = 10;

    // Variável para armazenar o valor da escolha do jogador
    char escolhaJogador = '\0'; // Inicia com valor vazio (caso o tempo acabe sem escolha)

    // Exibe o menu com as duas opções escolhidas
    Task.Run(() =>
    {
        // Exibe o texto inicial
        EscreverMensagem($"Escolha uma mão! Tempo restante: {tempoLimite}s");

        // Contador de tempo
        for (int i = tempoLimite; i >= 0; i--)
        {
            // Limpa a linha do contador e atualiza o número de segundos
            Console.SetCursorPosition(0, Console.CursorTop);  // Volta o cursor para o começo da linha
            Console.Write(new string(' ', Console.WindowWidth)); // Limpa a linha
            Console.SetCursorPosition(0, Console.CursorTop);  // Volta o cursor para o começo da linha novamente
            EscreverMensagem($"Escolha uma mão! Tempo restante: {i}s");

            // Atualiza o contador a cada segundo
            Thread.Sleep(1000);

            // Se já houver uma escolha, sai do loop
            if (escolhaJogador != '\0') break;
        }

        // Se não houver escolha até o final do tempo, retorna uma escolha padrão (por exemplo, '0')
        if (escolhaJogador == '\0')
        {
            escolhaJogador = mao1;  // Ou alguma lógica para escolha padrão
        }
    });

    // Espera a escolha do jogador (até o tempo limite)
    escolhaJogador = ExibirMenu("Escolha uma mão:",
        (mao1, opcoes[mao1].texto, opcoes[mao1].cor),
        (mao2, opcoes[mao2].texto, opcoes[mao2].cor));

    return escolhaJogador;
}

//Função para decidir a jogada final do computador com base nas jogadas do jogador
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
/// <param name="opcao">Opção escolhida pelo jogador.</param>
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
/// Executa uma rodada do jogo Jokempo.
/// O jogador escolhe uma opção, o computador escolhe aleatoriamente outra, e o resultado é avaliado.
/// </summary>
/// <param name="opcao">Opção escolhida pelo jogador (0 = Pedra, 1 = Papel, 2 = Tesoura)</param>
/// <returns>Retorna 1 se o jogador venceu, 0 se houve empate e -1 se o computador venceu.</returns>
/// 

(char maocomp1, char maocomp2) EscolhaMaosComputador()
{
    // Computador selecionando suas jogadas
    List<int> opcoes = new List<int> { 0, 1, 2 };
    opcoes = opcoes.OrderBy(x => random.Next()).ToList();

    // Converte os valores int para char e retorna a tupla
    return ((char)(opcoes[0] + '0'), (char)(opcoes[1] + '0'));
}

void MostrarJogadas(char jogador1, char jogador2, char comp1, char comp2)
{
    LimparTela();

    EscreverMensagem($"Você escolheu {ObterNomeOpcao(jogador1)} e {ObterNomeOpcao(jogador2)} e Eu escolhi {ObterNomeOpcao(comp1)} e {ObterNomeOpcao(comp2)}.");
}
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
/// Avalia o resultado de uma rodada com base nas escolhas do jogador e do computador.
/// </summary>
/// <param name="opcaoJogador">Opção escolhida pelo jogador.</param>
/// <param name="opcaoPC">Opção escolhida pelo computador.</param>
/// <returns>Retorna 1 se o jogador venceu, 0 se houve empate e -1 se o computador venceu.</returns>
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
/// Atualiza as estatísticas do jogador com base no resultado da rodada.
/// </summary>
/// <param name="nomeJogador">Nome do jogador.</param>
/// <param name="resultado">Resultado da rodada (-1 = derrota, 0 = empate, 1 = vitória).</param>
void AtualizarEstatisticas(string nomeJogador, int resultado)
{
    // Se o jogador não existe, cria uma entrada no dicionário com valores iniciais
    if (!jogadores.ContainsKey(nomeJogador))
    {
        jogadores[nomeJogador] = (0, 0, 0); // Inicializa com 0 vitórias, 0 empates e 0 derrotas
    }

    // Atualiza as estatísticas com base no resultado
    switch (resultado)
    {
        case -1: // Derrota
            jogadores[nomeJogador] = (jogadores[nomeJogador].vitorias, jogadores[nomeJogador].empates, jogadores[nomeJogador].derrotas + 1);
            break;
        case 0: // Empate
            jogadores[nomeJogador] = (jogadores[nomeJogador].vitorias, jogadores[nomeJogador].empates + 1, jogadores[nomeJogador].derrotas);
            break;
        case 1: // Vitória
            jogadores[nomeJogador] = (jogadores[nomeJogador].vitorias + 1, jogadores[nomeJogador].empates, jogadores[nomeJogador].derrotas);
            break;
    }

    // Salva as estatísticas após a atualização
    SalvarEstatisticas();
}

// Função para salvar o dicionário de jogadores no arquivo JSON
void SalvarEstatisticas()
{
    try
    {
        string json = JsonSerializer.Serialize(jogadores, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText("pontuacoes.json", json); // Salva no arquivo JSON
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao salvar as estatísticas: {ex.Message}");
    }
}

void CarregarEstatisticas()
{
    try
    {
        if (File.Exists("pontuacoes.json"))
        {
            string json = File.ReadAllText("pontuacoes.json");
            jogadores = JsonSerializer.Deserialize<Dictionary<string, (int vitorias, int empates, int derrotas)>>(json);
        }
        else
        {
            jogadores = new Dictionary<string, (int vitorias, int empates, int derrotas)>();
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro ao carregar as estatísticas: {ex.Message}");
    }
}

/// <summary>
/// Exibe a lista de jogadores cadastrados e suas estatísticas.
/// </summary>
/// <param name="continuar">Referência para a variável que armazena a decisão do jogador sobre continuar ou não.</param>
void ListarEstatisticasJogadores(ref char continuar)
{
    LimparTela();
    Console.BackgroundColor = ConsoleColor.DarkGray;
    EscreverMensagem("Jogadores e suas estatísticas:");

    LimparLinha();

    Console.WriteLine("");

    EscreverMensagem("_____________________________________________________________");
    EscreverMensagem("|       Jogador       |  Vitórias  |  Empates  |  Derrotas  |");
    foreach (var jogador in jogadores)
    {
        EscreverMensagem($"|  {jogador.Key.PadRight(19)}|  {jogador.Value.vitorias.ToString().PadRight(10)}|  {jogador.Value.empates.ToString().PadRight(9)}|  {jogador.Value.derrotas.ToString().PadRight(10)}|");
    }
    EscreverMensagem("_____________________________________________________________");

    Console.WriteLine("");
    continuar = ExibirMenu("E agora? Quer iniciar uma nova partida?", ('1', "Sim", ConsoleColor.DarkBlue), ('0', "Não", ConsoleColor.Red));
}
#endregion


#region Fluxo Principal
Inicializar();

var continuar = ExibirMenu("😀 Olá! Vamos jogar Jokempo?", ('1', "Sim", ConsoleColor.DarkBlue), ('0', "Não", ConsoleColor.Red));


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
EscreverMensagem("👋 Tchau! Até a próxima");
#endregion