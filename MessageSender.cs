using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.SQS;
using Amazon.SQS.Model;

namespace MsgProducer
{
    interface IMessageSender
    {
        Task SendMessage(string content);
    }

    class MessageSender : IMessageSender
    {
        private readonly IAmazonSQS _sqsClient;

        public MessageSender(IAmazonSQS sqsClient)
        {
            _sqsClient = sqsClient;
        }
        
        public async Task SendMessage(string content)
        {
            var queueUrl = Environment.GetEnvironmentVariable("QUEUE_URL");
            Console.WriteLine($"Sending message \'{content}\' to queue \'{queueUrl}\'...");

            var sendMessageRequest = new SendMessageRequest(queueUrl, content)
            {
                MessageAttributes = new Dictionary<string, MessageAttributeValue>
                {
                    ["Sent"] = new MessageAttributeValue
                    {
                        DataType = "String",
                        StringValue = DateTime.UtcNow.ToString("o")
                    }
                }
            };
            await _sqsClient.SendMessageAsync(sendMessageRequest);

            Console.WriteLine($"Sent message \'{content}\' to queue \'{queueUrl}\'");
        }
    }
}