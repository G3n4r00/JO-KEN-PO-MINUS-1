using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jokempo
{
    public class PlayerStatistics
    {
        public int Victories { get; set; }
        public int Draws { get; set; }
        public int Defeats { get; set; }

        // Construtor padrão necessário para serialização
        public PlayerStatistics()
        {
            Victories = 0;
            Draws = 0;
            Defeats = 0;
        }

        // Construtor para inicialização com valores
        public PlayerStatistics(int victories, int draws, int defeats)
        {
            Victories = victories;
            Draws = draws;
            Defeats = defeats;
        }
    }

}

