namespace TrucoSimplificado.Logica
{
    // Controla o estado e o resultado das rodadas dentro de uma mão de Truco.
    public class ControladorRodadas
    {
        // O número de vitórias do jogador humano na mão atual.
        public int VitoriasJogador { get; private set; }

        // O número de vitórias do computador na mão atual.
        public int VitoriasComputador { get; private set; }

        // Indica se a primeira rodada resultou em empate.
        public bool PrimeiraRodadaEmpatou { get; private set; }

        // Adiciona o resultado de uma rodada ao controlador.
        public void AdicionarResultado(bool jogadorGanhou, bool empatou)
        {
            if (empatou)
            {
                // Se a primeira rodada empatar, isso é registrado.
                if (VitoriasJogador == 0 && VitoriasComputador == 0)
                {
                    PrimeiraRodadaEmpatou = true;
                }
            }
            else if (jogadorGanhou)
            {
                VitoriasJogador++;
            }
            else
            {
                VitoriasComputador++;
            }
        }

        // Verifica se a mão atual já foi finalizada.
        public bool MaoEstaFinalizada()
        {
            // Se a primeira rodada empatou, a mão termina na próxima rodada com um vencedor.
            if (PrimeiraRodadaEmpatou)
            {
                return VitoriasJogador > 0 || VitoriasComputador > 0;
            }

            // Em uma mão normal, o primeiro a ganhar duas rodadas vence.
            return VitoriasJogador >= 2 || VitoriasComputador >= 2;
        }

        // Verifica se o jogador humano venceu a mão.
        public bool JogadorVenceuMao()
        {
            // Se a primeira rodada empatou, o vencedor da próxima rodada ganha a mão.
            if (PrimeiraRodadaEmpatou)
            {
                return VitoriasJogador > 0;
            }
            // Caso contrário, o jogador precisa de duas vitórias.
            return VitoriasJogador >= 2;
        }

        // Reseta o estado do controlador para o início de uma nova mão.
        public void Reset()
        {
            VitoriasJogador = 0;
            VitoriasComputador = 0;
            PrimeiraRodadaEmpatou = false;
        }

        // Retorna uma string com o status atual da mão.
        public string ObterStatusMao()
        {
            if (PrimeiraRodadaEmpatou)
            {
                return $"Primeira empatou - Você {VitoriasJogador} x {VitoriasComputador} Computador (próxima decide!)";
            }
            return $"Você {VitoriasJogador} x {VitoriasComputador} Computador";
        }
    }
}
