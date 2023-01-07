using OpenRem.Engine;
using OpenRem.Service.Protocol;
using System.Threading.Tasks;

namespace OpenRem.Service.Client;

class DetectManagerClient : RpcClient<DetectManager.DetectManagerClient>, IDetectManager
{
    public DetectManagerClient(IChannelProvider channelProvider) : base(channelProvider)
    {
        this.Client = new DetectManager.DetectManagerClient(this.Channel);
    }

    public async Task<Analyzer[]> GetAnalyzersAsync()
    {
        var response = await this.Client.GetAnalyzersAsync(new EmptyRequest(), new CallOptions());
        return response.Analyzers.Select(x => new Analyzer
        {
            Id = Guid.Parse(x.Id),
            Name = x.Name
        }).ToArray();
    }
}