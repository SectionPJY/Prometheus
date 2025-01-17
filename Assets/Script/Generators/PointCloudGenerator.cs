using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 포인트 클라우드를 생성하는 클래스
/// </summary>
public class PointCloudGenerator
{
	private GameObject pointCloudObject;// 포인트들을 가지고 있는 오브젝트
	private PointState pointState;      // 포인트 클라우드의 상태정보를 가지고 있는 클래스

	/// <summary>
	/// 포인트 클라우드의 상태 정보용 변수
	/// </summary>
	private float limitDepth;           // 포인트 클라우드의 깊이 제한
	private float distanceFromCar;      // 포인트 클라우드와 RCCar 사이의 거리
	private int size;                   // 원활한 출력을 위한 포인트 클라우드를 나눌 개수
    private float pointRatio;             // 포인트 클라우드 크기 비율

	private Color[] colors;			 // 포인트 클라우드의 색상


	private int a = 0;

	public PointCloudGenerator()
	{
		colors = new Color[10];
		
		// colors[0] = new Color(7f/255f, 20f/255f, 128f/255f, 255f/255f);
		// colors[1] = new Color(128f/255f, 128f/255f, 7f/255f, 255f/255f);
		colors[0] = Color.blue;
		colors[1] = Color.red;
		// colors[3] = Color.green;
		// colors[4] = Color.yellow;
		// colors[5] = Color.cyan;
		// colors[6] = Color.magenta;
		// colors[7] = Color.gray;
		// colors[8] = Color.black;
		// colors[9] = Color.white;
	}

	public PointState PointState
	{
		get { return pointState; }
		set { 
			pointState = value; 

			limitDepth = pointState.LimitDepth;
			distanceFromCar = pointState.DistanceFromCar;
			size = pointState.Size;
			pointRatio = pointState.PointRatio;	

		}
	}
	public GameObject PointCloudObject
	{
		get { return pointCloudObject; }
	}

	/// <summary>
    /// 현재 프레임(now)에 대한 포인트 클라우드를 생성
    /// </summary>
    /// <param name="now">현재 프레임 번호</param>
    /// <param name="dataController">데이터 컨트롤러</param>
    /// <param name="car">Car 스크립트</param>
	/// <param name="pointCloudMaterial">포인트 클라우드를 렌더링할 Material</param>
	/// <returns>생성된 포인트 클라우드 오브젝트</returns>
	public void GeneratePointCloud(int now, DataController dataController, CarController car, Material pointCloudMaterial)
    {
		a = now;
        // JSON 파일 파싱
        PointData jsonData = dataController.GetJsonData();

        int width = dataController.GetWidth();
        int height = dataController.GetHeight();

        // RCCar의 위치, 방향, 회전 정보를 가져옴
        Vector3 carPosition = car.CarPosition;      // 위치
        Vector3 carForward = car.CarForward;        // 방향
        Quaternion carRotation = car.CarRotation;   // 회전

        // 포인트 클라우드 생성
        GameObject pointCloudObject = CreatePointCloudObject(carPosition, carForward, carRotation);
        Mesh pointCloudMesh = CreatePointCloudMesh(jsonData, width, height, now);

        SetMeshAndMaterial(pointCloudObject, pointCloudMesh);
    }

	/// <summary>
	/// Car의 위치 및 방향을 기반으로 포인트 클라우드 오브젝트를 생성
	/// </summary>
	/// <param name="carPosition">Car의 위치</param>
	/// <param name="carForward">Car의 방향</param>
	/// <param name="carRotation">Car의 회전값</param>
	/// <returns>생성된 포인트 클라우드 오브젝트(포인트들을 가지고 있는 오브젝트)</returns>
	private GameObject CreatePointCloudObject(Vector3 carPosition, Vector3 carForward, Quaternion carRotation)
	{
		pointCloudObject = new GameObject("PointCloud");
		pointCloudObject.transform.rotation = carRotation;
		pointCloudObject.transform.position = carPosition + distanceFromCar * carForward;
		return pointCloudObject;
	}

