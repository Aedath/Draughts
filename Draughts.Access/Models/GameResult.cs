using System.ComponentModel.DataAnnotations;

namespace Draughts.Access.Models
{
    public class GameResult
    {
        [Key]
        public int GameResultId { get; set; }

        public virtual ApplicationUser User { get; set; }
        public virtual NeuralNetwork Network { get; set; }
        public int Score { get; set; }
    }

    public class GameResultViewModel
    {
        public int Generation { get; set; }
        public int Score { get; set; }
    }
}