using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;
using System.Text.Json;

public partial class Plugin : BasePlugin, IPluginConfig<Config>
{
    private string blocksFolder = "";
    private string savedBlocksPath = "";
    private string modelsPath = "";

    public static BlockModels BlockModels { get; set; } = new BlockModels();

    public void Files()
    {
        // saved map blocks folder
        blocksFolder = Path.Combine(ModuleDirectory, "blocks");
        Directory.CreateDirectory(blocksFolder);

        // block models file
        modelsPath = Path.Combine(ModuleDirectory, "models.json");

        if (!string.IsNullOrEmpty(modelsPath))
        {
            if (!File.Exists(modelsPath))
            {
                using (FileStream fs = File.Create(modelsPath))
                    fs.Close();

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                };

                string jsonContent = JsonSerializer.Serialize(BlockModels, options);

                File.WriteAllText(modelsPath, jsonContent);
            }
        }
        LoadBlocksModels();
    }

    public void LoadBlocksModels()
    {
        if (!string.IsNullOrEmpty(modelsPath) && File.Exists(modelsPath))
        {
            string jsonContent = File.ReadAllText(modelsPath);
            BlockModels = JsonSerializer.Deserialize<BlockModels>(jsonContent) ?? new BlockModels();
        }
    }
}