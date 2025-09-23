using System.ComponentModel;
using System.Text.Json;
using Microsoft.SemanticKernel;

namespace SKChatAgent.Plugins;

internal sealed class GitHubSettings
{
    public string BaseUrl { get; set; } = "https://api.github.com";

    public string Token { get; set; } = string.Empty;
}

internal sealed class GitHubPlugin(GitHubSettings settings)
{
    [KernelFunction("get_user_profile")]
    [Description("Get User Profile")]
    [return: Description("A User Profile")]
    public async Task<GitHubModels.User> GetUserProfileAsync()
    {
        using HttpClient client = CreateClient();
        JsonDocument response = await MakeRequestAsync(client, "/user");
        return response.Deserialize<GitHubModels.User>() ?? throw new InvalidDataException($"Request failed: {nameof(GetUserProfileAsync)}");
    }

    [KernelFunction("get_repository")]
    [Description("Get the information of one specific repository in one specific organization")]
    [return: Description("A Github Repository Profile")]
    public async Task<GitHubModels.Repo> GetRepositoryAsync(
        [Description("the name of the organization")] string organization,
        [Description("the name of the repo")]  string repo)
    {
        using HttpClient client = CreateClient();
        JsonDocument response = await MakeRequestAsync(client, $"/repos/{organization}/{repo}");

        return response.Deserialize<GitHubModels.Repo>() ?? throw new InvalidDataException($"Request failed: {nameof(GetRepositoryAsync)}");
    }

    [KernelFunction("get_issues")]
    [Description("Get the issues of one specific repository in one specific organization")]
    [return: Description("A List of Issues")]
    public async Task<GitHubModels.Issue[]> GetIssuesAsync(
        [Description("the name of the organization")]  string organization,
        [Description("the name of the repo")]  string repo,
        [Description("the number of issues returned in the response. Default count is 30")] int? maxResults = null,
        [Description("the status of the issue. It can be open, closed, or all")] string state = "",
        [Description("the label of the issue")]  string label = "",
        [Description("the assignee of the issue")]  string assignee = "")
    {
        using HttpClient client = CreateClient();

        string path = $"/repos/{organization}/{repo}/issues?";
        path = BuildQuery(path, "state", state);
        path = BuildQuery(path, "assignee", assignee);
        path = BuildQuery(path, "labels", label);
        path = BuildQuery(path, "per_page", maxResults?.ToString() ?? string.Empty);

        JsonDocument response = await MakeRequestAsync(client, path);

        return response.Deserialize<GitHubModels.Issue[]>() ?? throw new InvalidDataException($"Request failed: {nameof(GetIssuesAsync)}");
    }

    [KernelFunction("get_issue_detail")]
    [Description("Get the detail information of one specific issue of one specific repository in one specific organization")]
    [return: Description("A detailed information of one issue")]
    public async Task<GitHubModels.IssueDetail> GetIssueDetailAsync(
        [Description("the name of the organization")] string organization, 
        [Description("the name of the repo")] string repo,
        [Description("the id of the issue")] int issueId)
    {
        using HttpClient client = CreateClient();

        string path = $"/repos/{organization}/{repo}/issues/{issueId}";

        JsonDocument response = await MakeRequestAsync(client, path);

        return response.Deserialize<GitHubModels.IssueDetail>() ?? throw new InvalidDataException($"Request failed: {nameof(GetIssueDetailAsync)}");
    }

    private HttpClient CreateClient()
    {
        HttpClient client = new()
        {
            BaseAddress = new Uri(settings.BaseUrl)
        };

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("User-Agent", "request");
        client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {settings.Token}");
        client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");

        return client;
    }

    private static string BuildQuery(string path, string key, string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            return $"{path}{key}={value}&";
        }

        return path;
    }

    private static async Task<JsonDocument> MakeRequestAsync(HttpClient client, string path)
    {
        Console.WriteLine($"REQUEST: {path}");
        Console.WriteLine();

        HttpResponseMessage response = await client.GetAsync(new Uri(path, UriKind.Relative));
        response.EnsureSuccessStatusCode();
        string content = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(content);
    }
}