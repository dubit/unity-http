# unity-http

## What is it?
The http system has a quick and easy API for making http requests within Unity.  
The Http instance will run the WebRequest coroutines for you so you dont have to create it per request.   
 
## Features
* Singleton
* Fluent API for configuration
* Success, error and network error events
* Super headers

## Requirements
Unity 2017.3 and above (Required for GetTexture, SendRequest() and Assembly Definitions).

## How to use it.

```c#
var request = Http.Get("http://www.dubitlimited.co.uk")
	.SetHeader("Authorization", "username:password")
	.OnSuccess(response => Debug.Log(response.Text))
	.OnError(response => Debug.Log(response.StatusCode))
	.OnDownloadProgress(progress => Debug.Log(progress))
	.Send();
```

## API

### Http Static Methods

All these methods return a new HttpRequest.  

##### Get
* `Http.Get(string uri)`  
* `Http.GetTexture(string uri)`  
##### Post
* `Http.Post(string uri, string postData)`  
* `Http.Post(string uri, WWWForm formData)`  
* `Http.Post(string uri, Dictionary<string, string> formData))`  
* `Http.Post(string uri, List<IMultipartFormSection> multipartForm)`  
* `Http.Post(string uri, byte[] bytes, string contentType)`  
##### Post JSON
* `Http.PostJson(string uri, string json)`  
* `Http.PostJson<T>(string uri, T payload)` 
##### Put
* `Http.Put(string uri, byte[] bodyData)` 
* `Http.Put(string uri, string bodyData)` 
##### Misc
* `Http.Delete(string uri)`  
* `Http.Head(string uri)`  

### Http Request Configuration Methods

All these methods return the HttpRequest instance.  
##### Headers
* `SetHeader(string key, string value)`  
* `SetHeaders(IEnumerable<KeyValuePair<string, string>> headers)`  
* `RemoveHeader(string key)`  
* `RemoveSuperHeaders()`  
##### Events
* `OnSuccess(Action<HttpResonse> response)`  
* `OnError(Action<HttpResonse> response)`  
* `OnNetworkError(Action<HttpResonse> response)`  
##### Progress
* `OnUploadProgress(Action<float> progress)`  
* `OnDownloadProgress(Action<float> progress)`  

### Http Request

* `HttpRequest Send()`  
* `void Abort()`  

### Http Response
The callbacks for `OnSuccess`, `OnError` and `OnNetworkError` all return you a `HttpResponse`.  
This has the following properties:  
##### Properties
* `string Url`  
* `bool IsSuccessful`  
* `bool IsHttpError`  
* `bool IsNetworkError`  
* `long StatusCode`  
* `ResponseType ResponseType`  
* `byte[] Bytes`  
* `string Text`  
* `Texture Texture`  
* `Dictionary<string, string> ResponseHeaders`  

### Super Headers

Super Headers are a type of Header that you can set once to automatically attach to every Request you’re sending.  
They are Headers that apply to all requests without having to manually include them in each HttpRequest SetHeader call.

* `void Http.SetSuperHeader(string key, string value)`  
* `bool RemoveSuperHeader(string key)`  

* `Dictionary<string, string> GetSuperHeaders()`  