using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

public class RigidTransformationScript : MonoBehaviour
{

    [SerializeField] private Transform[] points;
    [SerializeField] private LineController line;


    // Source points lists
    private List<Vector3> sourcePoints1;
    private List<Vector3> sourcePoints2;

    // Number of RANSAC iterations
    public int ransacIterations = 1000;

    // Threshold for inliers
    public float inlierThreshold = 0.1f;


    // Method to receive the List of 3DPoints that been read
    public void ReceiveVectorList(List<Vector3> received3DPoints, List<Vector3> received3DPoints2)
    {


        foreach (Vector3 vector in received3DPoints)
        {
            Debug.Log("Received Vector: " + vector);
        }

        Debug.Log("----------------------------");

        foreach (Vector3 vector2 in received3DPoints2)
        {
            Debug.Log("Received Vector: " + vector2);
        }

        // Initialize source points with taken points
        sourcePoints1 = received3DPoints;
        sourcePoints2 = received3DPoints2;

        // Call the rigid transformation function using RANSAC
        List<Vector3> transformedPoints1 = RansacRigidTransform(sourcePoints1, sourcePoints2);

        // Convert List<Vector3> to Transform[]
        Transform[] transformArray = new Transform[transformedPoints1.Count];

        for (int i = 0; i < transformedPoints1.Count; i++)
        {
            // Create a new GameObject and set its position using the Vector3
            GameObject newObject = new GameObject("TransformObject" + i);
            newObject.transform.position = transformedPoints1[i];

            // Add the Transform component to the array
            transformArray[i] = newObject.transform;
        }

        // Now, transformArray contains the Transform components corresponding to the List<Vector3>

        // Example: Log the position of each Transform in the array
        foreach (Transform transform in transformArray)
        {
            Debug.Log("Transform Position: " + transform.position);
        }

        line.SetUpLine(transformArray);
    }


    // Start is called before the first frame update
    void Start()
    {
        

    }

    // Qi - R*Pi = T
    List<Vector3> RansacRigidTransform(List<Vector3> source, List<Vector3> target)
    {
        int numPoints = source.Count;
        int bestInlierCount = 0;
        Matrix4x4 bestTransformation = Matrix4x4.identity;

        for (int iteration = 0; iteration < ransacIterations; iteration++)
        {
            // Randomly select a minimal sample (e.g., 3 points) for the transformation
            List<Vector3> sampleSource = GetRandomSample(source, 3);
            List<Vector3> sampleTarget = GetRandomSample(target, 3);

            // Estimate rigid transformation from the minimal sample
            Matrix4x4 transformation = EstimateRigidTransformation(sampleSource, sampleTarget);

            // Apply the estimated transformation to all source points
            List<Vector3> transformedPoints = ApplyTransformation(source, transformation);

            // Count inliers based on a threshold
            int inlierCount = CountInliers(transformedPoints, target, inlierThreshold);

            // Update the best transformation if the current one has more inliers
            if (inlierCount > bestInlierCount)
            {
                bestInlierCount = inlierCount;
                bestTransformation = transformation;
            }
        }

        // Apply the best transformation to all source points
        List<Vector3> finalTransformedPoints = ApplyTransformation(source, bestTransformation);

        return finalTransformedPoints;

    }

    List<Vector3> GetRandomSample(List<Vector3> points, int sampleSize)
    {

        List<Vector3> RandomResult = new List<Vector3>();

        // Check that there are enough points to select from
        if (points.Count < sampleSize)
        {
            Debug.LogError("Not enough points to generate a random sample.");
            return RandomResult;
        }

        // Randomly select sampleSize number of points from the source List
        for (int i = 0; i < sampleSize; i++)
        {
            int randomIndex = UnityEngine.Random.Range(0, points.Count);
            RandomResult.Add(points[randomIndex]);
        }

        return RandomResult;
    }

    Matrix4x4 EstimateRigidTransformation(List<Vector3> source, List<Vector3> target)
    {
        // Check if the input lists have the same number of points
        if (source.Count != target.Count || source.Count < 3)
        {
            Debug.LogError("Invalid input for rigid transformation estimation.");
            return Matrix4x4.identity;
        }

        // Calculate the centroids of the source and target point sets
        Vector3 centroidSource = CalculateCentroid(source);
        Vector3 centroidTarget = CalculateCentroid(target);

        // Calculate the covariance matrix
        Matrix4x4 covarianceMatrix = CalculateCovarianceMatrix(source, target, centroidSource, centroidTarget);

        // Calculate the rotation quaternion
        Quaternion rotationQuaternion = CalculateRotationQuaternion(covarianceMatrix);

        // Calculate the translation vector
        Vector3 translationVector = centroidTarget - rotationQuaternion * centroidSource;

        // Create the 4x4 transformation matrix
        Matrix4x4 transformationMatrix = Matrix4x4.TRS(translationVector, rotationQuaternion, Vector3.one);

        return transformationMatrix;


    }


