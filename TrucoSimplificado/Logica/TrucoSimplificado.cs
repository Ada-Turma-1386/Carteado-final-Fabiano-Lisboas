using System;
using System.Collections.Generic;
using TrucoSimplificado.Models;
using TrucoSimplificado.Logica;

using TrucoSimplificado.Utils;

namespace TrucoSimplificado
{
    // Classe principal que orquestra o jogo de Truco Paulista.
    public class TrucoSimplificado
    {
        private List<Carta> baralho;
        private Jogador jogadorHumano;
        private Jogador jogadorComputador;
        private int manilha;
        private Random random;
        private ControladorRodadas controladorRodadas;
        private GerenciadorDeApostas gerenciadorDeApostas;

        // Construtor da classe TrucoSimplificado.
        public TrucoSimplificado()
        {
            baralho = new List<Carta>();
            jogadorHumano = new Jogador("Você");
            jogadorComputador = new Jogador("Computador");
            random = new Random();
            controladorRodadas = new ControladorRodadas();
            gerenciadorDeApostas = new GerenciadorDeApostas();
            CriarBaralho();
        }

        // Cria o baralho de 40 cartas do Truco .
        private void CriarBaralho()
        {
            baralho.Clear();
            int[] valores = { 4, 5, 6, 7, 11, 12, 13, 14, 15, 16 };
            for (int naipe = 1; naipe <= 4; naipe++)
            {
                foreach (int valor in valores)
                {
                    baralho.Add(new Carta(naipe, valor));
                }
            }
        }

        // Embaralha as cartas do baralho.
        private void EmbaralharCartas()
        {
            for (int i = 0; i < baralho.Count; i++)
            {
                int j = random.Next(baralho.Count);
                Carta temp = baralho[i];
                baralho[i] = baralho[j];
                baralho[j] = temp;
            }
        }

        // Define a manilha para a rodada atual.
        private void DefinirManilha()
        {
            Carta cartaVirada = baralho[0];
            baralho.RemoveAt(0);

            int[] valores = { 4, 5, 6, 7, 11, 12, 13, 14, 15, 16 };
            int index = Array.IndexOf(valores, cartaVirada.Valor);
            manilha = valores[(index + 1) % valores.Length];

            ConsoleInterface.ExibirMensagem("Carta virada: " + cartaVirada.ObterTexto());
            ConsoleInterface.ExibirMensagem("Manilha da rodada: " + Models.Carta.ObterNomeValor(manilha));
        }

        // Distribui 3 cartas para cada jogador.
        private void DistribuirCartas()
        {
            for (int i = 0; i < 3; i++)
            {
                jogadorHumano.ReceberCarta(baralho[0]);
                baralho.RemoveAt(0);
                jogadorComputador.ReceberCarta(baralho[0]);
                baralho.RemoveAt(0);
            }
        }

        // Inicia e controla o fluxo principal do jogo.
        public void Jogar()
        {
            ConsoleInterface.ExibirMensagem("=== TRUCO SIMPLIFICADO ===");
            ConsoleInterface.ExibirMensagem("Primeiro a fazer 12 pontos ganha!");

            while (jogadorHumano.Pontuacao < 12 && jogadorComputador.Pontuacao < 12)
            {
                PrepararNovaMao();
                JogarMao();
                ConsoleInterface.ExibirMensagem("\nPressione qualquer tecla para continuar...");
                ConsoleInterface.AguardarTecla();
            }

            ExibirResultadoFinal();
        }

        // Prepara o estado para uma nova mão.
        private void PrepararNovaMao()
        {
            jogadorHumano.Mao.Clear();
            jogadorComputador.Mao.Clear();
            CriarBaralho();
            EmbaralharCartas();
            DefinirManilha();
            DistribuirCartas();
            gerenciadorDeApostas.Reset();
            controladorRodadas.Reset();
        }

