
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

 - Go to the Datasets Tab and add the 2 datasets by clicking new for each![enter image description here](https://i.imgur.com/4jdbS8g.png)

 - Select the "From Local File"![enter image description here](https://i.imgur.com/MOyXTgE.png)

 - Chose the file from where you have it, you can give it a meaningful name and description![enter image description here](https://i.imgur.com/t32TSLL.png)

  ## The Setup
  In order to create a Web API we are going to create an experiment that is capable of Web I/O. Apart from the ones in the sample, here is a nice way to make one, purely from Python and CSV:
  

 - Go to the EXPERIMENTS Tab and click on NEW, the same way we did for the DataSet![enter image description here](https://i.imgur.com/L7qj80v.png)
 - Now chose "Blank Experiment" (you can check out the samples also, they are built in a different way)![enter image description here](https://i.imgur.com/mG0wrel.png)
 - First, we need to add the previously uploaded DataSets, for that we go to the SavedDatasets Tab (in the current Experiment), expand the "My Datasets" and Drag&Drop those that we need![enter image description here](https://i.imgur.com/ZEOuyF8.png)
 - Our "Endgame" will be something like this:![enter image description here](https://i.imgur.com/aAxUDEU.png)
We will talk how to do it, in detail, in the next section. For now you should have the 2 CSV files at the top of the diagram. Based on the used data the results will differ. (for the same algorithm used)

## The Flow

 1. Our CSV files need to be converted to a Dataset. For that we go under the "Data Format Conversions" tab and pick the "Convert to Dataset". In order to connect components click on the circle of either, a line will appear and it needs to be dragged to the circle of the other component. In most cases they will tip you what to connect to what, based on the type of input/output.
 2. Now we have 2 dataframes, one with positive tweets and one with negative tweets, and we need to convert and combine them into a single dataframe, this will be our next step, using a Python script. It is time to go unde "Python Language Modules" and  use "Execute Python Script". I usually connect the negative dataset to 1 and the positive to 2, if you have researched opinion mining you sure know that negative weights more most of the times. Once the datasets are connected we can work on the code itself. To expand the code editing window click on the module, then on the "Popout the script editor as in the picture".![enter image description here](https://i.imgur.com/mcS7loR.png)
 3. You can convert this in many ways, I like to use [list comprehension](https://www.pythonforbeginners.com/basics/list-comprehensions-in-python). In order to access data in a dataframe, as an array, you need to use the `.values`. The dataframe will provide us multiple lists so we need to join them and also remove the `None` values. In order to use this as a complete input for the next module we need 2 use 2 columns: sentiment and text.
```
import pandas as pd

# We are using the Azure ML format, dataframes
# We are going to use data plis into a positive dataframe and a negative dataframe
def  azureml_main(dataframe1  =  None, dataframe2  =  None):
# When using CSV datasource we need to preparse the data
	postitive = [text for sublist in dataframe2.values for text in sublist if text !=  None]
	negative = [text for sublist in dataframe1.values for text in sublist if text !=  None]

# We are going to use a joined format for the output: sentiment | text
	data = [['pos', text] for text in postitive] + [['neg', text] for text in negative]

return pd.DataFrame(data=data, # values
	columns=['sentiment', 'text'])
```
 4. Now we can use the output of the module (1) as the input of the next module, our main. This will be our actual model and here is the code for it, a classic in Sentiment Analysis.
```
import pandas as pd
from sklearn.feature_extraction.text import CountVectorizer
from sklearn.cross_validation import train_test_split
from sklearn.linear_model import LogisticRegression

# The first dataset represents the training data
# The second dataset represents our input that needs a prediction
def  azureml_main(dataframe1  =  None, dataframe2  =  None):
if (dataframe1 is  None) or (dataframe2 is  None):
	return dataframe1,

	# We need to do the same thing as the previous module did, we could define a function
	test_data = [text for sublist in dataframe2.values for text in sublist if text !=  None]
	test_count =  len(test_data)

	# We need to join the data in the preprocessing phase in order to have all the
	# features in place
	data = [text for [_, text] in dataframe1.values] + test_data
	data_labels = [sentiment for [sentiment, _] in dataframe1.values]

	# Standard NLP aproach for features
	vectorizer = CountVectorizer(
		analyzer  =  'word',
		lowercase  =  False,
	)

	features = vectorizer.fit_transform(
		data
	)
	features_nd = features.toarray() # for easy usage

	# We create our Regression model in order to guess the sentiment
	log_model = LogisticRegression()

	# We need to train only with the test data so we skip the input
	log_model = log_model.fit(X=features_nd[:-test_count], y=data_labels)

	# Time to grab the test data
	X_test = features_nd[-test_count:]

	# And Predict it
	y_pred = log_model.predict(X_test)

	to_show = []

	# Here we append to the output in the desired format

	for i in  range(test_count):
		ind = features_nd.tolist().index(X_test[i].tolist())
		to_append = [y_pred[i], data[ind].strip()]
		to_show.append(to_append)

	result = pd.DataFrame(data=to_show, columns=['text', 'score'])
	return result,
```
 5. Time to add some manual input to test it before deploying. In order to do this we need to add a  "Enter Data Manually" module from the "Data Input and Output" tab. For it chose "CSV", do not check "HasHeader" and add some text that we are going to try and predict. (do not use commas, those are line terminators). As always, this needs to be converted to a dataset and let's connect to the last Python module, the last, on the second dataset.
 6. It is time to Run and check that everything is in place. After run, you can check the results by right clicking on the last module's "1" output and Visualize.![enter image description here](https://i.imgur.com/cEu0m0b.png)![enter image description here](https://i.imgur.com/hzMPlj3.png)
 7. The results will look like those:![enter image description here](https://i.imgur.com/3CWe4dI.png)
 8. Now you can use the "SAVE AS" button and give a meaningful name to the experiment.
 9. Time to prepare the experiment for deployment. For this we will go under the "Web Service" tab, drag&drop both the Input and Output and connect them this way: Input goes to the Dataset2 of the last Python Module (same way as our Manual Input) and the Output goes with the "Result Dataset" of the module.
 10.Run 1 more time, SAVE, and click on "DEPLOY WEB SERVICE". It should open a tab like this.![enter image description here](https://i.imgur.com/T0Vo3Jw.png)
 
##  Web Services From Models
The world is now ours, we have created a WEB API from a Python Model. In case you closed the previous screen you can get back to it by clicking on the "Web Services" tab in ML Studio (it is under Experiments).
Now we can test it manually or using Web Requests.
### Manually
On the screen you can see in the previous picture click on Test and an Input screen should pop up. Enter your text and check the result. The result will appear on the bootom of the screen and will be something like this:
![enter image description here](https://i.imgur.com/yKQs6oF.png)
You can also check its details for mode info on the output. (structure and type)
This has also an option for batch input/output if you want to test it out via the Portal.

### Web Requests

 1. Click "New Web Services Experience"
 2. Chose "Use endpoint"![enter image description here](https://i.imgur.com/c1wwK6H.png)
 3. Go to "API Help" in order to check details and formats or use the provided examples. They are basic web requests and are provided for C#, Python (2 and 3) and R![enter image description here](https://i.imgur.com/gxN5eBF.png)
 4. As far as web requests go you can see all the details in the "API Help" page. Basically you need to have: The proper url, the API Key as the Authorization Bearer and the body as a JSON in the proper format, a sample is provided.![enter image description here](https://i.imgur.com/EruFGzs.png)

# What's next
Try different models and watch this repo. As I am currently working on my degree "Sentiment Analysis and Anomaly Detection in Tweets" I will provide more content using State of the ART papers, comparisons and more. (I will probably us MongoDB as a datasource also)
As far as emotions go, there is no perfect receipt for any data. Some methods work best on Tweets, some work on better on longer texts, some use emoticons and some use hybrids approach. It is our duty to check them out.


## Resources

Base Tutorial & Data: [https://www.twilio.com/blog/2017/12/sentiment-analysis-scikit-learn.html](https://www.twilio.com/blog/2017/12/sentiment-analysis-scikit-learn.html)
Papers to try on: [http://nlpprogress.com/english/sentiment_analysis.html](http://nlpprogress.com/english/sentiment_analysis.html)
Live 100 Tweets: [https://twitter-sentiment-csv.herokuapp.com/](https://twitter-sentiment-csv.herokuapp.com/)