	/// <summary>
	/// JSON 데이터를 기반으로 포인트 클라우드 메쉬를 생성
	/// </summary>
	/// <param name="jsonData">포인트 클라우드 데이터를 담고 있는 JSON 객체</param>
	/// <param name="width">포인트 클라우드의 가로 크기</param>
	/// <param name="height">포인트 클라우드의 세로 크기</param>
	/// <param name="now">현재 프레임 번호</param>
	/// <returns>생성된 포인트 클라우드 Mesh</returns>
	private Mesh CreatePointCloudMesh(PointData jsonData, int width, int height, int now)
	{
		Mesh pointCloudMesh = new Mesh();

		int hSize = height / size;
		int startIndex = hSize * now;

		int arraySize = width * hSize;

		Vector3[] vertices = new Vector3[arraySize]; // 포인트 클라우드의 위치를 설정할 Vector3 배열
		Color[] colors = new Color[arraySize];       // 각 포인트의 색상을 저장할 배열

		int vertexIndex = 0;

		for (int i = startIndex; i < startIndex + hSize; i++)
		{
			float ratio =  (i* -0.1458f + 100f) / 100f;
			// Debug.Log(ratio);
			for (int j = 0; j < width; j++)
			{
				float depth = jsonData.depth_frame[i][j];

				// float x = (j * pointRatio * ratio)  + (width / 2) - (j * pointRatio * ratio)/2;
				// float x = (j - width/2) * pointRatio * ratio + width/2f*pointRatio;/
				// float x = j * pointRatio * ratio;
				float x = j * pointRatio;
				float y = (height - i) * pointRatio;
				float z = depth * pointRatio;

				// depth가 0이면서 limitDepth보다 클 경우 해당 포인트는 포인트 클라우드에 추가하지 않음
				// if (z == 0 || z > limitDepth || z < 50)  continue;
				if(z == 0) continue;

				vertices[vertexIndex] = new Vector3(x - (width / 2f) * pointRatio, y, z);
				colors[vertexIndex] = GetPointColor(z);

				// 색상은 GetPointColor 함수를 이용하여 설정

				vertexIndex++;
			}
		}

		pointCloudMesh.vertices = vertices;
		pointCloudMesh.colors = colors;
		pointCloudMesh.SetIndices(Enumerable.Range(0, arraySize).ToArray(), MeshTopology.Points, 0);

		return pointCloudMesh;
	}
	/// <summary>
	/// 포인트 클라우드 오브젝트에 Mesh와 Material을 설정
	/// </summary>
    /// <param name="pointCloudObject">Mesh 및 Material이 설정될 포인트 클라우드 오브젝트</param>
    /// <param name="pointCloudMesh">포인트 클라우드 Mesh</param>
	private void SetMeshAndMaterial(GameObject pointCloudObject, Mesh pointCloudMesh)
	{
		MeshFilter meshFilter = pointCloudObject.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = pointCloudObject.AddComponent<MeshRenderer>();

		meshFilter.mesh = pointCloudMesh;

		// Color randomDepthColor = GetPointColor(Random.Range(0f, limitDepth));
		// Color randomDepthColor = GetArrayColor(a);

		// Material 생성 및 설정
		Material pointCloudMaterial = new Material(Shader.Find("Custom/PointCloudShader"));
		// pointCloudMaterial.color = randomDepthColor;
		pointCloudMaterial.SetFloat("_DepthFactorStrength", 3f);
		meshRenderer.material = pointCloudMaterial;

	}

	/// <summary>
	/// 깊이(depth)에 따라 색상을 반환
	/// </summary>
	/// <param name="depth">깊이 값</param>
	/// <returns></returns>
	private Color GetPointColor(float depth)
    {
        float normalizedDepth = Mathf.InverseLerp(0f, limitDepth, depth);
        return Color.Lerp(colors[0], colors[1], normalizedDepth);
    }

	// private Color GetTestPointColor(float depth)
	// {
	// 	float normalizedDepth = Mathf.InverseLerp(0f, limitDepth, depth);
	// 	// 3등분을 위한 기준 값
	// 	float redRangeEnd = 1f / 10f;
	// 	float greenRangeEnd = 2f / 10f;
	// 	float blueRangeEnd = 3f / 10f;
	// 	float yellowRangeEnd = 4f / 10f;
	// 	float whiteRangeEnd = 5f / 10f;
	// 	float blackRangeEnd = 6f / 10f;
	// 	float grayRangeEnd = 7f / 10f;
	// 	float cyanRangeEnd = 8f / 10f;
	// 	float magentaRangeEnd = 9f / 10f;
	// 	float brownRangeEnd = 10f / 10f;
		
	// 	if (normalizedDepth <= redRangeEnd)
	// 	{
	// 		// 범위 0~redRangeEnd: 빨강
	// 		return Color.green;
	// 	}
	// 	else if (normalizedDepth <= greenRangeEnd)
	// 	{
	// 		// 범위 redRangeEnd~greenRangeEnd: 초록
	// 		return Color.red;
	// 	}
	// 	else if(normalizedDepth <= blueRangeEnd){
	// 		return Color.blue;
	// 	}
	// 	else if(normalizedDepth <= yellowRangeEnd){
	// 		return Color.yellow;
	// 	}
	// 	else if(normalizedDepth <= whiteRangeEnd){
	// 		return Color.white;
	// 	}
	// 	else if(normalizedDepth <= blackRangeEnd){
	// 		return Color.black;
	// 	}
	// 	else if(normalizedDepth <= grayRangeEnd){
	// 		return Color.gray;
	// 	}
	// 	else if(normalizedDepth <= cyanRangeEnd){
	// 		return Color.cyan;
	// 	}
	// 	else if(normalizedDepth <= magentaRangeEnd){
	// 		return Color.magenta;
	// 	}
	// 	else if(normalizedDepth <= brownRangeEnd){
	// 		return new Color(128f/255f, 128f/255f, 7f/255f, 255f/255f);
	// 	}
	// 	else{
	// 		return Color.black;
	// 	}
	// }

	private Color GetArrayColor(int index)
	{
		return colors[index];
	}
}