        // Controla o fluxo de uma única mão, com suas 3 rodadas.
        private void JogarMao()
        {
            bool jogadorComeca = true;

            ConsoleInterface.LimparTela();
            ConsoleInterface.ExibirMensagem("╔══════════════════════════════════════════════════╗");
            ConsoleInterface.ExibirMensagem("║                  NOVA MÃO                       ║");
            ConsoleInterface.ExibirMensagem("╚══════════════════════════════════════════════════╝");
            ConsoleInterface.ExibirMensagem($"💰 Pontos em disputa: {gerenciadorDeApostas.PontosRodada}");
            ConsoleInterface.ExibirMensagem($"📊 Placar atual: Você {jogadorHumano.Pontuacao} x {jogadorComputador.Pontuacao} Computador");
            ConsoleInterface.ExibirMensagem($"🎯 Manilha: {Models.Carta.ObterNomeValor(manilha)}");
            ConsoleInterface.ExibirMensagem("");
            
            // As cartas serão exibidas durante cada rodada, não aqui


            for (int rodada = 1; rodada <= 3; rodada++)
            {
                if (controladorRodadas.MaoEstaFinalizada())
                    break;

                ConsoleInterface.ExibirMensagem("┌─────────────────────────────────────────────────┐");
                ConsoleInterface.ExibirMensagem($"│                   RODADA {rodada}                    │");
                ConsoleInterface.ExibirMensagem("└─────────────────────────────────────────────────┘");

                // SEMPRE exibe as cartas do jogador no início de cada rodada
                ExibirMaoJogador();

                // Agora o computador pode tentar apostas (após o jogador ver suas cartas)
                if (ComputadorTentaAposta())
                {
                    return; // Mão acabou
                }

                Carta cartaJogador = null;
                Carta cartaComputador = null;

                if (jogadorComeca)
                {
                    cartaJogador = ProcessarJogadaHumano();
                    if (cartaJogador == null) { { rodada--; continue; } }

                    cartaComputador = JogarCartaComputador(cartaJogador);
                }
                else
                {
                    cartaComputador = JogarCartaComputador(null);
                    cartaJogador = ProcessarJogadaHumano();
                }

                if (cartaJogador != null && cartaComputador != null)
                {
                    jogadorComeca = AvaliarResultadoRodada(cartaJogador, cartaComputador);
                    
                    // Exibe o status atual da mão após cada rodada
                    if (!controladorRodadas.MaoEstaFinalizada())
                    {
                        ExibirStatusMao();
                    }
                }
            }

            DeterminarVencedorMao();
        }

        // Lógica da IA do computador para decidir se deve fazer uma aposta.
        private bool ComputadorTentaAposta()
        {
            if (ComputadorQuerTruco())
            {
                int proximaAposta = gerenciadorDeApostas.ObterProximaAposta();
                if (proximaAposta > 0)
                {
                    if (!ProcessarAposta(proximaAposta, false))
                    {
                        return true; // Mão acabou
                    }
                    ConsoleInterface.ExibirMensagem("┌─────────────────────────────────────────────────┐");
                    ConsoleInterface.ExibirMensagem($"│           APOSTA ACEITA! ({gerenciadorDeApostas.PontosRodada} pontos)          │");
                    ConsoleInterface.ExibirMensagem("└─────────────────────────────────────────────────┘");
                }
            }
            return false;
        }

        // Processa a jogada do computador.
        private Carta JogarCartaComputador(Carta cartaAdversario)
        {
            int indice = ComputadorEscolherCarta(cartaAdversario);
            Carta carta = jogadorComputador.JogarCarta(indice);
            ConsoleInterface.ExibirMensagem($"🤖 Computador jogou: {carta.ObterTexto()}");
            return carta;
        }

        // Avalia o resultado de uma rodada e determina quem começa a próxima.
        private bool AvaliarResultadoRodada(Carta cartaJogador, Carta cartaComputador)
        {
            ConsoleInterface.ExibirMensagem("\n⚔️ RESULTADO DA BATALHA:");
            int forcaJogador = cartaJogador.ObterForca(manilha);
            int forcaComputador = cartaComputador.ObterForca(manilha);

            ConsoleInterface.ExibirMensagem($"   Sua carta: {cartaJogador.ObterTexto()} (força: {forcaJogador})");
            ConsoleInterface.ExibirMensagem($"   Carta do PC: {cartaComputador.ObterTexto()} (força: {forcaComputador})");

            bool empatou = forcaJogador == forcaComputador;
            bool jogadorGanhou = forcaJogador > forcaComputador;

            if (empatou)
            {
                ConsoleInterface.ExibirMensagem("\n🤝 EMPATE NA RODADA!");
                controladorRodadas.AdicionarResultado(false, true);
                return true; // Jogador começa a proxima
            }
            else if (jogadorGanhou)
            {
                ConsoleInterface.ExibirMensagem("\n🎉 VOCÊ GANHOU A RODADA!");
                controladorRodadas.AdicionarResultado(true, false);
                return true;
            }
            else
            {
                ConsoleInterface.ExibirMensagem("\n😔 COMPUTADOR GANHOU A RODADA!");
                controladorRodadas.AdicionarResultado(false, false);
                return false;
            }
        }

