using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public interface IMasterOfGame
{

    Transform SpawnAI(Vector3 position, Transform aiPrefab, AISettings settings, Image compassPre);
    Transform SpawnAggressiveAI();
    Transform SpawnDefensiveAI();
    Transform SpawnAggressiveAI(Vector3 position);
    Transform SpawnDefensiveAI(Vector3 position);
    Transform SpawnRandomAI(Vector3 position);
    int Score { get; }
    int NextSpawn { get; }
    int GetSpawnNumber();
    bool IsSpawnAllowed();
    void Pause();
    void Quit();
    void UploadSuccess();
    void UploadFailed(string name, string score, string time, string id);
    void CommitScore();
    void BackToMainMenu();
    void EndOfGame();
    void AIDied(Transform ai);
    void AddScore(int points);


}
