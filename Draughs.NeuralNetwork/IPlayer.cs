using System.Collections.Generic;

namespace Draughs.NeuralNetwork
{
    internal interface IPlayer
    {
        List<int> Move(List<int> gameBoard, int player);
    }
}