        // Determina e exibe o vencedor da mão.
        private void DeterminarVencedorMao()
        {
            ConsoleInterface.ExibirMensagem("\n" + new string('═', 50));
            // Verifica quem venceu a mão ou se houve empate.
            if (controladorRodadas.VitoriasJogador > controladorRodadas.VitoriasComputador)
            {
                ConsoleInterface.ExibirMensagem("🏆 VOCÊ GANHOU A MÃO!");
                jogadorHumano.Pontuacao += gerenciadorDeApostas.PontosRodada;
            }
            else if (controladorRodadas.VitoriasComputador > controladorRodadas.VitoriasJogador)
            {
                ConsoleInterface.ExibirMensagem("💻 COMPUTADOR GANHOU A MÃO!");
                jogadorComputador.Pontuacao += gerenciadorDeApostas.PontosRodada;
            }
            else // Caso de empate na mão (ex: 0x0 após 3 empates de rodada)
            {
                ConsoleInterface.ExibirMensagem("🤝 MÃO EMPATADA! Ninguém pontua nesta mão.");
            }

            ConsoleInterface.ExibirMensagem($"💰 Pontos ganhos: {gerenciadorDeApostas.PontosRodada}");
            ConsoleInterface.ExibirMensagem($"📊 PLACAR GERAL: Você {jogadorHumano.Pontuacao} x {jogadorComputador.Pontuacao} Computador");
            ConsoleInterface.ExibirMensagem(new string('═', 50));
        }

        // Exibe o resultado final do jogo.
        private void ExibirResultadoFinal()
        {
            ConsoleInterface.ExibirMensagem("\n=== FIM DE JOGO ===");
            if (jogadorHumano.Pontuacao >= 12)
            {
                ConsoleInterface.ExibirMensagem("*** PARABÉNS! VOCÊ VENCEU! ***");
            }
            else
            {
                ConsoleInterface.ExibirMensagem("*** COMPUTADOR VENCEU! ***");
            }

            ConsoleInterface.ExibirMensagem("Placar final: Você " + jogadorHumano.Pontuacao + " x " + jogadorComputador.Pontuacao + " Computador");
        }

        // Processa a jogada do jogador humano.
        private Carta ProcessarJogadaHumano()
        {
            // As cartas já foram exibidas no início da rodada, agora exibe apenas as opções
            ConsoleInterface.ExibirOpcoes(jogadorHumano, gerenciadorDeApostas.PodeApostar(true), gerenciadorDeApostas.ObterProximaAposta());

            while (true)
            {
                ConsoleInterface.ExibirMensagem("\n🎮 Digite sua escolha: ");
                string input = ConsoleInterface.ObterEntrada();

                if (string.IsNullOrWhiteSpace(input))
                {
                    ConsoleInterface.ExibirMensagem("❌ Entrada vazia! Tente novamente.");
                    continue;
                }

                if (gerenciadorDeApostas.PodeApostar(true))
                {
                    int proximaAposta = gerenciadorDeApostas.ObterProximaAposta();
                    if (ValidadorEntrada.ValidarApostaDisponivel(input, gerenciadorDeApostas.PontosRodada))
                    {
                        if (!ProcessarAposta(proximaAposta, true))
                        {
                            return null;
                        }
                        return null;
                    }
                }

                if (ValidadorEntrada.ValidarEscolhaCarta(input, jogadorHumano.ContarCartas()))
                {
                    int escolha = int.Parse(input.Trim());
                    Carta carta = jogadorHumano.JogarCarta(escolha - 1);
                    ConsoleInterface.ExibirMensagem($"\n✅ Você jogou: {carta.ObterTexto()}");
                    return carta;
                }

                ConsoleInterface.ExibirMensagem("❌ Opção inválida!");
            }
        }

