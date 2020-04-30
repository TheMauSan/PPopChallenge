using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinding
{
    public class NodeCreator : MonoBehaviour
    {
        //For Singleton
        public static NodeCreator instance { get; private set; }

        [Header("Tile Prefab")]
        [Tooltip("The Grid is constructed with this GameObject.")]
        [SerializeField] GameObject nodeObj = null;

        [Space]
        [Tooltip("Different textures of each tipe of tile")]
        [Header("Tile Textures")]
        [SerializeField] Texture t_grass, t_forest, t_desert, t_mountain, t_water = null;

        Dictionary<Node.NodeType, Texture> textureRef = new Dictionary<Node.NodeType, Texture>();

        [Space]
        [Header("Map Size")]
        [Tooltip("The grid will be constructed between the margins of this Vector. Can be changed from inspector and pre visualized with a Gizmo.")]
        [SerializeField] Vector3 mapSize = Vector3.one;
        [Tooltip("Put a limit to the quantity of nodes to generate in case the map is oversized.")]
        [SerializeField] int maxTiles = 500;

        [Space]
        [Tooltip("Allow the user to change the colors of the tiles when a path is choosen.")]
        [Header("Path Colors")]
        [SerializeField] Color startNodeColor = Color.green;
        [SerializeField] Color currentPathColor = Color.red;
        [SerializeField] Color oldPathColor = Color.blue;
        [SerializeField] Color waterColor = Color.blue;

        //Contain a List of all existing nodes on screen.
        List<Node> nodeList = new List<Node>();

        //Final path reference.
        IList<IAStarNode> finalPath = new List<IAStarNode>();

        //Define the start and end node when searching a path.
        Node startNode = null;
        Node endNode = null;

        //Just for tile positioning when actives.
        float inactiveZ = 0;
        float activeZ = 0;

        private void Awake()
        {
            //The Singleton check if there's no other Node Creator so there won't be another object creating other node layer.
            if (instance == null)
            {
                instance = this;
            } else
            {
                Destroy(gameObject);
            }

            //Create Texture Dictionary.
            textureRef.Add(Node.NodeType.Grass, t_grass);
            textureRef.Add(Node.NodeType.Forest, t_forest);
            textureRef.Add(Node.NodeType.Desert, t_desert);
            textureRef.Add(Node.NodeType.Mountain, t_mountain);
            textureRef.Add(Node.NodeType.Water, t_water);

        }

        // Start is called before the first frame update
        void Start()
        {
            //Create the nodes on screen.
            CreateNodes();
        }
        

        #region Node Creation (Main Functions)
        private void CreateNodes()
        {
            // This function creates the points in the map where the tiles will be placed.

            if (nodeObj == null)
            {
                // In case the tile Prefab is not selected, it returns an error.
                Debug.LogWarning("There's no node object selected!");
                return;
            }


            //The map Limit is selected with a Vector3. The tiles won't go forward the Vector3 margins.
            Vector3 mapLimit = mapSize / 2;

            //The starting point of the first tile is in the center of the GameObject who possess this component. Create the first node and add it to a list of references.
            Vector3 startPos = transform.position;
            Node startNode = new Node(startPos);
            nodeList.Add(startNode);

            //Put a limit on the amount of tiles to create.
            for (var i = 0; i < maxTiles; i++)
            {
                if (i >= nodeList.Count)
                {
                    //Break when all nodes are between the margins.
                    break;

                }
                Node currentNode = nodeList[i];

                //Get a list with the neighbour positions
                foreach (Vector3 neighbour in NeighbourPositions(currentNode.nodePosition, nodeObj.transform.localScale))
                {

                    //Check if a Tile GameObject exists in the neighbour position. 
                    //Since the positions are float points (not exacts), uses Mathf.Approximately to check it properly.
                    Node neighbourNode = nodeList.Find(n => Mathf.Approximately(n.nodePosition.x, neighbour.x) && Mathf.Approximately(n.nodePosition.y, neighbour.y));

                    //If a neighbour exists, add it to the current node neighbours, else, if the amount of max tiles is not surpased, create and add it.
                    if (neighbourNode != null)
                    {
                        currentNode.AddNeighbour(neighbourNode);
                    }
                    else if(nodeList.Count < maxTiles)
                    {
                        
                        neighbourNode = new Node(neighbour);
                        currentNode.AddNeighbour(neighbourNode);
                        nodeList.Add(neighbourNode);
                    }
                }

            }

            //Now all the nodes had been created in the positions. Now creates the objects.
            CreateMap();
        }
        private void CreateMap()
        {
            //This function creates the GameObjects on the map once all the Positions are setted and make a reference in each node to the correspondent GameObject (Tile).
            
            foreach(Node node in nodeList)
            {
                var newTile = Instantiate(nodeObj, node.nodePosition, Quaternion.identity);
                var renderer = newTile.GetComponentInChildren<Renderer>();


                node.tileObject = newTile;
                renderer.material.mainTexture = textureRef[node.nodeType];
            }

            //Just set active and inactive Z position.
            inactiveZ = nodeList[0].tileObject.transform.position.z;
            activeZ += 0.1f;
        }

        private List<Vector3> NeighbourPositions(Vector3 center, Vector3 size)
        {
            //This list returns the neighbour position in the six directions of the hexagon. 
            //It's based on the center of the node and the neighbours are multiplied by the node object size. This way the positions are always correct even if the object changes the size.

            float centerScaleX = size.x;
            float centerScaleY = size.y;
            Vector3 centerPosition = center;
            List<Vector3> positionList = new List<Vector3>();
            List<Vector3> checkedList = new List<Vector3>();
            Vector3 currentPos = new Vector3();

            Vector3 mapLimit = mapSize / 2;

            bool checkMapLimit = currentPos.x > (-mapLimit.x) && currentPos.x < mapLimit.x && currentPos.y < mapLimit.y && currentPos.y > (-mapLimit.y);

            // Left side
            currentPos = centerPosition + (Vector3.left * centerScaleX);
            positionList.Add(currentPos);

            currentPos = centerPosition + (Vector3.left * centerScaleX / 2) + (Vector3.up * centerScaleY / 1.35f);
            positionList.Add(currentPos);

            currentPos = centerPosition + (Vector3.left * centerScaleX / 2) + (Vector3.down * centerScaleY / 1.35f);
            positionList.Add(currentPos);

            // Right side
            currentPos = centerPosition + (Vector3.right * centerScaleX);
            positionList.Add(currentPos);

            currentPos = centerPosition + (Vector3.right * centerScaleX / 2) + (Vector3.up * centerScaleY / 1.35f);
            positionList.Add(currentPos);

            currentPos = centerPosition + (Vector3.right * centerScaleX / 2) + (Vector3.down * centerScaleY / 1.35f);
            positionList.Add(currentPos);


            //Once we have the six neighbour positions, we check if it's in the margins of the map size (Inside our Vector3 named mapSize)
            foreach(Vector3 pos in positionList)
            {
                checkMapLimit = pos.x > (-mapLimit.x) && pos.x < mapLimit.x && pos.y < mapLimit.y && pos.y > (-mapLimit.y);

                if (checkMapLimit)
                {
                    checkedList.Add(pos);
                }
            }

            //Return the Vectors of the neighbours that can exist in the map size.
            return checkedList;
        }


        #endregion

        #region Path Functions
        public  void SetPath(Transform node)
        {
            //This function set the Start and End point depending in the current activity.
            //This is the latest pressed node.
            Node currentNode = nodeList.Find(n => n.tileObject == node.transform.gameObject);

            //If the user is pressing water, make a short animation and return.
            if(currentNode.nodeType == Node.NodeType.Water)
            {
                Debug.Log("You are clicking on water!");
                StartCoroutine(ClickOnWater(currentNode));
                return;
            }


            if (startNode == null)
            {
                //If there's no start point, set the node as the first point.
                startNode = currentNode;
            }
            else
            {
                //If there's a start point, set the current node as the end of the path.
                endNode = currentNode;

                //If the start and the end point are the same, reset the nodes.
                if (startNode == endNode)
                {
                    ResetAllNodes(true);
                    return;
                }

                // This function change the color of the previous path under the new one. User can change the color of the old path on inspector to watch already traversed nodes. 
                ColorPreviousPath(); 

                //If everything is okay, Show the path between the Start and End points.
                finalPath = AStar.GetPath(startNode, endNode);
                ShowPathOnNodes(finalPath);

                //Once the path is done, make the End the new Start, so the player can continue searching from the last point.
                startNode = endNode;
            }

            //Change color to the start and end points.
            node.transform.position += new Vector3(0, 0, -activeZ);
            node.transform.GetComponentInChildren<Renderer>().material.color = startNodeColor;
        }
        private void ResetAllNodes(bool resetStartAndEnd = false)
        {
            //This clean the start and end node references with the old paths.
            if(resetStartAndEnd)
            {
                startNode = null;
                endNode = null;
                finalPath.Clear();
            }

            //This function return all nodes to the original inactive state.
            foreach(Node node in nodeList)
            {
                if(node == startNode) { continue; }

                Transform nodeObj = node.tileObject.transform;
                nodeObj.position = new Vector3(nodeObj.position.x, nodeObj.position.y, inactiveZ);
                nodeObj.GetComponentInChildren<Renderer>().material.color = Color.white;
            }
        }

        #endregion

        #region Display and Responsive Functions
        private void ShowPathOnNodes(IList<IAStarNode> list)
        {
            //This function take the list returned from GetPath() method and paint the nodes on screen.
            List<Node> finalPath = new List<Node>();
            foreach(IAStarNode node in list)
            {
                finalPath.Add(nodeList.Find(n => n == node));
            }

            //Paint and move up the path nodes.
            foreach (Node node in finalPath)
            {
                if (node == startNode) { continue; }
                    GameObject thisNode = node.tileObject;
                    var theRenderer = thisNode.GetComponentInChildren<Renderer>().material.color = currentPathColor;
                    thisNode.transform.position += new Vector3(0, 0, -activeZ);

            }

            // -------- DEBUG ONLY! This function draws a Debug Line between the path nodes.
            //var lineI = 0;
            //foreach(Node node in finalPath)
            //{
            //    try
            //    {
            //        Debug.DrawLine(finalPath[lineI].nodePosition, finalPath[lineI + 1].nodePosition, Color.red, 5f);
            //        lineI++;
            //    }
            //    catch
            //    {
            //        return;
            //    }
            //}
        }
        private void ColorPreviousPath()
        {
            //This function color the old paths under the new ones.
            //The color can be changed on inspector. It's white by default, so it won't show if there's no change from inspector.

            //If there's no previous path, just return.
            if(finalPath.Count == 0 || finalPath == null) { return; }

            //Change the color of the previous path.
            foreach(IAStarNode node in finalPath)
            {
                if(node == startNode || node == endNode) { continue; }

                var thisNode = nodeList.Find(n => n == node).tileObject;
                thisNode.GetComponentInChildren<Renderer>().material.color = oldPathColor;
                thisNode.transform.position += new Vector3(0, 0, inactiveZ);
            }
        }
        IEnumerator ClickOnWater(Node waterNode)
        {
            //This function reject the user when pressing on water tiles and make a tiny response.
            var waterObj = waterNode.tileObject;
            var waterRenderer = waterObj.GetComponentInChildren<Renderer>();

            waterObj.transform.position += new Vector3(0, 0, activeZ);
            waterRenderer.material.color = waterColor;
            yield return new WaitForSeconds(0.2f);
            waterObj.transform.position -= new Vector3(0, 0, activeZ);
            waterRenderer.material.color = Color.white;
        }

        #endregion

        #region Extra Functions
        public IEnumerator ChangeNodeBehavior(Transform Node)
        {
            //This function allows the User to switch a node behavior and value on runtime.
            //It's a Coroutine so the function can be responsive for the user.

            Node nodeToChange = nodeList.Find(n => n.tileObject == Node.gameObject);
            var nodeObj = nodeToChange.tileObject;
            nodeToChange.DefineNodeType(false);
            nodeObj.GetComponentInChildren<Renderer>().material.mainTexture = textureRef[nodeToChange.nodeType];


            nodeObj.transform.position += new Vector3(0, 0, activeZ);
            yield return null;
            nodeObj.transform.position -= new Vector3(0, 0, activeZ);

        }

        public Node FindNode(IAStarNode node)
        {
            //Allows to find a specific node object.
            return nodeList.Find(n => n == node);
        }
        #endregion


        private void OnDrawGizmos()
        {
            //This gizmo is in charge of showing on screen the size of the Map. The map tiles will be drawn between the mapSize (Vector3) margin.
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, mapSize);
        }
    }
}