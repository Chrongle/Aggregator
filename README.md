Aggregator Messaging vegetable weighing example for School Project.

Prerequisites:
  - RabbitMQ has to be running

What does it do?
  - In this example 2 Producers sends messages with different vegetables with random weights to an Aggregator.
  - The Aggregator collects the vegetables in a pool. When the treshold of 50g in total weight is reached it sends off the amount of different vegetables to the Consumer.
  - The Consumer tells in the console the total amount and weight of each vegetable recieved in bulk.

How to use:
  - Run the Consumer. It will listen for messages from the aggregator.
  - While the Consumer is running, run the Aggregator. The aggregator will listen for the different vegetable channels.
  - While the Consumer and Aggregator are running, run both the Producers (Preferred at the same time). The producers will send messages with vegetables with different weights to the Aggregator.
  - All of the messages are shown in the consoles.
