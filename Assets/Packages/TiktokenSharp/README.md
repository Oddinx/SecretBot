# TiktokenSharp

Due to the lack of a C# version of `cl100k_base` encoding (gpt-3.5-turbo), I have implemented a basic solution with encoding and decoding methods based on the official Rust implementation.

Currently, `cl100k_base` `p50k_base` has been implemented. Other encodings will be added in future submissions. If you encounter any issues or have questions, please feel free to submit them on the `lssues`."

If you want to use the ChatGPT C# library that integrates this repository and implements context-based conversation, please visit [ChatGPTSharp](https://github.com/aiqinxuancai/ChatGPTSharp).

### Getting Started

TiktokenSharp is available as [NuGet package](https://www.nuget.org/packages/TiktokenSharp/).

```csharp
using TiktokenSharp;

//use model name
TikToken tikToken = TikToken.EncodingForModel("gpt-3.5-turbo");
var i = tikToken.Encode("hello world"); //[15339, 1917]
var d = tikToken.Decode(i); //hello world

//use encoding name
TikToken tikToken = TikToken.GetEncoding("cl100k_base");
var i = tikToken.Encode("hello world"); //[15339, 1917]
var d = tikToken.Decode(i); //hello world
```

## Update

### 1.0.4 20230424
* Add method TikToken.GetEncoding(encodingName).

### 1.0.3 20230321
* **GetEncodingSetting** now supports the model of **gpt-4** and also allows for encoding names to be directly passed in.

### 1.0.2 20230317
* add method **TikToken.PBEFileDirectory** to allow for custom storage directory of bpe files. the path needs to be set before **TikToken.EncodingForModel()**.

### 1.0.1 20230313
* p50k_base encoding algorithm that supports the text-davinci-003 model.


