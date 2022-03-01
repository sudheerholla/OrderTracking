# OrderTracking
Online Food Order Delivery Tracking covers basic functionalities of a food delivery platform. It’s main purpose is to manage orders placed by customers.

A typical flow of the application -
1) Customer places order (PlaceOrder function)
   The customer places the order and it is stored in db and a message is fired using service bus queue (NewOrderQueue)
2) The queue is picked up by PlaceNewOrderQueueTrigger and it starts the OrderPlacedOrchestrator (durable function).
3) The  OrderPlacedOrchestrator will notify restaurant and wait for 3 minutes for order accepatnace otherwise the order will be cancelled.
4) The restaurant will accpet order using OrderAccepted function and it is added to OrderAcceptedQueue.
5) The AcceptedOrderQueueTrigger will be triggered and it will raise an event which will be picked up by OrderPlacedOrchestrator and it will move the order status to accepted.
6) After this a new orchestrator OrderAcceptedOrchestrator will be started which will wait for delivery partner to pick up the order
7) The delivery partner updates the status with OrderOutForDelivery function
8) Once this is done , the OrderAcceptedOrchestrator will mark status as out of delivery and start the OrderOutForDeliveryOrchestrator durable function.
9) Once order is delivered it is updated by delivery partner using OrderDelivered function and finally mark status as delivered

![image](https://user-images.githubusercontent.com/8706140/156182060-5996c5f3-14c5-4307-b912-2a5c3ac7e3ac.png)


#Architecture 

The application apis are built using azure Http triggered functions for scalabilty (upto 200 instances on consumption plan) and service bus queues .
Durable functions is uesd to handle the workflow.
Clean architecture is followed using cqrs and mediator pattern


#Postman Collection Link of Apis

https://www.getpostman.com/collections/16af6cfd1cdebb7ca82a

#Swagger Api 

http://localhost:7071/api/openapi/ui

![image](https://user-images.githubusercontent.com/8706140/156190454-e2180327-d632-4361-8b01-d8697c882039.png)
