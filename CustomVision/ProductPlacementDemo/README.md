# Product Placement Demo
Maybe I will make a readme, maybe I won't. Luckly, I made One. I will talk about things that you can also find inside this repo or my repos.

## Used Principles and Technologies

 - Microsoft Custom Visio
 - Xamarin Forms
 - Data Serialization
 - Web API


## Custom Vision
We will train a mode that is specialized in detecting soda, Fanta in particular. For this we will use a object detection model that we will train for scratch.

What is really important to have in mind is that the training data is really important and in has 3 dimensions:

 - Quality: we need to use the most relevant features of the object and not something general for multiples entities
 - Quantity: the more data, the better
 - Variety:  the more different contexts, the better

## Xamarin Forms
This is the new way of developing Mobile when you have a lot of common logic: you write 1 app and you have 3.

Since our app will just consume the Web API and it has no platform specific customization, Xamarin forms is the best thing to do.


## Data Serialization
APIs work with serialized data and most of the work with JSON. For this we will user [JSON.NET](https://www.newtonsoft.com/json) combine with [Quicktype](https://quicktype.io/) in order to have the best and cleanest results.

## Web API
I do not have words for this. Just "Thank you for existing".

On a serious note, a web API provides us some functionality on the simple expense of a HTTP request. Simple and facile as that.

Since we will work with images, that is a lot of data, it is recommended to use POST methods. As how it works behind, the same, it is all raw data for the HTTP protocol.