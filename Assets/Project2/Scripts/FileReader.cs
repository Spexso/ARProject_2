using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;

public class FileManager : MonoBehaviour
{
    [SerializeField] private GameObject prefab_s;
    [SerializeField] private GameObject prefab_t;

    // Points as float array
    private int num_points;

    // Points as float array
    private int num_points_2;

    // Variable to store file path
    string PointsfilePath, fileName, PointsfilePath2, fileName2;

    // 3D Vector made up from points
    List<Vector3> points3D;

    // 3D Vector made up from points
    List<Vector3> points3D_2;

    // RigidTransformer GameObject
    public RigidTransformationScript targetScript;

    // Start is called before the first frame update
    void Start()
    {
        fileName = "PointsCloud1.txt";
        PointsfilePath = Application.dataPath + "/Project2/" + fileName;

        fileName2 = "PointsCloud2.txt";
        PointsfilePath2 = Application.dataPath + "/Project2/" + fileName2;

        ReadPointCloud(PointsfilePath);

        ReadPointCloud2(PointsfilePath2);
        
        print("F1 Number of points=> " +  num_points);

        /*
         * Debug
        foreach (var point in points3D)
        {
            print(point.x);
            print(point.y);
            print(point.z);

            print("-----------------");
        }
        */

        // Reference to GameObject that is seen scene
        GetReferenceToTransformer();

        // Pass the Vector lists to the RigidTransformationScript
        if (targetScript != null)
        {
            targetScript.ReceiveVectorList(points3D, points3D_2);
        }

    }

    public void SpawnPoints(){
       
        for(int i= 0; i < points3D.Count; i++)
        {
            Instantiate(prefab_s, points3D[i], Quaternion.identity);
        }

        for (int i = 0; i < points3D_2.Count; i++)
        {
            Instantiate(prefab_t, points3D_2[i], Quaternion.identity);
        }
    }

    public void ReadPointCloud(string filePath)
    {
        points3D = new List<Vector3>();

        if (File.Exists(filePath))
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                num_points = int.Parse(reader.ReadLine());
                print(num_points);

                for (int i = 0; i < num_points; i++)
                {
                    string[] coordinates = reader.ReadLine()?.Split(' ');
                    float x = float.Parse(coordinates[0]);
                    float y = float.Parse(coordinates[1]);
                    float z = float.Parse(coordinates[2]);

                    points3D.Add(new Vector3(x, y, z));

                }
            }
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }

    }

    public void ReadPointCloud2(string filePath)
    {
        points3D_2 = new List<Vector3>();

        if (File.Exists(filePath))
        {
            using (StreamReader reader = new StreamReader(filePath))
            {
                num_points_2 = int.Parse(reader.ReadLine());
                print(num_points_2);

                for (int i = 0; i < num_points_2; i++)
                {
                    string[] coordinates = reader.ReadLine()?.Split(' ');
                    float x = float.Parse(coordinates[0]);
                    float y = float.Parse(coordinates[1]);
                    float z = float.Parse(coordinates[2]);

                    points3D_2.Add(new Vector3(x, y, z));

                }
            }
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }

    }

    public List<Vector3> getVectorList()
    {
        return points3D;
    }

    public void GetReferenceToTransformer()
    {

        // Find the GameObject 
        GameObject targetObject = GameObject.Find("RigidTransformer");

        if (targetObject != null)
        {
            // Get the target script from found GameObject
            targetScript = targetObject.GetComponent<RigidTransformationScript>();
        }
    }
}
