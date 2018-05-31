# unity-http

## What is it?
The http system has a quick and easy API forw making http requests within Unity.  
THe Http instance will run the WebRequest coroutines for you so you dont have to create it per request.  
It also includes static helpers for creating the webrequests easily.  
 
## How to use it.
First use one of Http static helper methods to create a HttpRequest. for example we will do a get request.  
`Http.Get("uri");`  
This will return a HttpRequest setup for Http Verb GET.  

The next part is to actually send the HttpRequest, we do this by simply calling Send() on the request object.  
`Http.Get("uri").Send();`  
The Send method signature has the options for onSuccess and onError callbacks.  
HttpRequest.Send(Action<HttpResponse> onSuccess = null, Action<HttpResponse> onError = null);  
Both callbacks include the HttpResponse object.  

You can get the Body of the response in 4 different forms: Text, Bytes, Texture or as an object parsed from Json.  
Heres what it looks like parsed to an object:  
`Http.Get("uri").Send(successResponse => { response.ParseBodyAs<User>(); });`  

```c#
var request = Http.Get("uri");
request.Send(successResponse => 
    {
        // handle success response
    },
    errorResponse => 
    {
        // handle error response
    }
);
```

### Http Utils
The HttpUtils.cs holds client side knowedlge of response code messages. These messages are customizable and more can be added.  
The class also provides a formating and appending method for url parameters.  

### Http Helper 
The `PostAsJson` method provides a HttpRequest configured to send json data to a server via HTTP POST.  
The `PostAsBytes` method provides a HttpRequest configured to send raw bytes to a server via HTTP POST.  

### Alternative use
You can use Http for sending UnityWebRequest with callbacks. For example:  

`Http.Instance.Send(UnityWebRequest.Post("uri", "payload"), (UnityWebRequest request) => { });`
`Http.Instance.Send(UnityWebRequest.Post("uri", "payload"), (HttpResponse response) => { });`
`Http.Instance.Send(UnityWebRequest.Post("uri", "payload"), onError: () => { });`
`Http.Instance.Send(UnityWebRequest.Post("uri", "payload"), onNetworkError: () => { });`
