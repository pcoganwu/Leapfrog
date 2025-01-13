using Leapfrog.Application.Interfaces;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Protocol;

namespace Leapfrog.Infrastructure.Services
{
    /// <summary>
    /// Service for managing LoRaWAN communication using MQTT.
    /// </summary>
    public class LoRaWANService : ILoRaWANService
    {
        private readonly IManagedMqttClient _mqttClient;

        /// <summary>
        /// Event triggered when a message is received.
        /// </summary>
        public event Action<byte[]>? OnMessageReceived;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoRaWANService"/> class.
        /// </summary>
        public LoRaWANService()
        {
            // Configure MQTT client options

            var options = new ManagedMqttClientOptionsBuilder()
                 .WithClientOptions(new MqttClientOptionsBuilder()
                 .WithClientId("LoRaWANClient")
                 .WithTcpServer("broker.hivemq.com", 1883) // Replace with your MQTT broker address and port
                 .WithCredentials("username", "password") // Replace with your MQTT broker credentials
                 .WithCleanSession()
                 .Build())
                 .Build();

            // Create and configure the managed MQTT client
            _mqttClient = new MqttFactory().CreateManagedMqttClient();

            // Trigger the OnMessageReceived event when a message is received
            _mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                OnMessageReceived?.Invoke([.. e.ApplicationMessage.PayloadSegment]);
                await Task.CompletedTask;
            };
        }

        /// <summary>
        /// Connects to the MQTT broker asynchronously.
        /// </summary>
        /// <returns>A task that represents the asynchronous connect operation.</returns>
        public async Task ConnectAsync()
        {
            var options = new ManagedMqttClientOptionsBuilder()
                .WithClientOptions(new MqttClientOptionsBuilder()
                .WithClientId("LoRaWANClient")
                .WithTcpServer("broker.hivemq.com", 1883)
                .WithCredentials("username", "password")
                .WithCleanSession()
                .Build())
                .Build();

            int retryCount = 5; // Number of retry attempts
            int delay = 1000; // Delay between retries in milliseconds

            for (int i = 0; i < retryCount; i++)
            {
                try
                {
                    await _mqttClient.StartAsync(options);
                    return; // Exit the method if connection is successful
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Connection attempt {i + 1} failed: {ex.Message}");
                    if (i == retryCount - 1)
                    {
                        throw; // Rethrow the exception if the last attempt fails
                    }
                    await Task.Delay(delay);
                }
            }
        }

        /// <summary>
        /// Sends a message to the specified MQTT topic asynchronously.
        /// </summary>
        /// <param name="message">The message payload to send.</param>
        /// <returns>A task that represents the asynchronous send operation.</returns>
        public async Task SendMessageAsync(byte[] message)
        {
            var mqttMessage = new MqttApplicationMessageBuilder()
                .WithTopic("your/topic") // Replace with your MQTT topic
                .WithPayload(message)
                .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.ExactlyOnce)
                .WithRetainFlag()
                .Build();

            // Enqueue the message for sending
            await _mqttClient.EnqueueAsync(mqttMessage);
        }
    }
}
