using UnityEngine;
using System.Collections;

public interface IScoreUploader
{
    void UploadFailed(string name, string score, string time, string id);
    void CommitScore();
    void UploadSuccess();
}
