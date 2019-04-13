import pandas as pd
from sklearn.feature_extraction.text import CountVectorizer
from sklearn.cross_validation import train_test_split
from sklearn.linear_model import LogisticRegression

# We are using the Azure ML format, dataframes
# We are going to use data plis into a positive dataframe and a negative dataframe
def azureml_main(dataframe1 = None, dataframe2 = None):
    # When using CSV datasource we need to preparse the data
    postitive = [text for sublist in dataframe2.values for text in sublist if text != None]
    negative = [text for sublist in dataframe1.values for text in sublist if text != None]
    
    # We are going to use a joined format for the output: sentiment | text
    data = [['pos', text] for text in postitive] + [['neg', text] for text in negative]
    
    return pd.DataFrame(data=data,    # values
                  columns=['sentiment', 'text'])

# The first dataset represents the training data
# The second dataset represents our input that needs a prediction
def azureml_main(dataframe1 = None, dataframe2 = None):
    if (dataframe1 is None) or (dataframe2 is None):
        return dataframe1,
    
    test_data = [text for sublist in dataframe2.values for text in sublist if text != None]
    test_count = len(test_data)
    
    # We need to join the data in the preprocessing phase in order to have all the
    # features in place
    data = [text for [_, text] in dataframe1.values] + test_data
    data_labels = [sentiment for [sentiment, _] in dataframe1.values]
    
    # Standard NLP aproach for features
    vectorizer = CountVectorizer(
        analyzer = 'word',
        lowercase = False,
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
    for i in range(test_count):
        ind = features_nd.tolist().index(X_test[i].tolist())
        to_append = [y_pred[i], data[ind].strip()]
        to_show.append(to_append)
        
    result = pd.DataFrame(data=to_show, columns=['text', 'score'])
    
    return result,