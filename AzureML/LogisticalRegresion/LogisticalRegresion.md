# Building Sentiment Models from Python
## Overview
Today we are going to learn how to build our custom model and deploy it to the wild using Azure ML studio and Python. For this example in particular we are going to use a classical example of sentiment analysis using a [Logistic Regression](https://en.wikipedia.org/wiki/Logistic_regression).
As for any other Opinion Mining topic we need to have in mind:

 - Our model's results depend on the training data
 - A model may work well on some type of data and terrible on another
 - Fine Tuning is required for anything that is related on ML

From now on, all the discussion happens in [ML Studio](https://studio.azureml.net/) with auxiliary files.

## The Data
A classic source of data for this kind of task are Tweets. We are going to use the same tweets as [Lesley Cordero](https://github.com/lesley2958) since we are going to to what she did in a Jupiter Notebook, but with ML Studio.
A good way of using data is having it in CSV files, I prefer the approach with 1 file per class without tags. We are using 2 files: positive tweets and negative tweets.
What we need to do now:
 - Go to the Datasets Tab and add the 2 datasets by clicking new for each
 - Select the "From Local File"
 - Chose the file from where you have it, you can give it a meaningful name and description

## Resources
Base Tutorial & Data: [https://www.twilio.com/blog/2017/12/sentiment-analysis-scikit-learn.html](https://www.twilio.com/blog/2017/12/sentiment-analysis-scikit-learn.html)