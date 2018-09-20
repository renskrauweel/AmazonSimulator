using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models
{
    public class Node
    {
        public char letter;
        public double shortestDistanceFromStart;
        public Node previousVertex = null;
        public double lengthFromPreviousVertex;
        public List<Node> nodes = new List<Node>();

        public Node(char letter, double lengthFromPreviousVertex)
        {
            this.letter = letter;
            this.lengthFromPreviousVertex = lengthFromPreviousVertex;
        }
    }
}
