using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace NXStartCenter.Model
{
    public class GitLabRelease
    {
        [JsonPropertyName("tag_name")]
        public string TagName { get; set; } = "";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        [JsonPropertyName("assets")]
        public GitLabAssets? Assets { get; set; }
    }

    public class GitLabAssets
    {
        [JsonPropertyName("links")]
        public GitLabReleaseLink[]? Links { get; set; }
    }

    public class GitLabReleaseLink
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("url")]
        public string Url { get; set; } = "";

        [JsonPropertyName("direct_asset_url")]
        public string? DirectAssetUrl { get; set; }

        [JsonPropertyName("link_type")]
        public string? LinkType { get; set; }
    }
}
