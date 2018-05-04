# Speech Recognition

Using Bing Speech API, speech is detected and written on the screen then played back using your speaker.

## Getting Started

Download the files in this repository

### Prerequisites

Get Bing Speech API subscription key from 

```
https://azure.microsoft.com/en-us/services/cognitive-services/speech/
```

Go to the file 

```
..\Speech Recognition\ConsoleApp5\Program.cs
```

Replace the string in line 59 "Add your Key subscription key here" with your Key

```
private string SubscriptionKey = "Add your Key subscription key here";
```


## Testing

Run the project on Visual Studio, Then press enter and start speaking, you will find the speech detected with some accuracy and written on the console in the real-time. Then the result with the highest confidence is played back on your speaker.

## Built With

* [Bing Speech API](https://azure.microsoft.com/en-us/services/cognitive-services/speech/) 

## Authors

* **Mohamed Wahballah** - *Initial work* - [mawahballah](https://github.com/mawahballah)
