using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.IO;

public class TrackingObject
{
    public int userID { get; set; }
    public int task { get; set; }
    public string language { get; set; }
    public string group { get; set; }
    public List<string> machineLayout { get; set; }
    public List<string> trackings { get; set; }
}


public class Tracking : MonoBehaviour {

    public static Tracking Instance;

    private string trackingEndpoint = "http://35.207.73.131/tracking";

    private Transform cursor;

    //Todo check if Stopwatch is the most efficient way to do this
    private Stopwatch stopwatch;

    private TrackingObject trackingRequest = new TrackingObject();

    private bool track;

    private string testRequestPrint;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        cursor = gameObject.transform;

        trackingRequest.trackings = new List<string>();

        trackingRequest.machineLayout = new List<string>();
        trackingRequest.machineLayout.Add("BoxNr; ProductId; ProductType; ProductNutriLabel; ProductNutriScore; Position.x; Position.y; Position.z; LocalScale.x; LocalScale.y; LocalScale.z; hasColor");
    }
	
	void Update () {
        if (track)
        {
            trackingRequest.trackings.Add($"{stopwatch.ElapsedMilliseconds}," +
                                          $"{cursor.position.x}," +
                                          $"{cursor.position.y}," +
                                          $"{cursor.position.z}," +
                                          $"{NutriboardOrganizer.Instance.CurrentBoxNr}");
        }
    }

    public void GetMachineLayoutInfo()
    {
        trackingRequest.language = SetupOrganizer.Instance.language;
        trackingRequest.userID = SetupOrganizer.Instance.userID;
        if (SelectaOrganizer.Instance.HasColor)
        {
            trackingRequest.group = "test";
        }
        else
        {
            trackingRequest.group = "control";
        }


        //Get the BoxNr, ProductId, Type, positions, color of all boxes.
        //TODO this should be done in the SelectaOrganizer and writen to a file
        foreach (string BoxKey in SelectaOrganizer.Instance.BoxKeys.Keys)
        {
            string productId = SelectaOrganizer.Instance.BoxKeys[BoxKey];
            GameObject Box = SelectaOrganizer.Instance.Boxes[BoxKey];
            trackingRequest.machineLayout.Add($"{BoxKey}; " +
                                              $"{SelectaOrganizer.Instance.ProductNutris[productId].id}; " +
                                              $"{SelectaOrganizer.Instance.ProductNutris[productId].type}; " +
                                              $"{SelectaOrganizer.Instance.ProductNutris[productId].nutri_label}; " +
                                              $"{SelectaOrganizer.Instance.ProductNutris[productId].nutri_score}; " +
                                              $"{Box.transform.position.x}; " +
                                              $"{Box.transform.position.y}; " +
                                              $"{Box.transform.position.z}; " +
                                              $"{Box.transform.localScale.x}; " +
                                              $"{Box.transform.localScale.y}; " +
                                              $"{Box.transform.localScale.z}; " +
                                              $"{SelectaOrganizer.Instance.HasColor}");
        }
    }

    /// <summary>
    /// Start the tracker, reset the timer
    /// </summary>
    public void StartTracker()
    {
        trackingRequest.trackings.Clear();
        trackingRequest.trackings.Add("timestamp,Cursor.Position.x,Cursor.Position.y,Cursor.Position.z,CurrentActiveNutriBarBoxNr");

        trackingRequest.task = SceneOrganizer.Instance.state - 1;

        stopwatch = Stopwatch.StartNew();

        track = true;
    }

    /// <summary>
    /// Stop the tracker, send the data of to the server
    /// </summary>
    public void StopTracker()
    {
        stopwatch.Stop();
        track = false;
        TrackingRequest();
    }

    private struct RequestPtr
    {
        public byte[] rawRequest;
    }

    /// <summary>
    /// Stop the tracker, send the data of to the server
    /// </summary>
    private async Task TrackingRequest()
    {
        RequestPtr rawrequestPtr = await Task.Run(() => SerializeDataAsync());

        StartCoroutine(SendTrackingRequest(rawrequestPtr, 0));
    }

    private IEnumerator SendTrackingRequest(RequestPtr rawrequestPtr, int tries)
    {
        if (tries >= 5)
        {
            //string path = Path.Combine(Application.persistentDataPath, "tracking.txt");
            //using (TextWriter writer = File.CreateText(path))
            //{
            //        
            //}
        }
        else
        {
            WWWForm webForm = new WWWForm();

            using (UnityWebRequest unityWebRequest = UnityWebRequest.Post(trackingEndpoint, webForm))
            {
                //Set headers
                unityWebRequest.SetRequestHeader("Content-Type", "application/json");

                //Upload Handler to handle data
                unityWebRequest.uploadHandler = new UploadHandlerRaw(rawrequestPtr.rawRequest);
                unityWebRequest.uploadHandler.contentType = "application/json";

                //Download handler
                unityWebRequest.downloadHandler = new DownloadHandlerBuffer();

                // Send the request
                yield return unityWebRequest.SendWebRequest();

                //await response
                if (unityWebRequest.downloadHandler.text.Contains("success"))
                {
                    UnityEngine.Debug.Log($"Sending Request - succeded: true");
                }
                else
                {
                    UnityEngine.Debug.Log($"Sending Request - succeded: false , Resending Request");
                    StartCoroutine(SendTrackingRequest(rawrequestPtr, tries++));
                }
            }
        }
    }


    private RequestPtr SerializeDataAsync()
    {
        RequestPtr rawrequestPtr = new RequestPtr();

        string jsonRequest = JsonConvert.SerializeObject(trackingRequest);

        rawrequestPtr.rawRequest = Encoding.UTF8.GetBytes(jsonRequest);

        return rawrequestPtr;
    }
}
