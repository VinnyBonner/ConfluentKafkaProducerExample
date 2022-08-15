// Copyright 2020 Confluent Inc.

// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at

// http://www.apache.org/licenses/LICENSE-2.0

// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace CCloud
{
    class Program
    {
        static async Task<ClientConfig> LoadConfig()
        {
            try
            {                
                var cloudConfig = (await File.ReadAllLinesAsync("confluent.config"))
                    .Where(line => !line.StartsWith("#") && !line.Length.Equals(0))
                    .ToDictionary(
                        line => line.Substring(0, line.IndexOf('=')),
                        line => line.Substring(line.IndexOf('=') + 1));

                var clientConfig = new ClientConfig(cloudConfig);

                return clientConfig;
            }
            catch (Exception e)
            {
                Console.WriteLine($"An error occured reading the config file: {e.Message}");
                System.Environment.Exit(1);
                return null; // avoid not-all-paths-return-value compiler error.
            }
        }

        static async Task CreateTopicMaybe(string name, string strNumPartitions, ClientConfig cloudConfig)
        {
            if(!int.TryParse(strNumPartitions, out int numPartitions))
            {
                Console.WriteLine($"An error occured parsing number of partitions '{strNumPartitions}'");
                System.Environment.Exit(1);
                return;
            }

            using (var adminClient = new AdminClientBuilder(cloudConfig).Build())
            {
                try
                {
                    await adminClient.CreateTopicsAsync(new List<TopicSpecification> {
                        new TopicSpecification { Name = name, NumPartitions = numPartitions } });

                    Console.WriteLine($"Created Topic {name} with {numPartitions} number of partitions.");
                }
                catch (CreateTopicsException e)
                {
                    if (e.Results[0].Error.Code != ErrorCode.TopicAlreadyExists)
                    {
                        Console.WriteLine($"An error occured creating topic {name}: {e.Results[0].Error.Reason}");
                    }
                    else
                    {
                        Console.WriteLine("Topic already exists");
                    }
                }
            }
        }
        
        static void Produce(string topic, ClientConfig config)
        {
            var producerConfig = new ProducerConfig(config)
            {
                Partitioner = Partitioner.Random
            };

            try
            {               
                using (var producer = new ProducerBuilder<string, string>(producerConfig).Build())
                {
                    int numProduced = 0;
                    int numMessages = 100;
                    int totalNumMessages = 0;

                    for (int i = 0; i < numMessages; ++i)
                    {
                        var val = JObject.FromObject(new { count = i }).ToString(Formatting.None);
                        Console.WriteLine($"Producing record: {val}");
                        producer.Produce(topic, new Message<string, string> { Key = null, Value = val },
                            (deliveryReport) =>
                            {
                                if (deliveryReport.Error.Code != ErrorCode.NoError)
                                {
                                    Console.WriteLine($"Failed to deliver message: {deliveryReport.Error.Reason}");
                                }
                                else
                                {
                                    Console.WriteLine($"Produced message to: {deliveryReport.TopicPartitionOffset}");
                                    Console.WriteLine($"Partition: {deliveryReport.Partition}");
                                    numProduced += 1;
                                }
                            });


                        totalNumMessages += numProduced;
                        numProduced = 0;
                        producer.Flush(TimeSpan.FromSeconds(10));
                    }                   

                    Console.WriteLine($"{totalNumMessages} messages were produced to topic {topic}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }


        }

        static void DeleteTopic(string topic, ClientConfig config)
        {
            try
            {
                var adminClientConfig = new AdminClientConfig(config);

                var topics = new List<string>() { topic };

                using (var admin = new AdminClientBuilder(adminClientConfig).Build())
                {
                    var response = admin.DeleteTopicsAsync(topics);

                    response.Wait();

                    if (response.IsFaulted)
                    {
                        Console.WriteLine(response.Exception);
                    } else if (response.IsCompletedSuccessfully)
                    {
                        Console.WriteLine($"Completed Successfully: {response.IsCompletedSuccessfully}");
                    } else
                    {
                        Console.WriteLine(response.Status);
                    }

                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static void PrintUsage()
        {
            Console.WriteLine("usage: .. produce|createtopic|deletetopic <topic> [<numPartitions>]");
            System.Environment.Exit(1);
        }

        static async Task Main(string[] args)
        {
            if (args.Length != 2 && args.Length != 3) { PrintUsage(); }
            
            var mode = args[0];
            var topic = args[1];
            var numPartitions = args.Length == 3 ? args[2] : null;

            var config = await LoadConfig();

            switch (mode.ToLower())
            {
                case "produce":                   
                    Produce(topic, config);
                    break;
                case "deletetopic":
                    DeleteTopic(topic, config);
                    break;
                case "createtopic":
                    await CreateTopicMaybe(topic, numPartitions, config);
                    break;
                default:
                    PrintUsage();
                    break;
            }
        }

    }  
}
