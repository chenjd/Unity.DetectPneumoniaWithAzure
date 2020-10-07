using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training;
using Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.Models;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;
using System.Text;

public class ObjectClassification : MonoBehaviour
{
    [SerializeField] private Sprite[] _testImages;
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _information;
    [SerializeField] private TextMeshProUGUI _indexText;
    [SerializeField] private string _projectID;
    [SerializeField] private string _publishedModelName;
    
    private int _index = -1;
    private MemoryStream _testStream;
    private CustomVisionPredictionClient _prediction;
    private CustomVisionTrainingClient _training;
    private Project _project;

    // Start is called before the first frame update
    void Start()
    {
        var endpoint = Environment.GetEnvironmentVariable("CUSTOM_VISION_ENDPOINT");

        var predictionKey = Environment.GetEnvironmentVariable("CUSTOM_VISION_PREDICTION_KEY");

        _prediction = new CustomVisionPredictionClient(new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Prediction.ApiKeyServiceClientCredentials(predictionKey))
        {
            Endpoint = endpoint
        };

        _training = new CustomVisionTrainingClient(new Microsoft.Azure.CognitiveServices.Vision.CustomVision.Training.ApiKeyServiceClientCredentials(predictionKey))
        {
            Endpoint = endpoint
        };

        _project = _training.GetProject(Guid.Parse(_projectID));
    }

    public void ChangeTexture()
    {
        if (_index < _testImages.Length - 1)
            _index++;
        else
            _index = 0;
        var sprite = _testImages[_index];
        _image.sprite = sprite;
        byte[] bytes = sprite.texture.EncodeToPNG();
        _testStream = new MemoryStream(bytes);

        var publishedModelName = "Iteration3";
        var result = _prediction.ClassifyImage(_project.Id, _publishedModelName, _testStream);

        var sb = new StringBuilder();

        foreach (var c in result.Predictions)
        {
            sb.Append($"{c.TagName}: {c.Probability:P1}\n");
        }

        _information.text = sb.ToString();
        _indexText.text = $"Index #{_index}";
    }

}
