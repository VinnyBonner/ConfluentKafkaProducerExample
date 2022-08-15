# Confluent Kafka Producer Example

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


## Set Up



### Create C# Config
1. Click on Data Integration side menu item
2. Select C#
3. Click "Create Kafka cluster API Key"
4. Click "Copy" and save
