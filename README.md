# Confluent Kafka Producer Example
#### This Example code goes along with my [Azure Kafka Trigger Function Examples](https://github.com/VinnyBonner/AzureKafkaTriggerFunctionExamples)

## Prerequisites
Create a free [Confluent Cloud Account](https://www.confluent.io/)



### Create Kafka Cluster Steps:
1. Create a Kafka Cluster (Free Tier Available)
2. Select Cloud Provider and region (Recommend Azure with closest region and single zone availability)
3. Enter payment info or press Skip Payment to use free trial.
4. Enter a name for the Kafka Cluster and click "Launch Cluster"

### Create Kafka Topic
1. Click on Topics side menu item
2. Click + Add Topic
3. Set the Topic name, and number of partitions (Recommend reading about how to [determine the number of partitions](https://docs.microsoft.com/en-us/azure/architecture/reference-architectures/event-hubs/partitioning-in-event-hubs-and-kafka#determine-the-number-of-partitions) needed)
4. Click Show advanced settings to configure storage cleanup and retention settings, and max message size.
5. Click Save & create

## Set up C# Producer (Standard Message Format)
### Get Configuration from the Confluent Portal
1. Click on Data Integration side menu item
2. Select C#
3. Click "Create Kafka Cluster API Key"
4. Click "Copy" and Paste into the confluent.config file which is part of the project

### Set Up C# Producer  
5. [Download the project files](https://github.com/VinnyBonner/ConfluentKafkaProducerExample/tree/main/CSharpExample)
6. Open the .csproj in [Visual Studios](https://visualstudio.microsoft.com/)
7. Paste the Confluent config info from step 4 into the confluent.config file and save
8. Build the solution

### Run the project
9. You can run the project using the command line with the following arguments
  - Produce <TopicName> <NumberOfMessagesToProduce>
  - DeleteTopic <TopicName>
  - CreateTopic <TopicName> <NumberOfPartitions>

## Set up Java Producer (Avo Schema Message Format)

### Create the Avo Schema
