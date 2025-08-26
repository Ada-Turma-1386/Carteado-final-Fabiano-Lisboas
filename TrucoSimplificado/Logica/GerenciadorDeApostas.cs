namespace TrucoSimplificado.Logica
{
    // Gerencia o estado e a lógica das apostas (Truco, Seis, etc.) em uma mão.
    public class GerenciadorDeApostas
    {
        // O valor em pontos da aposta atual na mão.
        public int PontosRodada { get; private set; }

        // Indica se as apostas foram encerradas na mão atual.
        public bool ApostasEncerradas { get; private set; }

        // Indica se a última aposta foi feita pelo jogador humano.
        public bool UltimaApostaFoiJogador { get; private set; }

        // Construtor da classe GerenciadorDeApostas.
        public GerenciadorDeApostas()
        {
            Reset();
        }

        // Reseta o estado das apostas para o início de uma nova mão.
        public void Reset()
        {
            PontosRodada = 1;
            ApostasEncerradas = false;
            UltimaApostaFoiJogador = false;
        }

        // Calcula o valor da próxima aposta possível.
        public int ObterProximaAposta()
        {
            switch (PontosRodada)
            {
                case 1: return 3;  // Truco
                case 3: return 6;  // Seis
                case 6: return 9;  // Nove
                case 9: return 12; // Doze
                default: return -1; // Não há mais apostas
            }
        }

        // Verifica se um jogador (humano ou computador) pode fazer uma aposta.
        public bool PodeApostar(bool isJogador)
        {
            if (ApostasEncerradas) return false;
            // A primeira aposta (Truco) pode ser feita por qualquer um.
            if (PontosRodada == 1) return true;
            // As apostas subsequentes devem ser alternadas.
            return isJogador != UltimaApostaFoiJogador;
        }

        // Processa a aceitação de uma aposta, atualizando o estado do gerenciador.
        public void AceitarAposta(int novaPontuacao, bool isJogador)
        {
            PontosRodada = novaPontuacao;
            UltimaApostaFoiJogador = isJogador;
            // Se a aposta chegou a 12, as apostas são encerradas.
            if (ObterProximaAposta() == -1)
            {
                ApostasEncerradas = true;
            }
        }
    }
}