    Matrix4x4 CalculateCovarianceMatrix(List<Vector3> source, List<Vector3> target, Vector3 centroidSource, Vector3 centroidTarget)
    {
        Matrix4x4 covarianceMatrix = Matrix4x4.zero;

        for (int i = 0; i < source.Count; i++)
        {
            Vector3 sourceDeviation = source[i] - centroidSource;
            Vector3 targetDeviation = target[i] - centroidTarget;

            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    covarianceMatrix[row, col] += sourceDeviation[row] * targetDeviation[col];
                }
            }
        }

        // Set the last row and column to zeros for 3x3 covariance matrix
        covarianceMatrix.SetRow(3, Vector4.zero);
        covarianceMatrix.SetColumn(3, Vector4.zero);

        return covarianceMatrix;
    }


    int CountInliers(List<Vector3> originalPoints, List<Vector3> transformedPoints, float distanceThreshold)
    {
        if (originalPoints.Count != transformedPoints.Count)
        {
            Debug.LogError("Point sets must have the same number of points.");
            return 0;
        }

        int inlierCount = 0;

        for (int i = 0; i < originalPoints.Count; i++)
        {
            float distance = Vector3.Distance(originalPoints[i], transformedPoints[i]);

            if (distance < distanceThreshold)
            {
                inlierCount++;
            }
        }

        return inlierCount;
    }

    Vector3 CalculateCentroid(List<Vector3> points)
    {
        if (points == null || points.Count == 0)
        {
            Debug.LogError("Cannot calculate centroid for an empty or null list of points.");
            return Vector3.zero;
        }

        Vector3 sum = Vector3.zero;

        foreach (Vector3 point in points)
        {
            sum += point;
        }

        return sum / points.Count;
    }

    Quaternion CalculateRotationQuaternion(Matrix4x4 covarianceMatrix)
    {
        DenseMatrix matrix = DenseMatrix.OfArray(new double[,]
         {
            { covarianceMatrix[0, 0], covarianceMatrix[0, 1], covarianceMatrix[0, 2], 0 },
            { covarianceMatrix[1, 0], covarianceMatrix[1, 1], covarianceMatrix[1, 2], 0 },
            { covarianceMatrix[2, 0], covarianceMatrix[2, 1], covarianceMatrix[2, 2], 0 },
            { 0, 0, 0, 1 } // Homogeneous coordinate
         });

        var svd = matrix.Svd();

        DenseMatrix rotationMatrix = DenseMatrix.OfArray(new double[,]
        {
        { svd.U[0, 0], svd.U[0, 1], svd.U[0, 2] },
        { svd.U[1, 0], svd.U[1, 1], svd.U[1, 2] },
        { svd.U[2, 0], svd.U[2, 1], svd.U[2, 2] }
        });

        // Check if the determinant is close to 1 (valid rotation matrix)
        if (Math.Abs(rotationMatrix.Determinant() - 1.0) > 1e-6)
        {
            // Adjust the sign of a column to ensure a positive determinant
            var sign = Math.Sign(rotationMatrix.Determinant());
            rotationMatrix = DenseMatrix.OfArray(new double[,]
            {
            { rotationMatrix[0, 0], rotationMatrix[0, 1], sign * rotationMatrix[0, 2] },
            { rotationMatrix[1, 0], rotationMatrix[1, 1], sign * rotationMatrix[1, 2] },
            { rotationMatrix[2, 0], rotationMatrix[2, 1], sign * rotationMatrix[2, 2] }
            });
        }

        Quaternion rotationQuaternion = RotationMatrixToQuaternion(rotationMatrix);

        rotationQuaternion.Normalize();

        return rotationQuaternion;
    }

    Quaternion RotationMatrixToQuaternion(DenseMatrix rotationMatrix)
    {
        // Extract quaternion elements from the rotation matrix
        float trace = (float)rotationMatrix.Trace();
        float w = Mathf.Sqrt(1f + trace) / 2f;
        float scale = 1f / (4f * w);
        float x = (float)(rotationMatrix[2, 1] - rotationMatrix[1, 2]) * scale;
        float y = (float)(rotationMatrix[0, 2] - rotationMatrix[2, 0]) * scale;
        float z = (float)(rotationMatrix[1, 0] - rotationMatrix[0, 1]) * scale;

        return new Quaternion(x, y, z, w);
    }

    List<Vector3> ApplyTransformation(List<Vector3> points, Matrix4x4 transformationMatrix)
    {
        List<Vector3> transformedPoints = new List<Vector3>();

        foreach (Vector3 point in points)
        {
            // Apply rotation and translation to each point
            Vector4 homogeneousPoint = new Vector4(point.x, point.y, point.z, 1f);
            Vector4 transformedHomogeneousPoint = transformationMatrix * homogeneousPoint;

            // Convert back to Vector3
            Vector3 transformedPoint = new Vector3(transformedHomogeneousPoint.x, transformedHomogeneousPoint.y, transformedHomogeneousPoint.z);

            transformedPoints.Add(transformedPoint);
        }

        return transformedPoints;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


}
