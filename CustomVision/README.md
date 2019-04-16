# Custom Vision
## Definition
Is a branch of [Computer Vision](https://en.wikipedia.org/wiki/Computer_vision) that allows you to build your own custom model. There are many providers for this service and from now on we will consider [Microsoft's](https://www.customvision.ai/).

## Capabilities
As of Now, it has 2 primary capabilities:

 - Classification
 - Object detection

As a General statement, we do not know what kind of models are used in the process, it is not a public information yet, but they have state of the art performance considered how much input data is required.
Another cool thing about Custom Vision is that it comes with some helping models for basic tasks. This can be really useful since the model is already specialized in a context. (logo recognition for example)

## Clasification
This method is used to determine if an individual can be classified in one of the classed that we know or not.
For example we can take a picture of a soda can and determine if it is Cola, Fanta or something else that we have considered a class in the initial training data.
This can be:

 - Multilabel (Multiple tags per image)
 - Multiclass (Single tag per image)

A classic approach for this are Support Vector Machines.

## Object Detection
This method is used when you want to find an entity in an image with those possible outcomes:

 - You do not find the item
 - You find the item once and you need to know where it is
 - You find the item multiple times in multiple places and you need to know about all the occurences
 - You are looking for multiple entities in the picture and you want to know what is where
