# unity-http

## What is it?
The Http system has a quick and easy API for making http requests within Unity.  
The Http instance will run the WebRequest coroutines for you so you dont have to create it per request.   

## Features
* Singleton
* Fluent API for configuration
* Success, error and network error events
* Super headers

## Requirements
* Unity 2018.3+
* .NET 4.5
* C# 7

## Installation

Add it as a package using [Unity Package Manager](https://docs.unity3d.com/Manual/upm-git.html) or via submodule:  
`git submodule add git@github.com:dubit/unity-http.git Assets/Duck/Http`

## Releasing
* Use [gitflow](https://nvie.com/posts/a-successful-git-branching-model/)
* Create a release branch for the release
* On that branch, bump version number in package json file, any other business (docs/readme updates)
* Merge to master via pull request and tag the merge commit on master.
* Merge back to development.#

## How to use it.
If you are using an AssemblyDefinition then reference the Http Assembly.  
Import the namespace `using DUCK.Http;`

```c#
var request = Http.Get("http://mywebapi.com/")
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
##### Configure
* `SetRedirectLimit(int redirectLimit)`   
* `SetTimeout(int duration)`

Redirect limit subject to Unity's documentation.  
* [Redirect Limit Documentation](https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest-redirectLimit.html)

Progress events will invoke each time the progress value has increased, they are subject to Unity's documentation.
* [Upload Progress Documentation](https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest-uploadProgress.html)
* [Download Progress Documentation](https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest-downloadProgress.html)
 
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
* `string Error`  
* `Texture Texture`  
* `Dictionary<string, string> ResponseHeaders`  

### Super Headers

Super Headers are a type of Header that you can set once to automatically attach to every Request you’re sending.  
They are Headers that apply to all requests without having to manually include them in each HttpRequest SetHeader call.

* `Http.SetSuperHeader(string key, string value)`
* `Http.RemoveSuperHeader(string key)` returns `bool`
* `Http.GetSuperHeaders()` returns `Dictionary<string, string>`


## JSON Response Example
In this given example, the `response.Text` from `http://mywebapi.com/user.json` is:
```json
{
    "id": 92,
    "username": "jason"
}
```

Create a serializable class that maps the data from the json response to fields
```c#
[Serializable]
public class User
{
    [SerializeField]
    public int id;
    [SerializeField]
    public string username;
}
```

We can listen for the event `OnSuccess` with our handler method `HandleSuccess`
```c#
var request = Http.Get("http://mywebapi.com/user.json")
    .OnSuccess(HandleSuccess)
    .OnError(response => Debug.Log(response.StatusCode))
    .Send();
```

Parse the `response.Text` to the serialized class `User` that we declared earlier by using Unity's built in [JSONUtility](https://docs.unity3d.com/ScriptReference/JsonUtility.html)
```c#
private void HandleSuccess(HttpResponse response)
{
     var user = JsonUtility.FromJson<User>(response.Text);
     Debug.Log(user.username);
}
```