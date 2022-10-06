using HtmlConverter.Data.Entities;
using Microsoft.AspNetCore.SignalR;

namespace HtmlConverter.Hubs
{
    public interface IConverterHub
    {
        Task ConversionStatusChanged(int id, ConversionStatus status);
        Task NewConversionJob(ConversionJob job);
    }

    public class ConverterHub : Hub<IConverterHub>
    {
        public async Task ConversionStatusChanged(int id, ConversionStatus status)
        {
            await Clients.All.ConversionStatusChanged(id, status);
        }

        public async Task NewConversionJob(ConversionJob job)
        {
            await Clients.All.NewConversionJob(job);
        }
    }
}