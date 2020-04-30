using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PathFinding
{
    public class Node : IAStarNode
    {
        public Vector3 nodePosition = Vector3.zero;
        List<IAStarNode> neighbourList = new List<IAStarNode>();
        public NodeType nodeType = NodeType.Grass;
        public GameObject tileObject = null;

        // Day cost of the different types. Defined as const so they are not modified.
        // Water is always a maximum value, so the algorithm never take it in count.
        const int GRASS_DAYS = 1;
        const int FOREST_DAYS = 3;
        const int DESERT_DAYS = 5;
        const int MOUNTAIN_DAYS = 10;
        const int WATER_DAYS = int.MaxValue;

        // GValue contain the value of moving to this tile. Defined by DefineNodeType() on constructor.
        public int gValue = 0;

        public enum NodeType
        {
            //Contains the type of nodes that can exist
            Grass = 0, Forest= 1, Desert = 2, Mountain = 3, Water = 4
        }

        public Node(Vector3 position)
        {
            //Node constructor.
            this.nodePosition = position;
            DefineNodeType();
        }

        public void DefineNodeType(bool random = true)
        {
            //If Random is TRUE, define a random tile. If it's FALSE, it change to the next tile type. It's used so the user can change the tile behavior with right click.

            if (random)
            {
                nodeType = (NodeType)UnityEngine.Random.Range(0, 5);
            } else
            {
                nodeType++;
                if((int)nodeType > 4)
                {
                    nodeType = 0;
                }
            }

            //Change the GCost
            if(nodeType == NodeType.Grass)
            {
                gValue = GRASS_DAYS;
            } else if(nodeType == NodeType.Forest)
            {
                gValue = FOREST_DAYS;
            } else if(nodeType == NodeType.Desert)
            {
                gValue = DESERT_DAYS;
            } else if(nodeType == NodeType.Mountain)
            {
                gValue = MOUNTAIN_DAYS;
            } else if(nodeType == NodeType.Water)
            {
                gValue = WATER_DAYS;
            }
        }

        public void AddNeighbour(Node nbr)
        {
            //This function allows the NodeCreator to reference the neighbours of this node.
            if (!neighbourList.Contains(nbr))
            {
                neighbourList.Add(nbr);
            }
        }

        IEnumerable<IAStarNode> IAStarNode.Neighbours
        {
            get
            {
                //Return the neighbour nodes.
                return neighbourList;
            }
        }


        float IAStarNode.CostTo(IAStarNode next)
        {
            //Search if the next node is a neighbour. If it is, return the day cost of moving to that tile.
            foreach(Node node in neighbourList)
            {
                if(node == next)
                {
                    return node.gValue;
                } else
                {
                    continue;
                }
            }
            //Else return max value so the algorithm don't take in count before others.
            return int.MaxValue;

        }
        float IAStarNode.EstimatedCostTo(IAStarNode target)
        {
            //Find the target node position calling the main Node List on Node Creator.
            Node lastNode = NodeCreator.instance.FindNode(target);
            Vector3 lastPos = lastNode.nodePosition;

            //Check the distance between this node and the last node.
            var distance = Vector3.Distance(lastNode.nodePosition, lastPos);

            //Multiply by the biggest cost suposing all tiles are the same (Not water)
            return distance * MOUNTAIN_DAYS;

        }

    }
}
