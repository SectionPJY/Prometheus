using UnityEngine;

namespace JsonSetting
{
    /// <summary>
    /// Json ������
    /// </summary>
    [System.Serializable]
    public struct ScanData
    {
        public string angle;
        public string distance;
    }

    [System.Serializable]
    public struct ScanDataArray
    {
        public ScanData[] data;
    }

    public class LoadJson
    {
        private ScanDataArray data;

        public ScanDataArray Data
        {
            get { return data; }
        }

        /// <summary>
        /// ��ο� �ִ� json ������ �д� ������
        /// </summary>
        /// <param name="path">json ������ �ִ� ��� ���</param>
        public LoadJson(string path)
        {
            TextAsset loadedJson = Resources.Load<TextAsset>(path);
            data = JsonUtility.FromJson<ScanDataArray>(loadedJson.ToString());
        }

    }
}

