# PPopChallenge
Exercise made for the Panda Pop Team of Jam City. A* Pathfinding algorithm used on hexagonal nodes with different behaviors.

INSTRUCTIONS:
- Left Click allows to select the Start and End node. The path will be drawn over the fastest way of achieving the goal using the A* Pathfinding algorithm (Provided for the exercise)
- Clicking again the final node will clean the path and reset the map.
- Right click allows to change a tile behavior and texture on runtime.

COSTS:
- Crossing between each node has a different cost of "days".
- Walking through GRASS takes 1 day.
- Walking through FOREST takes 3 day.
- Walking through DESERT takes 5 day.
- Walking through MOUNTAIN takes 10 day.
- The Player CAN'T walk on WATER.
(To make this, the Water price is setted as Max Value, so the Algorithm always evade it.)

EXPLANATION:
As said, the exercise is done using the provided A* Algorithm, consisting of a static class and an interface for the nodes.
It was done creating 2 different classes. One is in charge of creating the map on runtime, make references to the existing nodes and make changes on the map regarding the user decisions.
The Node class make reference to the IAStarNode interface.
The NodeCreator create all the nodes on screen based on a Tile Prefab.
The map size is limited by a Vector3, it can be seen on screen with a Gizmo. Nodes will not go forward this Vector3 limits.
The addresses of each node is multiplied by the node size. This way the map will always be generated respectively without differences on the nodes positions.
On Inspector, he user can set the max amount of tiles who will be created instead of using the vector3 limits.

When the player make a click on a tile, it create a Start or End point, once the two points are setted, it will give a path.
When the player click another point, the path continues since the last point selected.
When the player click again the last node, the map will reset.

With right click, the player can change a tile behavior (cost and texture), so it can create his own map and test the algorithm in different ways.

On Inspector, the user can change the Map Size (The Gizmo) on the NodeCreator object and set his own size.
Even, the player can change the colors of the path, the start and end point, and can add a third color who will let the old paths colored, showing the nodes who had been already been searched by the algorithm.
This last color is setted in white by default, so it will not show at first sight.
This old paths will reset with the main path when the last node is clicked twice.

The Tile Objects have a Custom Unlit Color texture, who allows to have the unlit render but coloring the texture.

The Main Cost of each node depends on the behavior (texture) it has.
It correspond to the G Cost of the A* pathfinding algorithm.
The H Cost is the distance between the Start and End Point multiplied by 10 (Estimated cost of the most expensive tile, the Mountain)
The F Cost is the sum of G and H Costs.


Mauro Sanchez.
Exercise made for the Panda Pop Team of Jam City.
30/04/2020

