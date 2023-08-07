using System.Text.Json.Serialization;

namespace SwitchBotMqttApp.Models.SwitchBotApi;

public class GetSceneListBody
{
    [JsonPropertyName("sceneId")]
    public string SceneId { get; set; } = default!;
    [JsonPropertyName("sceneName")]
    public string SceneName { get; set; } = default!;
}