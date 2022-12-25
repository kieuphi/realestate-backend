using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Common.Shared.RealTimeProcess
{
    public class PaymentTransactionHandler : IDisposable
    {
        HubConnection connection;
        public PaymentTransactionHandler()
        {
            
        }

        public async Task<bool> ConnectToSignalR()
        {
            connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:5015/purchaseOrderHub")
                .Build();
            try
            {
                await connection.StartAsync();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public async Task Disconnect()
        {
            if (connection.State == HubConnectionState.Connected)
            {
                await connection.StopAsync();
            }
        }

        public void Dispose()
        {
            
        }
    }
}
