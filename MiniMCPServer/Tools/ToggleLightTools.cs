using ModelContextProtocol.Server;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MiniMCPServer.Tools
{
    /// <summary>
    /// Sample MCP tools for demonstration purposes.
    /// These tools can be invoked by MCP clients to perform various operations.
    /// </summary>
    [McpServerToolType]
    internal class ToggleLightTools
    {
        // Mock data for the lights
        private readonly List<LightModel> lights = new()
        {
            new LightModel { Id = 1, Name = "Table Lamp", IsOn = false },
            new LightModel { Id = 2, Name = "Porch light", IsOn = false },
            new LightModel { Id = 3, Name = "Chandelier", IsOn = true }
        };

        [McpServerTool]
        [Description("Gets a list of lights and their current state.")]
        public List<LightModel> GetLights()
        {
            Console.WriteLine("GetLights called");
            return lights;
        }

        [McpServerTool]
        [Description("Changes the state of the light.")]
        public LightModel ChangeStates(
            [Description("Id of the light")] int id,
            [Description("New State of the light.")] bool isOn
        )
        {
            Console.WriteLine("ChangeStates");
            var light = lights.FirstOrDefault(light => light.Id == id);

            if (light == null)
            {
                return null;
            }

            // Update the light with the new state
            light.IsOn = isOn;

            return light;
        }
    }

    public class LightModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("is_on")]
        public bool? IsOn { get; set; }
    }
}