        // Processa uma aposta feita por um dos jogadores.
        private bool ProcessarAposta(int tipoAposta, bool jogadorPediu)
        {
            string nomeAposta = "";
            if (tipoAposta == 3) nomeAposta = "TRUCO";
            else if (tipoAposta == 6) nomeAposta = "SEIS";
            else if (tipoAposta == 9) nomeAposta = "NOVE";
            else if (tipoAposta == 12) nomeAposta = "DOZE";

            if (jogadorPediu)
            {
                ConsoleInterface.ExibirMensagem($"\n🔥 VOCÊ PEDIU {nomeAposta}!");
                ConsoleInterface.ExibirMensagem("🤖 Computador está pensando...");

                if (ComputadorAceitaTruco())
                {
                    ConsoleInterface.ExibirMensagem("💪 Computador: ACEITO!");
                    gerenciadorDeApostas.AceitarAposta(tipoAposta, true);
                    return true;
                }
                else
                {
                    ConsoleInterface.ExibirMensagem("🏃 Computador: CORRO!");
                    ConsoleInterface.ExibirMensagem($"🎉 Você ganhou {gerenciadorDeApostas.PontosRodada} ponto(s)!");
                    jogadorHumano.Pontuacao += gerenciadorDeApostas.PontosRodada;
                    return false;
                }
            }
            else
            {
                ConsoleInterface.ExibirMensagem($"\n🔥 Computador: {nomeAposta}!");
                string resposta = ConsoleInterface.ObterEntrada();

                if (ValidadorEntrada.RespostaEhSim(resposta))
                {
                    ConsoleInterface.ExibirMensagem($"✅ Você aceitou o {nomeAposta.ToLower()}!");
                    gerenciadorDeApostas.AceitarAposta(tipoAposta, false);
                    return true;
                }
                else
                {
                    ConsoleInterface.ExibirMensagem($"🏃 Você correu do {nomeAposta.ToLower()}!");
                    ConsoleInterface.ExibirMensagem($"💻 Computador ganhou {gerenciadorDeApostas.PontosRodada} ponto(s)!");
                    jogadorComputador.Pontuacao += gerenciadorDeApostas.PontosRodada;
                    return false;
                }
            }
        }

        // Lógica da IA para decidir se o computador deve pedir truco.
        private bool ComputadorQuerTruco()
        {
            int cartasFortes = 0;
            foreach (Carta carta in jogadorComputador.Mao)
            {
                if (carta.ObterForca(manilha) >= 14 || carta.ObterForca(manilha) >= 37)
                {
                    cartasFortes++;
                }
            }
            return cartasFortes >= 2;
        }

        // Lógica da IA para decidir se o computador aceita um pedido de truco.
        private bool ComputadorAceitaTruco()
        {
            int cartasFortes = 0;
            foreach (Carta carta in jogadorComputador.Mao)
            {
                if (carta.ObterForca(manilha) >= 12 || carta.ObterForca(manilha) >= 37)
                {
                    cartasFortes++;
                }
            }
            return cartasFortes >= 1;
        }

        // Lógica da IA para escolher a melhor carta a ser jogada.
        private int ComputadorEscolherCarta(Carta cartaAdversario)
        {
            if (cartaAdversario != null)
            {
                int forcaAdversario = cartaAdversario.ObterForca(manilha);
                int melhorIndice = -1;
                int menorForcaQueGanha = 999;

                for (int i = 0; i < jogadorComputador.ContarCartas(); i++)
                {
                    int forca = jogadorComputador.Mao[i].ObterForca(manilha);
                    if (forca > forcaAdversario && forca < menorForcaQueGanha)
                    {
                        menorForcaQueGanha = forca;
                        melhorIndice = i;
                    }
                }

                if (melhorIndice != -1)
                {
                    return melhorIndice;
                }

                return EncontrarCartaMaisFraca();
            }
            else
            {
                return EncontrarCartaMaisForte();
            }
        }

        // Encontra a carta mais fraca na mão do computador.
        private int EncontrarCartaMaisFraca()
        {
            int menorForca = 999;
            int indice = 0;
            for (int i = 0; i < jogadorComputador.ContarCartas(); i++)
            {
                int forca = jogadorComputador.Mao[i].ObterForca(manilha);
                if (forca < menorForca)
                {
                    menorForca = forca;
                    indice = i;
                }
            }
            return indice;
        }

        // Encontra a carta mais forte na mão do computador.
        private int EncontrarCartaMaisForte()
        {
            int maiorForca = -1;
            int indice = 0;
            for (int i = 0; i < jogadorComputador.ContarCartas(); i++)
            {
                int forca = jogadorComputador.Mao[i].ObterForca(manilha);
                if (forca > maiorForca)
                {
                    maiorForca = forca;
                    indice = i;
                }
            }
            return indice;
        }

        // Método centralizado para exibir a mão do jogador humano.
        private void ExibirMaoJogador()
        {
            ConsoleInterface.ExibirMensagem("\n🃏 SUAS CARTAS:");
            ConsoleInterface.ExibirMao(jogadorHumano);
            ConsoleInterface.ExibirMensagem(""); // Linha em branco para separar
        }

        // Método para exibir o status atual da mão.
        private void ExibirStatusMao()
        {
            ConsoleInterface.ExibirMensagem($"\n📊 Status da mão: {controladorRodadas.ObterStatusMao()}");
        }
    }
}
