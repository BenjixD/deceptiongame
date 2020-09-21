using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controller for MapGenerator
// A few things here can be seperated into their own scripts later
public class MapController : MonoBehaviour
{
    public MapGenerator mapGenerator;

    public GameObject playerPrefab;
    public int numPlayersToSpawn = 4;

    public GameObject treePrefab;

    public int treeSparseness = 1;

    public bool testMouse = true;

    public Transform treeParent;

    public MinimapController minimap;

    public FogOfWarGenerator fogOfWar;


    [Header("Debug")]
    public float cameraDistance = 10f;

    public bool showDebugCubes = false;
    public Transform cubeParent;


    private GameObject[,] debugCubes;

    private List<List<MapGenerator.Coord>> activeGreenCubes;

    private GameObject mainPlayer;

    void Start() {
        this.Initialize();
    }

    public void Initialize() {
        this.debugCubes = new GameObject[mapGenerator.width, mapGenerator.height];
        this.activeGreenCubes = new List<List<MapGenerator.Coord>>();
        this.mapGenerator.GenerateMap();
        this.CreateCubes();
        this.SpawnTrees();
        this.SpawnPlayers();
        this.minimap.Initialize(this.mapGenerator, this.mainPlayer);
    }

    // Update is called once per frame
	void Update() {
        // Test World to map coord
		if (Input.GetMouseButton(0) && testMouse) {
            RaycastHit hitInfo;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hitInfo)) {
                MapGenerator.Coord nearestPoint = mapGenerator.NearestWorldPointToCoord(hitInfo.point);
                MapTileType tile = mapGenerator.GetMapCoord(nearestPoint);

                if (tile != MapTileType.OUT_OF_BOUNDS) {
                    this.debugCubes[nearestPoint.tileX, nearestPoint.tileY].GetComponent<MeshRenderer>().material.color = Color.red;
                }

            }
		}

        // Restart 
        if (Input.GetKeyDown(KeyCode.R)) {
            this.Initialize();
        }

        // Spawn random stuff
        if (Input.GetKeyDown(KeyCode.T)) {
            int numCount = 5;
            int radius = 1;
            List<MapGenerator.Coord> loc = mapGenerator.GetRandomOpenCoords(numCount, radius, true);
            foreach (MapGenerator.Coord coord in loc) {

                List<MapGenerator.Coord> cubesForSquare = new List<MapGenerator.Coord>();

                for (int x = -radius; x <= radius; x++) {
			        for (int y = -radius; y <= radius; y++) {
                        int drawX = coord.tileX + x;
                        int drawY = coord.tileY + y;
                        this.debugCubes[drawX, drawY].GetComponent<MeshRenderer>().material.color = Color.green;
                        MapGenerator.Coord newCoord = new MapGenerator.Coord(drawX, drawY);
                        mapGenerator.SetMapTile(newCoord, MapTileType.OTHER);
                        cubesForSquare.Add(newCoord);
                    }
                }

                this.activeGreenCubes.Add(cubesForSquare);
            }
        }

        // Remove a patch of random stuff
        if (Input.GetKeyDown(KeyCode.Y) && this.activeGreenCubes.Count > 0) {
            List<MapGenerator.Coord> selectedCube = this.activeGreenCubes[0];
            this.activeGreenCubes.RemoveAt(0);
            foreach(MapGenerator.Coord coord in selectedCube) {
                this.debugCubes[coord.tileX, coord.tileY].GetComponent<MeshRenderer>().material.color = Color.white;
                this.mapGenerator.SetMapTile(coord, MapTileType.EMPTY);
            }
        }
	}

    private void CreateCubes() {
        if (showDebugCubes) {
            for (int x = 0; x < mapGenerator.width; x++) {
                List<GameObject> debugCubeRow = new List<GameObject>();
                for (int y = 0; y < mapGenerator.height; y++) {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.SetParent(this.cubeParent);
                    cube.transform.position = mapGenerator.CoordToWorldPoint(new MapGenerator.Coord(x, y));
                    cube.transform.localScale = Vector3.one * 0.5f;
                    cube.gameObject.name = "Cube: " + x + " " + y;

                    this.debugCubes[x, y] = cube;
                }
            }
        }
    }

    private void SpawnTrees() {
        MapTileType[,] map = this.mapGenerator.GetMap();
        for (int x = 0; x < this.mapGenerator.width; x++) {
            for (int y = 0; y < this.mapGenerator.height; y ++) {
                if (map[x,y] == MapTileType.WALL) {

                    if ((x % this.treeSparseness == 0 && y % this.treeSparseness == 0) || (x == 0 || y == 0 || x == this.mapGenerator.width-1 || y == this.mapGenerator.height-1)) {
                        MapGenerator.Coord coord = new MapGenerator.Coord(x, y);
                        Vector3 spawnLocation = this.mapGenerator.CoordToWorldPoint(coord);
                        GameObject tree = Instantiate(treePrefab, spawnLocation, Quaternion.identity) as GameObject;
                        float randScale = Random.Range(0.5f, 2f);
                        tree.transform.localScale = new Vector3(randScale, randScale, 1);
                        tree.transform.SetParent(this.treeParent);
                    }

                }
            }
        }
    }

    private void SpawnPlayers() {
        List<MapGenerator.Coord> playerLocations = mapGenerator.GetRandomOpenCoords(this.numPlayersToSpawn, 1, false);
        int index = 0;

        foreach (MapGenerator.Coord coord in playerLocations) {
            Vector3 spawnLocation = mapGenerator.CoordToWorldPoint(coord);
            GameObject player = Instantiate(playerPrefab, spawnLocation, Quaternion.identity) as GameObject;
            if (index == 0) {
                this.SetMainPlayer(player);
            } else {
                player.GetComponent<ShowOnlyIfInRange>().Initialize(this.mainPlayer.transform);
                player.gameObject.name += " " + index;
            }
            index++;
        }
    }

    private void SetMainPlayer(GameObject player) {
        this.mainPlayer = player;
        this.mainPlayer.name = "Main player";
        Vector3 playerPos = this.mainPlayer.transform.position;
        Camera.main.transform.position = new Vector3(playerPos.x, playerPos.y + this.cameraDistance, playerPos.z - this.cameraDistance);
        Camera.main.transform.LookAt(playerPos);
        Camera.main.transform.SetParent(player.transform);
        fogOfWar.player = player.transform;
        fogOfWar.transform.SetParent(player.transform);
        fogOfWar.gameObject.transform.position = new Vector3(player.transform.position.x , player.transform.position.y + 100, player.transform.position.z);
    }
}
